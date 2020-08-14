using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    //External objects
    [SerializeField]
    VideoData videoData;
    [SerializeField]
    VideoTextureChanger videoTextureChanger;

    //private parameters
    private int currentVideoID=0;

    // Start is called before the first frame update
    void Start()
    {
      Invoke("setStartVideo",2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void setStartVideo()
    {
      string startVideoFileName = videoData.getVideoPart(currentVideoID).getVideoFileName();
      videoTextureChanger.changeVideo(startVideoFileName);
    }

    public void actionSelected(int actionID)
    {
      //Debug.Log("button clicked " + actionID);
      getToNextPart(actionID);
    }

    public VideoPart getCurrentVideoPart()
    {
      return videoData.getVideoPart(currentVideoID);
    }

    public void getToNextPart(int actionID)
    {
      int nextVideoID = videoData.getVideoPart(currentVideoID).getNextVideoID(actionID);

      if (nextVideoID == -1)
      {
        currentVideoID=nextVideoID;
        Debug.Log("Game ended");
        return;
      }

      string nextVideoFileName = videoData.getVideoPart(nextVideoID).getVideoFileName();

      if (nextVideoFileName=="")
      {
        Debug.Log("Something goes wrong");
        return;
      }

      if (nextVideoFileName=="None")
      {
        currentVideoID=nextVideoID;
        Debug.Log("This part is without video file.");
        return;
      }

      currentVideoID=nextVideoID;
      videoTextureChanger.changeVideo(nextVideoFileName);
      Debug.Log("Video is "+nextVideoFileName);
    }

}
