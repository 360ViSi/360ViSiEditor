using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEditor;

public class VideoTextureChanger : MonoBehaviour
{
    //variables used in editor
    [SerializeField]
    private VideoPathPointer_OS videoPointer;

    //private variables
    [SerializeField] private VideoPlayer videoPlayer360;

    // Start is called before the first frame update
    void Start()
    {
      //initialize Video player
      //videoPlayer360 = gameObject.GetComponent<VideoPlayer>();
      videoPlayer360.prepareCompleted += PrepareCompleted;

    }

    void PrepareCompleted(VideoPlayer videoPlayer360)
    {
      Debug.Log("video is ready");
      videoPlayer360.Play();
    }

    public void changeVideo(string videoFileName)
    {
      videoPlayer360.url = getVideoURL(videoPointer, videoFileName);
      videoPlayer360.Prepare();
    }

    private string getVideoURL(VideoPathPointer_OS pointer, string videoFileName)
    {
      string directory = @"C:\Unity\"; //getVideoFolder(pointer);
      DirectoryInfo dirInfo = new DirectoryInfo(directory);
      string videoURL = "";

      //read every file name in folder
      foreach (var file in dirInfo.GetFiles())
      {
        string fileName = file.Name;
        if (videoFileName != fileName)
        {
          continue;
        }
        //compare to VideoPathPointer_OS extensions
        foreach (var extension in pointer.fileExtensions)
        {
          string nameTail = fileName.Substring(fileName.Length-extension.Length,extension.Length);

          //if extension matches add URL to list
          if(nameTail == extension)
          {
            videoURL = directory + fileName;
            break;
          }
        }
      }
      if (videoURL=="")
      {
        Debug.Log("Video "+videoFileName+" is not in "+directory+" folder");
      }
      return videoURL;
    }

    private string getVideoFolder(VideoPathPointer_OS pointer)
    {
      // Get absolute path to Application
      // and relative path to videoPointer.
      // Both will have same "Assets" folder which need to be removed.
      // Also videoPointer filename need to be removed to get pure folder path.
      // Path separator is used to make the code operationsystem independent
      string pathToVideo = Application.dataPath;
      char pathSeparator = Path.DirectorySeparatorChar;
      string[] relativePathFolders = AssetDatabase.GetAssetPath(videoPointer).Split(pathSeparator);
      //drop first and last part (i=1 and Length-1)
      for (int i = 1; i < relativePathFolders.Length - 1 ; i++)
      {
        pathToVideo += pathSeparator+relativePathFolders[i];
      }
      pathToVideo += pathSeparator;
      return pathToVideo;
    }
}
