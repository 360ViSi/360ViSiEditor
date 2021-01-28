using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayPauseButton : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoPlayer;

    [SerializeField] Image _playPauseImage;
    [SerializeField] Sprite _playSprite;
    [SerializeField] Sprite _pauseSprite;

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
}
