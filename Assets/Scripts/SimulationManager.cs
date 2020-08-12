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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void actionSelected(int buttonID)
    {
      Debug.Log("button clicked " + buttonID);
      getToNextPart(buttonID);
    }

    public VideoPart getCurrentVideoPart()
    {
      return videoData.getVideoPart(currentVideoID);
    }

    public void getToNextPart(int actionID)
    {
      VideoPart currentPart = videoData.getVideoPart(currentVideoID);
      Action action = currentPart.getAction(actionID);
      int nextVideoID = action.getNextVideo();

      if (nextVideoID == -1)
      {
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
        Debug.Log("This part is without video file.");
      }

      currentVideoID=nextVideoID;
      Debug.Log("next video is "+nextVideoFileName);
    }

}
