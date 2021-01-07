using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{

    //1. StructureManager use VideoNode gameobjects and ConnectionManager
    //to keep record of the simulation structure.
    //(VideoNodes keep track of their own action nodes)
    //2. Creates Simulation JSON-fileStructureJSON
    //3. Hold prefabs for VideoNodes and ActionNode gameobjects
    //(and special nodes: start[=actioNode], end[=VideoNode])

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
      // get ConnectionManager
      connectionManager = GetComponent<ConnectionManager>();
      if(connectionManager==null)
      {
        Debug.Log("There are no ConnectionManager as a child of " + name);
      }
    }

    public void createNewVideoNode()
    {
      //Initialize new Video node from prefab and add it to the list
      //With Unique video ID (setVideoID handles testing)

      GameObject newVideoObject = Instantiate(videoNodePrefab, GetComponent<Transform>());
      generatedVideoID++;//initialized to zero so first used will be 1
      setVideoID(newVideoObject,generatedVideoID);
      videoGameObjects.Add(newVideoObject);
    }

    public GameObject getActionNodePrefab()
    {
      //Used when creating new action inside VideoNode Class
      return actionNodePrefab;
    }


    private void setVideoID(GameObject videoGameObject, int newVideoID)
    {
      //check that the video ID is unique
      //and asign it to video node

      while(!isVideoIDFree(newVideoID))
      {
        //Debug.Log("Video ID is already in use: "+newVideoID);
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
        //Debug.Log("video ID: "+ listedVideoID);
        if (testVideoID==listedVideoID)
        {
          return false;
        }
      }

      return true;
    }

    public List<VideoNode> getVideoNodeList()
    {
      //"Converts" GameObject list to VideoNode List
      List<VideoNode> videoNodes = new List<VideoNode>();
      foreach (GameObject videoGameObject in videoGameObjects)
      {
        videoNodes.Add(videoGameObject.GetComponent<VideoNode>());
      }
      return videoNodes;
    }

    public void parseVideoStructure()
    {
      //parse video structure as JSON list
      //**REPLACE WITH JSON PARSER**

      string fileStructureJSON="";
      //if there are no structures
      if(videoGameObjects.Count<1)
      {
        //return fileStructureJSON;
        return;
      }
      fileStructureJSON += "{"+"\n";
      fileStructureJSON += addTabs(1) + "\"videos\":"+"\n";
      fileStructureJSON += addTabs(1) + "["+"\n";

      List<VideoNode> videoNodes = getVideoNodeList();
      for(int i=0; i<videoNodes.Count; i++)
      {
        fileStructureJSON += addTabs(2) +"{"+"\n";
        fileStructureJSON += addTabs(3) +"\"videoFileName\""+":\""+videoNodes[i].getVideoFileName() + "\","+"\n";
        fileStructureJSON += addTabs(3) +"\"videoID\""+":\""+videoNodes[i].getVideoID() + "\","+"\n";
        fileStructureJSON += addTabs(3) +"\"actions\":"+"\n";
        //actions
        fileStructureJSON += addTabs(3) +"["+"\n";
        fileStructureJSON += parseActionStructure(videoNodes[i]);
        fileStructureJSON += addTabs(3) +"]"+"\n";

        fileStructureJSON += addTabs(2) +"}";
        if (i<videoNodes.Count-1)
        {
            fileStructureJSON += ","; //add if not the last
        }
        fileStructureJSON += "\n";
      }
      fileStructureJSON += addTabs(1) +"]"+"\n";
      fileStructureJSON += "}"+"\n";
      Debug.Log(fileStructureJSON);
      //return fileStructureJSON;

    }

    private string parseActionStructure(VideoNode VideoNode)
    {
      //parse and returns actions as JSON list

      string actionStructure="";
      List<ActionNode> actionNodes = VideoNode.getActionNodeList();
      for(int i=0; i<actionNodes.Count; i++)
      {
        int nextVideoID=-2; //-2 used as error indicator if there is no connection for this action
        List<Connection> actionConnection = connectionManager.getConnections(actionNodes[i].getNodePort(),null);
        if (actionConnection.Count > 0)
        {
          //uses first connection if there are many
          nextVideoID=actionConnection[0].getToNode().getParentVideoNode().getVideoID();
        }
        actionStructure += addTabs(4) +"{"+"\n";
        actionStructure += addTabs(5) +"\"actionText\":\""+actionNodes[i].getActionText()+"\""+","+"\n";
        actionStructure += addTabs(5) +"\"nextVideoID\":"+nextVideoID + "\n";
        actionStructure += addTabs(4) +"}";
        if (i<actionNodes.Count-1)
        {
            actionStructure += ","; //add if not the last
        }
        actionStructure += "\n";

      }
      //Debug.Log(actionStructure);
      return actionStructure;
    }

    private string addTabs(int tabCount)
    {
      if(tabCount<1)
      {
        return "";
      }
      //string tabSize = "\t";
      string tabSize = "  ";
      string tabs ="";
      for(int i=0;i<tabCount;i++)
      {
        tabs += tabSize;
      }
      return tabs;
    }
}
