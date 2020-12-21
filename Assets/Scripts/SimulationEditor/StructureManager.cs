using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    [SerializeField]
    private GameObject videoNodePrefab;
    [SerializeField]
    private GameObject actionNodePrefab;
    [SerializeField]
    private GameObject starNode;
    [SerializeField]
    private GameObject endNode;

    private List<GameObject> videoGameObjects = new List<GameObject>();
    private int generatedVideoID = 0;
    private ConnectionManager connectionManager;

    // Start is called before the first frame update
    void Start()
    {
      connectionManager = GetComponent<ConnectionManager>();
      if(connectionManager==null)
      {
        Debug.Log("There are no ConnectionManager as a child of " + name);
      }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void createNewVideoNode()
    {
      //Initialize new Video node from prefab and add it to the list
      //With Unique video ID (setVideoID handles testing)
      GameObject newVideoObject = Instantiate(videoNodePrefab, GetComponent<Transform>());
      setVideoID(newVideoObject,generatedVideoID);
      generatedVideoID++;
      videoGameObjects.Add(newVideoObject);
    }

    public GameObject getActionNodePrefab()
    {
      return actionNodePrefab;
    }


    private void setVideoID(GameObject videoGameObject, int newVideoID)
    {
      //check that the video ID is unique
      //and asign it to video node
      while(!isVideoIDFree(newVideoID))
      {
        Debug.Log("Video ID is already in use: "+newVideoID);
        newVideoID++;
      }
      videoGameObject.GetComponent<VideoNode>().setVideoID(newVideoID);
    }

    private bool isVideoIDFree(int testVideoID)
    {
      //-1 reserved for "end"
      if (testVideoID==-1)
      {
        return false;
      }

      //check through video nodes
      List<VideoNode> videoNodes = getVideoNodeList();
      foreach (VideoNode listedVideoNode in videoNodes)
      {
        int listedVideoID = listedVideoNode.getVideoID();
        Debug.Log("video ID: "+ listedVideoID);
        if (testVideoID==listedVideoID)
        {
          return false;
        }
      }

      return true;
    }

    public List<VideoNode> getVideoNodeList()
    {
      List<VideoNode> videoNodes = new List<VideoNode>();
      foreach (GameObject videoGameObject in videoGameObjects)
      {
        videoNodes.Add(videoGameObject.GetComponent<VideoNode>());
      }
      return videoNodes;
    }

    public void parseVideoStructure()
    {
      string fileStructureJSON="";
      //if there are no structures
      if(videoGameObjects.Count<1)
      {
        //return fileStructureJSON;
        return;
      }
      List<VideoNode> videoNodes = getVideoNodeList();
      foreach (VideoNode videoNode in videoNodes)
      {
        parseActionStructure(videoNode);
      }

      //return fileStructureJSON;

    }

    private string parseActionStructure(VideoNode VideoNode)
    {
      string actionStructure="";
      //List<GameObject> actionNodeList = VideoNode.GetComponent<VideoNode>().getActionNodeList();
      foreach (ActionNode actionNode in VideoNode.getActionNodeList())
      {
        int nextVideoID=-2;
        List<Connection> actionConnection = connectionManager.getConnections(actionNode.getNodePort(),null);
        if (actionConnection.Count > 0)
        {
          nextVideoID=actionConnection[0].getToNode().getParentVideoNode().getVideoID();
        }
        actionStructure += "{";
        actionStructure += "\"actionText\":\""+actionNode.getActionText()+"\"";
        actionStructure += ", ";
        actionStructure += "\"nextVideoID\":"+nextVideoID;
        actionStructure += "}";
      }
      Debug.Log(actionStructure);
      return actionStructure;
    }
}
