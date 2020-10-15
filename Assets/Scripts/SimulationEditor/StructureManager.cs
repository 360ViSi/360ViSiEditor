using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public GameObject videoNodePrefab;
    private List<GameObject> videoNodes = new List<GameObject>();
    private int generatedVideoID = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void createNewVideoNode()
    {
      GameObject newVideoObject = Instantiate(videoNodePrefab, GetComponent<Transform>());
      setVideoID(newVideoObject,generatedVideoID);
      generatedVideoID++;
      videoNodes.Add(newVideoObject);
    }

    private void setVideoID(GameObject videoNode, int newVideoID)
    {
      while(!isVideoIDFree(newVideoID))
      {
        Debug.Log("Video ID is used: "+newVideoID);
        newVideoID++;
      }
      videoNode.GetComponent<VideoNode>().setVideoID(newVideoID);
    }

    private int getVideoID(GameObject videoNode)
    {
      return videoNode.GetComponent<VideoNode>().getVideoID();
    }

    private bool isVideoIDFree(int testVideoID)
    {
      //-1 reserved for "end"
      if (testVideoID==-1)
      {
        return false;
      }

      //check through video nodes
      foreach (GameObject listedVideoNode in videoNodes)
      {
        Debug.Log("video ID: "+ getVideoID(listedVideoNode));
        if (testVideoID==getVideoID(listedVideoNode))
        {
          return false;
        }
      }

      return true;
    }
}
