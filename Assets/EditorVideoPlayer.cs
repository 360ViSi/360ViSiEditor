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
    VideoPlayer _videoPlayer;
    [SerializeField] NodeInspector _nodeInspector;
    [SerializeField] TimeSlider _timeSlider;
    [SerializeField] Material _renderMat;
    [SerializeField] EditorVideoControls _editorVideoControls;
    bool _isSliderHeld = false;
    bool _playAfterHold = false;
    float _currentVideoLoopTime = 0;
    bool _realLoop; //Determines if video actually looped OR was just rewound
    
    void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.loopPointReached += LoopVideo;
        _timeSlider.OnHold.AddListener(StartHold);
        _timeSlider.OnRelease.AddListener(EndHold);
        //S TODO perhaps move this to when video is paused and revert when playing?
        _renderMat.SetFloat("_Exposure", .5f);
    }



    private void OnDisable()
    {
        _renderMat.SetFloat("_Exposure", 1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TogglePause();
        UpdateVideoStatus();

        if(_videoPlayer.frame > (long)_videoPlayer.frameCount - 5)
            _realLoop = true;
    }

    public void ChangeVideo(string filename)
    {
        var wasPaused = _videoPlayer.isPaused;

        var fullpath = @"C:\Unity\" + filename;
        if (!File.Exists(fullpath))
        {
            Debug.LogError($"No file with path {fullpath} found");
            return;
        }
        _videoPlayer.url = fullpath;

        _videoPlayer.prepareCompleted -= PrepareVideo(_videoPlayer, true);
        _videoPlayer.prepareCompleted -= PrepareVideo(_videoPlayer, false);

        if (wasPaused)
            _videoPlayer.prepareCompleted += PrepareVideo(_videoPlayer, true);
        else
            _videoPlayer.prepareCompleted += PrepareVideo(_videoPlayer, false);

        GetLoopTimeFromVideo();
        _videoPlayer.Prepare();
    }

    ///<summary>
    /// Loop time is set from EditorVideoControls function called from a UI button
    /// set to the current time, is there need for more customization 
    /// -> might get too complex for users
    ///</summary>
    public void SetLoopTimeToVideo()
    {
        _nodeInspector.CurrentVideoNode.setLoopTime(_timeSlider.value);
        _currentVideoLoopTime = _timeSlider.value;
    }

    public void GetLoopTimeFromVideo()
    {
        var loopTime = _nodeInspector.CurrentVideoNode.getLoopTime();
        _timeSlider.value = loopTime;
        _currentVideoLoopTime = loopTime;
        _editorVideoControls.SetLoopPoint();
    }

    private VideoPlayer.EventHandler PrepareVideo(VideoPlayer player, bool pause)
    {
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
        if(_realLoop)
            player.time = (double)(player.length * _currentVideoLoopTime);
    }

    private void TogglePause()
    {
        if (_isSliderHeld) return;

        if (_videoPlayer.isPaused) _videoPlayer.Play();
        else _videoPlayer.Pause();
    }

    private void UpdateVideoStatus()
    {
        if (_videoPlayer.length == 0 || _isSliderHeld) return;
        var videoTime = (float)(_videoPlayer.time / _videoPlayer.length);
        _timeSlider.value = videoTime;
    }

    private void StartHold()
    {
        _isSliderHeld = true;
        _playAfterHold = !_videoPlayer.isPaused;
        _videoPlayer.Pause();
        _realLoop = false;
    }
    private void EndHold()
    {
        _isSliderHeld = false;
        _videoPlayer.frame = (long)(_videoPlayer.frameCount * _timeSlider.value);
        
        if (_playAfterHold)
            _videoPlayer.Play();
        
    }
}
