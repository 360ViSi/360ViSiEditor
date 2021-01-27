using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class EditorVideoPlayer : MonoBehaviour
{
    VideoPlayer _videoPlayer;
    float watchedTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.prepareCompleted += (VideoPlayer player) => player.Play();
        _videoPlayer.loopPointReached += ResetTimer();
    }

    private VideoPlayer.EventHandler ResetTimer()
    {

        return null;
    }

    public void ChangeVideo(string filename)
    {
        var fullpath = @"C:\Unity\" + filename;
        if(!File.Exists(fullpath)){
            Debug.Log($"No file with path {fullpath} found");
            return;
        }
        _videoPlayer.url = fullpath;
        _videoPlayer.Prepare();
    }

    public void Update()
    {
        UpdateVideoStatus();
    }

    private void UpdateVideoStatus()
    {
        print(_videoPlayer.time + "/" +_videoPlayer.length);
    }
}
