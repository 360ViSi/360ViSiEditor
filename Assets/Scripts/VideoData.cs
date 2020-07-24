using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VideoData : MonoBehaviour
{
    public TextAsset jsonFile;
    private VideoStructure videoStructure;
    // Start is called before the first frame update
    void Start()
    {
      print ("tuli");
      videoStructure = JsonUtility.FromJson<VideoStructure>(jsonFile.text);

      foreach (VideoPart video in videoStructure.videos)
      {
        Debug.Log(video.videoFileName);
      }
      print (checkActionRoutes(videoStructure));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool checkActionRoutes(VideoStructure structure)
    {
      // no overlapping videoIDs
      int[] videoIDs = new int[structure.videos.Length];
      for(int i=0;i<structure.videos.Length;i++)
      {
        videoIDs[i]=structure.videos[i].videoID;
        if(videoIDs[i]<0)
        {
          Debug.Log("There is negative video ID used: "+structure.videos[i].videoFileName);
          return false;
        }
      }
      if (videoIDs.Count() != videoIDs.Distinct().Count())
      {
        Debug.Log("There is dublicated videoIDs");
        return false;
      }
      foreach (VideoPart videoPart in structure.videos)
      {
        foreach (Action action in videoPart.actions)
        {
          for (int i=0; i<videoIDs.Length; i++)
          {
            if( action.nextVideo == -1)
            {
              break;
            }

            if( action.nextVideo == videoIDs[i])
            {
              break;
            }

            if (i == videoIDs.Length-1)
            {
              Debug.Log("No such video ID "+action.nextVideo+". Action "+action.actionText+" in videoPart (ID) "+videoPart.videoID);
              return false;
            }
          }
        }
      }
      return true;
    }
}

[System.Serializable]
public class VideoStructure
{
  public VideoPart[] videos;
}

[System.Serializable]
public class VideoPart
{
  public string videoFileName;
  public int videoID;
  public List<Action> actions;
}
[System.Serializable]
public class Action
{
  public string actionText;
  public int nextVideo;
}
