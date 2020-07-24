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
    private VideoFilePointer_OS videoPointer;

    //private variables
    private VideoPlayer videoPlayer360;
    private int currentClip = 0;
    private List<string> videoURLs = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
      //initialize Video player
      videoPlayer360 = gameObject.GetComponent<VideoPlayer>();
      videoPlayer360.prepareCompleted += PrepareCompleted;

      //videos
      videoURLs = getVideoURLs(videoPointer);

    }

    void PrepareCompleted(VideoPlayer videoPlayer360)
    {
      Debug.Log("video is ready");
      videoPlayer360.Play();
    }

    public void ChangeVideo()
    {
      currentClip = (currentClip+1)%2;
      videoPlayer360.url = videoURLs[currentClip];
      videoPlayer360.Prepare();
      Debug.Log("Clip: "+ currentClip);
    }

    private string getVideoFolder(VideoFilePointer_OS pointer)
    {
      // Get absolute path to Application
      // and relative path to videoPointer.
      // Both will have same "Assets" folder which need to be removed.
      // Also videoPointer filename need to removed to get pure folder path.
      // Pathseparator is used to make the code operationsystem independent
      string pathToVideo = Application.dataPath;
      char pathSeparator = Path.DirectorySeparatorChar;
      string[] relativePathFolders = AssetDatabase.GetAssetPath(videoPointer).Split(pathSeparator);
      for (int i=1; i<relativePathFolders.Length-1;i++)
      {
        pathToVideo += pathSeparator+relativePathFolders[i];
      }
      pathToVideo += pathSeparator;
      return (pathToVideo);
    }

    private List<string> getVideoURLs(VideoFilePointer_OS pointer)
    {
      string directory = getVideoFolder(pointer);

      List<string> URLs = new List<string>();
      DirectoryInfo dirInfo = new DirectoryInfo(directory);
      foreach (var file in dirInfo.GetFiles())
      {
        string fileName=file.Name;
        foreach (var extension in pointer.fileExtensions)
        {
          string nameTail = fileName.Substring(fileName.Length-extension.Length,extension.Length);
          if(nameTail==extension)
          {
            URLs.Add(directory+file.Name);
          }
        }
      }
      return URLs;

    }
}
