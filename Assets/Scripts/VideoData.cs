using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VideoData : MonoBehaviour
{
    //variables used in editor
    [SerializeField]
    private TextAsset jsonFile;

    //private variables
    private VideoStructure videoStructure;
    // Start is called before the first frame update
    void start()
    {

    }

    //awake is called before start
    void Awake()
    {
      videoStructure = JsonUtility.FromJson<VideoStructure>(jsonFile.text);

      //debuging print video filenames
      foreach (VideoPart video in videoStructure.videos)
      {
        Debug.Log(video.getVideoFileName());
      }
      print (isVideoIDsValid(videoStructure));


    }

    // Update is called once per frame
    void Update()
    {

    }

    //public functions

    public VideoPart getVideoPart(int askedVideoID)
    {
      return videoStructure.getVideoPart(askedVideoID);
    }

    public VideoPart getStartPart(){
      return videoStructure.getStartPart();
    }

    //private functions
    private bool isVideoIDsValid(VideoStructure structure)
    {
      // no overlapping videoIDs
      int[] videoIDs = new int[structure.videos.Length];

      //construct videoIDs list and
      //check that all video IDs are positive (not negative)
      for(int i=0;i<structure.videos.Length;i++)
      {
        videoIDs[i]=structure.videos[i].getVideoID();

        if(videoIDs[i]<0)
        {
          Debug.Log("There is negative video ID used: "+structure.videos[i].getVideoFileName());
          return false;
        }
      }

      //check that every video ID is used only ones
      if (videoIDs.Count() != videoIDs.Distinct().Count())
      {
        Debug.Log("There is dublicated videoIDs");
        return false;
      }

      //check that there is video for every action
      foreach (VideoPart videoPart in structure.videos)
      {
        for (int a=0; a<videoPart.getActionCount(); a++)
        {
          Action action = videoPart.getAction(a);
          for (int i=0; i<videoIDs.Length; i++)
          {

            //ID -1 is "end action"
            if( action.getNextVideo() == -1)
            {
              break;
            }

            //video ID matches
            if( action.getNextVideo() == videoIDs[i])
            {
              break;
            }

            //No matching video ID in videoIDs list
            if (i == videoIDs.Length-1)
            {
              Debug.Log("No such video ID "+action.getNextVideo()+". Action "+action.getActionText()+" in videoPart (ID) "+videoPart.getVideoID());
              return false;
            }
          }
        }
      }
      return true;
    }
}

//Videodata structure
[System.Serializable]
public class VideoStructure
{
  public VideoPart[] videos;
  public int startId;
  public VideoPart getVideoPart(int askedVideoID)
  {
    foreach (VideoPart vPart in videos)
    {

      if (vPart.getVideoID() == askedVideoID)
      {
        return vPart;
      }
    }
    Debug.Log("There is not a videoID "+askedVideoID);
    return null;
  }

  public VideoPart getStartPart(){
    return getVideoPart(startId);
  }
}

[System.Serializable]
public class VideoPart
{
  public string videoFileName;
  public int videoID;
  public List<Action> actions;

  public string getVideoFileName()
  {
    return videoFileName;
  }

  public int getVideoID()
  {
    return videoID;
  }

  public int getActionCount()
  {
    return actions.Count;
  }

  public string getActionText(int askedActionID)
  {
    Action action = getAction(askedActionID);
    if (action!=null)
    {
      return action.getActionText();
    }
    else
    {
      return "";
    }
  }

  public int getNextVideoID(int askedActionID)
  {
    Action action = getAction(askedActionID);
    if(action!=null)
    {
      return action.getNextVideo();
    }
    else
    {
      return -2; //error
    }

  }

  public Action getAction(int askedActionID)
  {
    if (askedActionID<0)
    {
      return null;
    }
    if (askedActionID>actions.Count-1)
    {
      return null;
    }
    return actions[askedActionID];
  }
}

[System.Serializable]
public class Action
{
  public string actionText;
  public int nextVideo;

  public string getActionText()
  {
    return actionText;
  }
  public int getNextVideo()
  {
    return nextVideo;
  }
}
