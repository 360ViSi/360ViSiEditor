using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class SimulationManager : MonoBehaviour
{
    //External objects
    [SerializeField]
    VideoData videoData;
    [SerializeField]
    VideoTextureChanger videoTextureChanger;
    [SerializeField] GameObject endPanel; 
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] ButtonHandler buttonHandler;
    //private parameters
    private int currentVideoID=-2;
    private VideoPart currentVideoPart;

    // Start is called before the first frame update
    void Start()
    {
      setStartVideo(); //changed from invoke sith 2s delay
    }

    // Update is called once per frame
    void Update()
    {
      //getCurrentVideoPart();
    }

    void setStartVideo()
    {
      string startVideoFileName = videoData.getStartPart().getVideoFileName();
      currentVideoPart = videoData.getStartPart();
      currentVideoID = currentVideoPart.videoID;
      videoTextureChanger.ChangeVideo(videoData.getFolderPath() + startVideoFileName);
      buttonHandler.SetupActions();
      videoPlayer.Play();
    }

    public void actionSelected(int actionID)
    {
      //Debug.Log("button clicked " + actionID);
      goToNextPart(actionID);
    }

    public VideoPart getCurrentVideoPart()
    {
      //Debug.Log("GetCurrentVideoPart " + currentVideoID);
      return currentVideoPart;
    }

    public void goToNextPart(int actionID)
    {
      int nextVideoID = videoData.getVideoPart(currentVideoID).getNextVideoID(actionID);
      goToVideo(nextVideoID);
    }

    public void goToVideo(int nextVideoID)
    {
      Debug.Log(nextVideoID);

      if (nextVideoID == -1)
        EndGame();

      var videoPart = videoData.getVideoPart(nextVideoID);
      //No video or tool with that id is found -> EndGame
      if(videoPart == null)
      {
        EndGame();
        return;
      }
      currentVideoPart = videoPart;
      string nextVideoFileName = videoPart.getVideoFileName();

      if (string.IsNullOrEmpty(nextVideoFileName))
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
      
      videoTextureChanger.ChangeVideo(videoData.getFolderPath() + nextVideoFileName);
      buttonHandler.SetupActions();
      Debug.Log("Video is "+nextVideoFileName);
    }

    public void AutoEnd()
    {
      Debug.Log("AutoEnd");
      var videoPart = getCurrentVideoPart();

      var autoEnds = videoPart.actions.Where(e => e.autoEnd);

      Debug.Log("autoEnds.Count" + autoEnds.Count());

      if(autoEnds.Count() > 0)
        goToVideo(autoEnds.FirstOrDefault().nextVideo);
    }

    public void ResetSimulation()
    {
      endPanel.SetActive(false);
      setStartVideo();    
    }

    public void EndGame()
    {
      endPanel.SetActive(true);
      videoPlayer.Stop();
    }
}
