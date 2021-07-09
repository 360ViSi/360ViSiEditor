using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System.IO;

public class ImageSetter : MonoBehaviour
{
    private string folderPath;
    private string fileName;
    private string fullPath { get => folderPath + fileName; }

    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
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

        image.sprite = LoadNewSprite(fullPath);

    }
    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
                
        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

        return NewSprite;
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
                return Tex2D;                           // If data = readable -> return texture
        }
        Debug.LogWarning("Image not loaded");
        return null;                                    // Return null if load failed
    }
}
