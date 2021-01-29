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
    [SerializeField] RectTransform _sliderHandleRect;


    private void Update()
    {
        //S TODO change to function with events
        if (_videoPlayer.isPaused)
            _playPauseImage.sprite = _playSprite;
        else _playPauseImage.sprite = _pauseSprite;
    }

    public void TogglePause()
    {
        if (_videoPlayer.isPaused)
            _videoPlayer.Play();
        else _videoPlayer.Pause();
    }

    public void SetLoopPoint()
    {
        _loopPointImageRect.anchorMin = _sliderHandleRect.anchorMin;
        _loopPointImageRect.anchorMax = _sliderHandleRect.anchorMax;
        _editorVideoPlayer.SetLoopTimeToVideo();
    }
}
