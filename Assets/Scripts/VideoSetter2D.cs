using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using SFB;
using UnityEngine.UI;
using System.IO;
using System;

public class VideoSetter2D : MonoBehaviour
{
    private string folderPath;
    private string fileName;
    [SerializeField] private RawImage image;
    private InfoCreator infoCreator;
    private string videoPath;
    public string VideoPath { get => videoPath; set => videoPath = value; }

    public string FullPath { get => folderPath + fileName; set => videoPath = value; }

    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        infoCreator = GetComponentInParent<InfoCreator>();

    }
    public void OpenImageFile(string path)
    {
        var extensions = new[] {
        new ExtensionFilter("Video Files", "mp4" )
        };

        if (path == "")
            path = StandaloneFileBrowser.OpenFilePanel("Load a new video file", folderPath, extensions, false)[0];

        folderPath = Utilities.FolderPathFromFilePath(path);
        fileName = Utilities.FileNameFromFilePath(path, true);
        videoPath = FullPath;
        VideoToSimulation();
    }

    //Sets the image sprite
    private void VideoToSimulation()
    {
        if (File.Exists(FullPath) == false)
        {
            Debug.LogError($"File not found at: {FullPath}");
            return;
        }
        image.texture = infoCreator.videoTexture;
        videoPlayer.url = FullPath;
    }
    public void SetOldLoadedVideo(string videoURL)
    {
        image.texture = infoCreator.videoTexture;
        videoPath = videoURL;
        videoPlayer.url = videoPath;
    }
}
