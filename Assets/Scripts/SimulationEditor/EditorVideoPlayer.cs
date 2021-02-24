using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

[RequireComponent(typeof(VideoPlayer))]
public class EditorVideoPlayer : MonoBehaviour
{
    VideoPlayer videoPlayer;
    [SerializeField] NodeInspector nodeInspector;
    [SerializeField] TimelineDraggable timeSlider;
    [SerializeField] SimulationTimeline simulationTimeline;
    [SerializeField] TMPro.TMP_Text currentTime;
    [SerializeField] Material renderMaterial;
    [SerializeField] EditorVideoControls editorVideoControls;
    bool isSliderHeld = false;
    bool playAfterHold = false;
    float currentVideoLoopTime = 0;
    float currentVideoStartTime = 0;
    float currentVideoEndTime = 1;
    bool realLoop; //Determines if video actually looped OR was just rewound

    public VideoPlayer VideoPlayer { get => videoPlayer; }

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        timeSlider.OnHold.AddListener(StartHold);
        timeSlider.OnRelease.AddListener(EndHold);
        //S TODO perhaps move this to when video is paused and revert when playing?
        renderMaterial.SetFloat("_Exposure", .5f);
    }



    private void OnDisable()
    {
        renderMaterial.SetFloat("_Exposure", 1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TogglePause();

        UpdateVideoStatus();

        if (videoPlayer.length == 0 || !videoPlayer.isPlaying)
            return;

        // S TODO - has to be a better way to solve this, maybe with just checking for a stop?
        var endThresholdFrames = 1;
        if (currentVideoEndTime == 1)
            endThresholdFrames++;
        if (videoPlayer.frame < (videoPlayer.frameCount * currentVideoEndTime - endThresholdFrames))
            return;

        if (nodeInspector.CurrentVideoNode.getLoop() == false)
        {
            videoPlayer.Stop();
            return;
        }

        realLoop = true;
        LoopVideo(videoPlayer);
    }

    ///<summary>
    /// Tries to change the video of the videoplayer,
    /// returns true if succesful, false if video not found
    ///</summary>
    public bool TryChangeVideo(string filename)
    {
        var wasPaused = videoPlayer.isPaused;

        var fullpath = @"C:\Unity\" + filename;
        fullpath.Replace('\\', Path.DirectorySeparatorChar);
        if (!File.Exists(fullpath))
        {
            Debug.LogError($"No file with path {fullpath} found");
            return false;
        }
        videoPlayer.url = fullpath;

        videoPlayer.prepareCompleted -= PrepareVideo(videoPlayer, true);
        videoPlayer.prepareCompleted -= PrepareVideo(videoPlayer, false);


        if (wasPaused)
            videoPlayer.prepareCompleted += PrepareVideo(videoPlayer, true);
        else
            videoPlayer.prepareCompleted += PrepareVideo(videoPlayer, false);

        videoPlayer.Prepare();
        RefreshMarkers();
        return true;
    }

    IEnumerator SetVideoStartTimeOnVideoChange()
    {
        yield return new WaitForFixedUpdate(); 
        videoPlayer.time = nodeInspector.CurrentVideoNode.getStartTime() * videoPlayer.length;
    }

    /*
    All markers are basically handled individually, could maybe generalize
    partly but not sure if worth the effort.
    */
    #region Markers
    public void RefreshMarkers()
    {
        if (nodeInspector.CurrentActionNode != null)
        {
            GetStartTimeFromAction();
            GetEndTimeFromAction();
            editorVideoControls.SetCurrentControlsToVideo(false);
        }
        else
        {
            editorVideoControls.SetCurrentControlsToVideo(true);
        }
        GetLoopTimeFromVideo();
        GetStartTimeFromVideo();
        GetEndTimeFromVideo();
    }
    ///<summary>
    /// Loop time is set from EditorVideoControls function called from a UI button
    /// set to the current time, is there need for more customization 
    /// -> might get too complex for users
    ///</summary>
    public void SetLoopTimeToVideo()
    {
        nodeInspector.CurrentVideoNode.setLoopTime(timeSlider.Value);
        currentVideoLoopTime = timeSlider.Value;
        nodeInspector.CreateFields(nodeInspector.CurrentVideoNode, true);
    }

    void GetLoopTimeFromVideo()
    {
        var loopTime = nodeInspector.CurrentVideoNode.getLoopTime();
        timeSlider.Value = loopTime;
        currentVideoLoopTime = loopTime;
        editorVideoControls.SetLoopPoint();
    }

    public void SetStartTimeToVideo(float value)
    {
        nodeInspector.CurrentVideoNode.setStartTime(value);
        currentVideoStartTime = value;
        simulationTimeline.StartTime = value;
        nodeInspector.CreateFields(nodeInspector.CurrentVideoNode, true);
    }

    void GetStartTimeFromVideo()
    {
        var startTime = nodeInspector.CurrentVideoNode.getStartTime();
        timeSlider.Value = startTime;
        currentVideoStartTime = startTime;
        simulationTimeline.StartTime = startTime;
    }
    public void SetEndTimeToVideo(float value)
    {
        nodeInspector.CurrentVideoNode.setEndTime(value);
        currentVideoEndTime = value;
        simulationTimeline.EndTime = value;
        nodeInspector.CreateFields(nodeInspector.CurrentVideoNode, true);
    }
    void GetEndTimeFromVideo()
    {
        var endTime = nodeInspector.CurrentVideoNode.getEndTime();
        timeSlider.Value = endTime;
        currentVideoEndTime = endTime;
        simulationTimeline.EndTime = endTime;
    }

    public void SetStartTimeToAction()
    {
        nodeInspector.CurrentActionNode.setStartTime(timeSlider.Value);
        nodeInspector.CreateFields(nodeInspector.CurrentActionNode, true);
    }
    void GetStartTimeFromAction()
    {
        var startTime = nodeInspector.CurrentActionNode.getStartTime();
        timeSlider.Value = startTime;
        editorVideoControls.SetActionStartPoint();
    }
    public void SetEndTimeToAction()
    {
        nodeInspector.CurrentActionNode.setEndTime(timeSlider.Value);
        nodeInspector.CreateFields(nodeInspector.CurrentActionNode, true);
    }
    void GetEndTimeFromAction()
    {
        var endTime = nodeInspector.CurrentActionNode.getEndTime();
        timeSlider.Value = endTime;
        editorVideoControls.SetActionEndPoint();
    }

    #endregion

    private VideoPlayer.EventHandler PrepareVideo(VideoPlayer player, bool pause)
    {
        StartCoroutine(SetVideoStartTimeOnVideoChange());

        if (pause)
            player.Pause();
        else player.Play();

        return null;
    }

    private void LoopVideo(VideoPlayer player)
    {
        //S NOTE - Skip On Drop needs to be disabled in the videoplayer loop points are used
        //Having it enabled causes the videoplayer to go to frame 0 to wait for the skip 
        //to the actual loop point
        if (realLoop && nodeInspector.CurrentVideoNode.getLoop())
        {
            StartCoroutine(LoopRoutine((float)(player.length * nodeInspector.CurrentVideoNode.getLoopTime())));
        }
        else if (realLoop)
        {
            StartCoroutine(LoopRoutine((float)(player.length * nodeInspector.CurrentVideoNode.getStartTime())));
        }
        else player.time = timeSlider.Value;
    }

    private void TogglePause()
    {
        if (isSliderHeld) return;

        if (videoPlayer.isPaused) videoPlayer.Play();
        else videoPlayer.Pause();
    }

    private void UpdateVideoStatus()
    {
        if (isSliderHeld)
        {
            currentTime.text = Utilities.FloatToTime(timeSlider.Value, (float)videoPlayer.length, true);
            return;
        }

        if (videoPlayer.length == 0) return;

        var videoTime = (float)(videoPlayer.time / videoPlayer.length);
        timeSlider.Value = videoTime;
        currentTime.text = Utilities.FloatToTime(videoTime, (float)videoPlayer.length, true);
    }

    private void StartHold()
    {
        isSliderHeld = true;
        playAfterHold = !videoPlayer.isPaused;
        videoPlayer.Pause();
        realLoop = false;
    }
    private void EndHold(float value)
    {
        isSliderHeld = false;
        videoPlayer.frame = (long)(videoPlayer.frameCount * value);

        if (playAfterHold)
            videoPlayer.Play();

    }
    IEnumerator LoopRoutine(float time)
    {
        videoPlayer.time = time;
        videoPlayer.Pause();
        yield return new WaitForSeconds(.25f); // it takes some time for the VideoPlayer to seek
        videoPlayer.Play();
    }
}
