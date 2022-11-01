using System.Collections;
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
    private string spritePath;
    public string SpritePath { get => spritePath; set => spritePath = value; }
    private string FullPath { get => folderPath + fileName; set => spritePath = value; }

    [SerializeField] private RawImage rawImageInstance;
    public RawImage RawImageInstance { get => rawImageInstance; set => rawImageInstance = value; }
    public string FileName { get => fileName; set => fileName = value; }

    private void Awake()
    {
        RawImageInstance = GetComponent<RawImage>();
    }

    //Opens a file browser that filters files according to extension. Also sets the spritePath -variable
    public void OpenImageFile(string path)
    {
        var extensions = new[] {
        new ExtensionFilter("Image Files", "tga","tif","psd","bitmap","png","jpeg" )
        };

        if (path == "")
            path = StandaloneFileBrowser.OpenFilePanel("Load a new image file", folderPath, extensions, false)[0];

        folderPath = Utilities.FolderPathFromFilePath(path);
        fileName = Utilities.FileNameFromFilePath(path, true);
        spritePath = FullPath;

        ImageToSimulation();
    }

    //Sets the image texture from sprite
    public void ImageToSimulation()
    {
        if (File.Exists(FullPath) == false)
        {
            Debug.LogError($"File not found at: {FullPath}");
            return;
        }
        RawImageInstance.texture = LoadTexture(FullPath);

    }
    // Loads a PNG or JPG image from disk to a Texture2D and returns its reference
    public Texture LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {                
        Texture2D SpriteTexture = LoadTexture(FilePath);
        
        return SpriteTexture;
    }

    //Loads a sprite with spritePath -variable
    public void SetOldLoadedSprite(string filePath)
    {
        if(String.IsNullOrEmpty(filePath)) return;

        spritePath = filePath;
        Texture2D texture = LoadTexture(filePath);

        RawImageInstance.texture = texture;
        // Vector2 textureSize = new Vector2(texture.width, texture.height);
        if(gameObject.name == "RawImageEditor") return;
        RawImageInstance.SetNativeSize();
        //rawImageInstance.transform.localScale *= .5f;
        if (texture!=null)
        {
            RawImageInstance.texture = texture;
            Vector2 textureSize = new Vector2(texture.width, texture.height);
            RawImageInstance.SetNativeSize();
            //rawImageInstance.transform.localScale *= .5f;
        }
    }

    // Load a PNG or JPG file from disk to a Texture2D
    // Returns null if load fails
    public Texture2D LoadTexture(string FilePath)
    {

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);                            // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))                          // Load the imagedata into the texture (size is set automatically)
            {              
                return Tex2D;                                       // If data = readable -> return texture
            }
            else
            {
                Debug.LogWarning("Texture load from file failed");
            }
        }
        Debug.LogWarning("Image not loaded. File \""+FilePath+"\" not found");
        return null;                                                // Return null if load failed
    }

    public void Reset()
    {
        spritePath = "";
        rawImageInstance.texture = null;
    }
}
