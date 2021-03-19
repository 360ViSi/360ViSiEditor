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

    public VideoPart getCurrentVideoPart()
    {
      //Debug.Log("GetCurrentVideoPart " + currentVideoID);
      return currentVideoPart;
    }

    public void GoToNode(int nextNodeID)
    {
      Debug.Log(nextNodeID);

      if (nextNodeID == -1)
          EndGame();

      var videoPart = videoData.getVideoPart(nextNodeID);
      //No video or tool with that id is found -> EndGame
      if (videoPart != null)
      {
          GoToVideo(nextNodeID, videoPart);
          return;
      }

      if (videoPart == null && videoData.VideoStructure.tools == null)
      {
          Debug.LogError("no tools");
          EndGame();
          return;
      }

      foreach (var item in videoData.VideoStructure.tools)
          if (item.nodeId == nextNodeID)
              item.ProcessTool(GoToNode);
    }

    private void GoToVideo(int nextNodeID, VideoPart videoPart)
    {
        currentVideoPart = videoPart;
        string nextVideoFileName = videoPart.getVideoFileName();

        if (string.IsNullOrEmpty(nextVideoFileName))
        {
            Debug.Log("Something goes wrong");
            return;
        }

        if (nextVideoFileName == "None")
        {
            currentVideoID = nextNodeID;
            Debug.LogError("This part is without video file.");
            return;
        }

        currentVideoID = nextNodeID;

        videoTextureChanger.ChangeVideo(videoData.getFolderPath() + nextVideoFileName);
        buttonHandler.SetupActions();
        Debug.Log("Video is " + nextVideoFileName);
    }

    public void AutoEnd()
    {
      Debug.Log("AutoEnd");
      var videoPart = getCurrentVideoPart();

      var autoEnds = videoPart.actions.Where(e => e.autoEnd);

      Debug.Log("autoEnds.Count" + autoEnds.Count());

      if(autoEnds.Count() > 0)
        GoToNode(autoEnds.FirstOrDefault().nextVideo);
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
