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
    [SerializeField] private RawImage image;
    private string folderPath;
    private string fileName;
    private VideoPlayer videoPlayer;
    private InfoCreator infoCreator;
    private string videoPath;
    public string VideoPath { get => videoPath; set => videoPath = value; }

    public string FullPath { get => folderPath + fileName; set => videoPath = value; }


    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        infoCreator = GetComponentInParent<InfoCreator>();

    }
    //Opens a file browser that filters files according to extension. Also sets the videoPath -variable
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

    //Loads the videoTexture and video path
    private void VideoToSimulation()
    {
        if (File.Exists(FullPath) == false)
        {
            Debug.LogError($"File not found at: {FullPath}");
            return;
        }
        image.texture = infoCreator.VideoTexture;
        videoPlayer.url = FullPath;
    }
    //Loads video texture and video path from videoPath-variable
    public void SetOldLoadedVideo(string videoURL)
    {
        image.texture = infoCreator.VideoTexture;
        videoPath = videoURL;
        videoPlayer.url = videoPath;
    }
}
