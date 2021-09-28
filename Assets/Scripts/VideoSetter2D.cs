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

public string FullPath { get => folderPath + fileName; }

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


//public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
//{

//    // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

//    Texture2D SpriteTexture = LoadTexture(FilePath);
//    Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

//    return NewSprite;
//}

//public void SetOldLoadedSprite(string spriteDataString, float PixelsPerUnit = 100.0f)
//{
//    spriteData = spriteDataString;
//    byte[] spriteByte = Convert.FromBase64String(spriteDataString);
//    Texture2D LoadedTexture = new Texture2D(2, 2);
//    LoadedTexture.LoadImage(spriteByte);
//    Sprite NewSprite = Sprite.Create(LoadedTexture, new Rect(0, 0, LoadedTexture.width, LoadedTexture.height), new Vector2(0, 0), PixelsPerUnit);

//    image.sprite = NewSprite;
//}

//public Texture2D LoadTexture(string FilePath)
//{
//    // Load a PNG or JPG file from disk to a Texture2D
//    // Returns null if load fails

//    Texture2D Tex2D;
//    byte[] FileData;

//    if (File.Exists(FilePath))
//    {
//        FileData = File.ReadAllBytes(FilePath);
//        Tex2D = new Texture2D(2, 2);                // Create new "empty" texture
//        if (Tex2D.LoadImage(FileData))              // Load the imagedata into the texture (size is set automatically)
//        {
//            spriteData = Convert.ToBase64String(FileData, 0, FileData.Length);
//            return Tex2D;                           // If data = readable -> return texture
//        }
//    }
//    Debug.LogWarning("Image not loaded");
//    return null;                                    // Return null if load failed
//}
}
