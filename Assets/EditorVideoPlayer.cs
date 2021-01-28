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
    [SerializeField] TimeSlider _timeSlider;
    bool _isSliderHeld = false;
    float watchedTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.prepareCompleted += (VideoPlayer player) => player.Play();
        _timeSlider.OnHold.AddListener(StartHold);
        _timeSlider.OnRelease.AddListener(EndHold);
    }

    void StartHold()
    {
        _isSliderHeld = true;
        _videoPlayer.Pause();
    }
    void EndHold()
    {
        _isSliderHeld = false;
        _videoPlayer.time = (double)(_videoPlayer.length * _timeSlider.value);
        _videoPlayer.Play();
    }
    public void ChangeVideo(string filename)
    {
        var fullpath = @"C:\Unity\" + filename;
        if (!File.Exists(fullpath))
        {
            Debug.Log($"No file with path {fullpath} found");
            return;
        }
        _videoPlayer.url = fullpath;
        _videoPlayer.Prepare();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TogglePause();
        UpdateVideoStatus();
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
}
