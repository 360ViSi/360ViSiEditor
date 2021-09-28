﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System.IO;
using System;

public class ImageSetter : MonoBehaviour
{
    private string folderPath;
    private string fileName;
    private string spriteData;
    public string SpriteData { get => spriteData; set => spriteData = value; }
    private string fullPath { get => folderPath + fileName; }

    public RawImage image;

    private void Awake()
    {
        image = GetComponent<RawImage>();
        
    }
    public void OpenImageFile(string path)
    {
        var extensions = new[] {
        new ExtensionFilter("Image Files", "tga","tif","psd","bitmap","png","jpeg" )
        };

        if (path == "")
            path = StandaloneFileBrowser.OpenFilePanel("Load a new image file", folderPath, extensions, false)[0];

        folderPath = Utilities.FolderPathFromFilePath(path);
        fileName = Utilities.FileNameFromFilePath(path, true);

        ImageToSimulation();
    }

    //Sets the image sprite
    private void ImageToSimulation()
    {
        if (File.Exists(fullPath) == false)
        {
            Debug.LogError($"File not found at: {fullPath}");
            return;
        }

        image.texture = LoadNewSprite(fullPath);

    }
    public Texture LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
                
        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

        return NewSprite.texture;
    }

    public void SetOldLoadedSprite(string spriteDataString, float PixelsPerUnit = 100.0f)
    {
        spriteData = spriteDataString;
        byte[] spriteByte = Convert.FromBase64String(spriteDataString);
        Texture2D LoadedTexture = new Texture2D(2,2); 
        LoadedTexture.LoadImage(spriteByte);
        Sprite NewSprite = Sprite.Create(LoadedTexture, new Rect(0, 0, LoadedTexture.width, LoadedTexture.height), new Vector2(0, 0), PixelsPerUnit);

        image.texture = NewSprite.texture;
    }

    public Texture2D LoadTexture(string FilePath)
    {
        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);                // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))              // Load the imagedata into the texture (size is set automatically)
            {              
                spriteData = Convert.ToBase64String(FileData, 0, FileData.Length);
                return Tex2D;                           // If data = readable -> return texture
            }
        }
        Debug.LogWarning("Image not loaded");
        return null;                                    // Return null if load failed
    }
}
