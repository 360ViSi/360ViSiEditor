using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EditorVideoControls : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] EditorVideoPlayer editorVideoPlayer;
    [SerializeField] GameObject nodeCanvas;

    [Header("Play/Pause")]
    [SerializeField] Image playPauseImage;
    [SerializeField] Sprite playSprite;
    [SerializeField] Sprite pauseSprite;

    [Header("Time Line Rects")]
    [SerializeField] RectTransform loopPointImageRect;
    [SerializeField] RectTransform videoStartPointImageRect;
    [SerializeField] RectTransform videoEndPointImageRect;
    [SerializeField] RectTransform actionStartPointImageRect;
    [SerializeField] RectTransform actionEndPointImageRect;
    [SerializeField] RectTransform sliderHandleRect;

    [Header("Buttons")]
    [SerializeField] GameObject loopButton;
    [SerializeField] GameObject videoStartButton;
    [SerializeField] GameObject videoEndButton;
    [SerializeField] GameObject actionStartButton;
    [SerializeField] GameObject actionEndButton;

    private void Update()
    {
        //S TODO set a real input for this
        if(Input.GetKeyDown(KeyCode.Tab))
            nodeCanvas.SetActive(!nodeCanvas.activeInHierarchy);

        //S LATER - set this to happen with events
        if (videoPlayer.isPlaying)
            playPauseImage.sprite = pauseSprite;
        else playPauseImage.sprite = playSprite;
    }

    public void TogglePause()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else videoPlayer.Play();
    }

    #region Marker points
    public void SetLoopPoint(bool setToVideo = false)
    {
        loopPointImageRect.anchorMin = sliderHandleRect.anchorMin;
        loopPointImageRect.anchorMax = sliderHandleRect.anchorMax;
        if (setToVideo)
            editorVideoPlayer.SetLoopTimeToVideo();
    }
    public void SetVideoStartPoint(bool setToVideo = false)
    {
        videoStartPointImageRect.anchorMin = sliderHandleRect.anchorMin;
        videoStartPointImageRect.anchorMax = sliderHandleRect.anchorMax;
        if (setToVideo)
            editorVideoPlayer.SetStartTimeToVideo();
    }
    public void SetVideoEndPoint(bool setToVideo = false)
    {
        videoEndPointImageRect.anchorMin = sliderHandleRect.anchorMin;
        videoEndPointImageRect.anchorMax = sliderHandleRect.anchorMax;
        if (setToVideo)
            editorVideoPlayer.SetEndTimeToVideo();
    }

    public void SetActionStartPoint(bool setToVideo = false)
    {
        actionStartPointImageRect.anchorMin = sliderHandleRect.anchorMin;
        actionStartPointImageRect.anchorMax = sliderHandleRect.anchorMax;
        if (setToVideo)
            editorVideoPlayer.SetStartTimeToAction();
    }
    public void SetActionEndPoint(bool setToVideo = false)
    {
        actionEndPointImageRect.anchorMin = sliderHandleRect.anchorMin;
        actionEndPointImageRect.anchorMax = sliderHandleRect.anchorMax;
        if (setToVideo)
            editorVideoPlayer.SetEndTimeToAction();
    }
    #endregion

    public void SetCurrentControlsToVideo(bool value)
    {
        videoEndButton.SetActive(value);
        videoStartButton.SetActive(value);
        
        var loop = NodeInspector.instance.CurrentVideoNode.getLoop();
        loopButton.SetActive(loop);
        loopPointImageRect.gameObject.SetActive(loop);
        
        if(NodeInspector.instance.CurrentActionNode != null && NodeInspector.instance.CurrentActionNode.getAutoEnd())
            value = !value;

        actionStartButton.SetActive(!value);
        actionEndButton.SetActive(!value);
        actionEndPointImageRect.gameObject.SetActive(!value);
        actionStartPointImageRect.gameObject.SetActive(!value);
    }
}
