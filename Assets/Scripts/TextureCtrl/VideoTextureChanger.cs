using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class VideoTextureChanger : MonoBehaviour
{
    //variables used in editor
    [SerializeField]
    private SO_StringList videoFileNames;
    [SerializeField] private SimulationManager simulationManager;

    //private variables
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] TMPro.TMP_Text videoTimeText;
    float currentVideoEndTime = 1;
    float currentVideoStartTime = 0;
    float currentVideoLoopTime = 0;
    bool currentVideoLoop = false;

    // Start is called before the first frame update
    void Start()
    {
        //initialize Video player
        videoPlayer.prepareCompleted += PrepareCompleted;
    }

    private void Update()
    {
        videoTimeText.text = Utilities.FloatToTime((float)(videoPlayer.time / videoPlayer.length), (float)videoPlayer.length, true);

        if (videoPlayer.length == 0 || !videoPlayer.isPlaying)
            return;


        // S TODO - has to be a better way to solve this, maybe with just checking for a stop?
        var endThresholdFrames = 1;
        if (currentVideoEndTime == 1)
            endThresholdFrames++;
        if (videoPlayer.frame < (videoPlayer.frameCount * currentVideoEndTime - endThresholdFrames))
            return;

        if (currentVideoLoop == false)
        {
            videoPlayer.Stop();
            simulationManager.AutoEnd();
            return;
        }
        StartCoroutine(LoopRoutine((float)videoPlayer.length * currentVideoLoopTime));
    }

    IEnumerator LoopRoutine(float time)
    {
        videoPlayer.time = time;
        videoPlayer.Pause();
        yield return new WaitForSeconds(.25f); // it takes some time for the VideoPlayer to seek
        videoPlayer.Play();
    }

    void PrepareCompleted(VideoPlayer videoPlayer)
    {
        Debug.Log("video is ready");
        videoPlayer.time = videoPlayer.length * currentVideoStartTime;
        videoPlayer.Play();
    }

    public void ChangeVideo(string videoPath)
    {
        CheckVideoExtension(videoPath);

        videoPlayer.url = videoPath;

        //get currents
        var videoPart = simulationManager.getCurrentVideoPart();
        currentVideoStartTime = videoPart.getStartTime();
        currentVideoEndTime = videoPart.getEndTime();
        currentVideoLoop = videoPart.getLoop();
        currentVideoLoopTime = videoPart.getLoopTime();
        Debug.Log($"start: {currentVideoStartTime}, end: {currentVideoEndTime}, Loop: {currentVideoLoop}, Looptime: {currentVideoLoopTime}");

        videoPlayer.Prepare();
    }

    private void CheckVideoExtension(string path)
    {
        var pathSplit = path.Split('.');
        var extension = pathSplit[pathSplit.Length - 1];

        if (videoFileNames.data.Contains(extension) == false)
            throw new System.Exception("Video file format not supported");
    }
}