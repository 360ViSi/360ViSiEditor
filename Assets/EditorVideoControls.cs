using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EditorVideoControls : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] EditorVideoPlayer _editorVideoPlayer;

    [Header("Play/Pause")]
    [SerializeField] Image _playPauseImage;
    [SerializeField] Sprite _playSprite;
    [SerializeField] Sprite _pauseSprite;

    [Header("Loop")]
    [SerializeField] RectTransform _loopPointImageRect;
    [SerializeField] RectTransform _startPointImageRect;
    [SerializeField] RectTransform _endPointImageRect;
    [SerializeField] RectTransform _sliderHandleRect;


    private void Update()
    {
        //S TODO change to function with events
        if (_videoPlayer.isPlaying)
            _playPauseImage.sprite = _pauseSprite;
        else _playPauseImage.sprite = _playSprite;
    }

    public void TogglePause()
    {
        if (_videoPlayer.isPlaying)
            _videoPlayer.Pause();
        else _videoPlayer.Play();
    }

    public void SetLoopPoint()
    {
        _loopPointImageRect.anchorMin = _sliderHandleRect.anchorMin;
        _loopPointImageRect.anchorMax = _sliderHandleRect.anchorMax;
        _editorVideoPlayer.SetLoopTimeToVideo();
    }
    public void SetVideoStartPoint()
    {
        _startPointImageRect.anchorMin = _sliderHandleRect.anchorMin;
        _startPointImageRect.anchorMax = _sliderHandleRect.anchorMax;
        _editorVideoPlayer.SetStartTimeToVideo();
    }
    public void SetVideoEndPoint()
    {
        _endPointImageRect.anchorMin = _sliderHandleRect.anchorMin;
        _endPointImageRect.anchorMax = _sliderHandleRect.anchorMax;
        _editorVideoPlayer.SetEndTimeToVideo();
    }
}
