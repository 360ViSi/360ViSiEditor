using System.Runtime.CompilerServices;
using System.Data;
using System.Runtime.Serialization.Json;
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
      connectionManager = GetComponentInChildren<ConnectionManager>();
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

    public void removeVideoNode(GameObject nodeObject)
    {
      videoGameObjects.Remove(nodeObject);
      var videoId = nodeObject.GetComponent<VideoNode>().getVideoID();
      //get all action nodes of all videonodes
      List<ActionNode> allActionNodes = new List<ActionNode>();
      List<VideoNode> allVideoNodes = getVideoNodeList();

      allActionNodes.Add(starNode.GetComponent<ActionNode>());
      foreach (var item in allVideoNodes)
      {
        allActionNodes.AddRange(item.getActionNodeList());
      }
      foreach (var item in allActionNodes)
      {
          if(item.getNextVideoID() == videoId)
            item.disconnect();
      }

      Destroy(nodeObject);
    }

    [ContextMenu("Parse video structure to log")]
    public void parseVideoStructure()
    {
      string fileStructureJSON = "";
      //if there are no structures
      if(videoGameObjects.Count<1)
      {
        print("Empty");
      }
      List<VideoNode> videoNodes = getVideoNodeList();
      
      var videoJSONWrapper = new VideoJSONWrapper(VideoJSONWrapper.ConvertVideoNodeListToJSONFormat(videoNodes));
      fileStructureJSON = JsonUtility.ToJson(videoJSONWrapper);
      print(fileStructureJSON);
    }

    private string parseActionStructure(VideoNode VideoNode)
    {
      string actionStructure="";
      //List<GameObject> actionNodeList = VideoNode.GetComponent<VideoNode>().getActionNodeList();
      foreach (ActionNode actionNode in VideoNode.getActionNodeList())
      {
        //ActionNode currentActionNode = actionNode.GetComponent<ActionNode>();
        actionStructure += "{";
        actionStructure += "\"actionText\":\""+actionNode.getActionText()+"\"";
        actionStructure += ", ";
        actionStructure += "\"nextVideoID\":"+actionNode.getNextVideoID();
        actionStructure += "}";
      }
      Debug.Log(actionStructure);
      return actionStructure;
    }
}
