using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    [SerializeField]
    private GameObject videoNodePrefab;
    [SerializeField]
    private GameObject starNode;

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
        Debug.Log("Video ID is already in use: "+newVideoID);
        newVideoID++;
      }
      videoNode.GetComponent<VideoNode>().setVideoID(newVideoID);
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
        int listedVideoID = listedVideoNode.GetComponent<VideoNode>().getVideoID();
        Debug.Log("video ID: "+ listedVideoID);
        if (testVideoID==listedVideoID)
        {
          return false;
        }
      }

      return true;
    }

    public void parseVideoStructure()
    {
      string fileStructureJSON="";
      //if there are no structures
      if(videoNodes.Count<1)
      {
        //return fileStructureJSON;
        return;
      }
      foreach (GameObject videonode in videoNodes)
      {
        parseActionStructure(videonode);
      }

      //return fileStructureJSON;

    }

    private string parseActionStructure(GameObject VideoNode)
    {
      string actionStructure="";
      //List<GameObject> actionNodeList = VideoNode.GetComponent<VideoNode>().getActionNodeList();
      foreach (GameObject actionNode in VideoNode.GetComponent<VideoNode>().getActionNodeList())
      {
        ActionNode currentActionNode = actionNode.GetComponent<ActionNode>();
        actionStructure += "{";
        actionStructure += "\"actionText\":\""+currentActionNode.getActionText()+"\"";
        actionStructure += ", ";
        actionStructure += "\"nextVideoID\":"+currentActionNode.getNextVideoID();
        actionStructure += "}";
      }
      Debug.Log(actionStructure);
      return actionStructure;
    }
}
