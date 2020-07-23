using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoTextureChanger : MonoBehaviour
{
    public VideoClip [] videos;
    private VideoPlayer videoPlayer360;
    private int currentCLip = 0;
    // Start is called before the first frame update
    void Start()
    {
      videoPlayer360 = gameObject.GetComponent<VideoPlayer>();
      videoPlayer360.prepareCompleted += PrepareCompleted;


      Debug.Log( videoPlayer360.clip.name );
      Debug.Log( videoPlayer360.clip.originalPath );
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
      videoPlayer360.clip = videos[currentCLip];
      videoPlayer360.Prepare();
      Debug.Log("Clip: "+ currentCLip);
    }
}
