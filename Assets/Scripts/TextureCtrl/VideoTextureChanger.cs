using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class VideoTextureChanger : MonoBehaviour
{

    //public VideoClip [] videos;
    public string pathToVideoFolder ="/Art/Materials/360VideoClips/";
    private VideoPlayer videoPlayer360;
    private int currentCLip = 0;
    private string pathToRoot;
    private List<string> videoURLs = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
      videoPlayer360 = gameObject.GetComponent<VideoPlayer>();
      videoPlayer360.prepareCompleted += PrepareCompleted;
      //pathToRoot = Application.streamingAssetsPath;
      pathToRoot = Application.dataPath;
      getFileNames(pathToRoot +pathToVideoFolder);

      Debug.Log("Videoclip name: "+ videoPlayer360.clip.name );
      Debug.Log("Videoclip path: "+ videoPlayer360.clip.originalPath );
      Debug.Log("Application path: "+pathToRoot);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PrepareCompleted(VideoPlayer videoPlayer360)
    {
      Debug.Log("video is ready");
      videoPlayer360.Play();
    }

    public void ChangeVideo()
    {
      currentCLip = (currentCLip+1)%2;
      //videoPlayer360.clip = video[currentCLip];
      videoPlayer360.url = videoURLs[currentCLip];
      videoPlayer360.Prepare();
      Debug.Log("Clip: "+ currentCLip);
    }

    private void getFileNames(string directory)
    {
      DirectoryInfo dirInfo = new DirectoryInfo(directory);
      foreach (var file in dirInfo.GetFiles())
      {
        string fileName=file.Name;
        string last4 = fileName.Substring(fileName.Length-4,4);
        if(last4=="webm")
        {
          videoURLs.Add(directory+file.Name);
        }

      }
    }
}
