using System.Text;
using System;
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
      newVideoObject.GetComponent<VideoNode>().SetStructureManager(this);
      generatedVideoID++;
      videoNodes.Add(newVideoObject);
    }

    public void RemoveVideoNode(GameObject nodeObject)
    {
      videoNodes.Remove(nodeObject);
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

    [ContextMenu("Parse Video Structure to Log")]
    public string parseVideoStructure()
    {
      //if there are no structures
      if(videoNodes.Count<1)
      {
        //return fileStructureJSON;
        return "null";
      }

      StringBuilder sb = new StringBuilder();
      sb.Append("{");
      sb.Append("\"videos\":[");

      var firstVideo = true;
      foreach (GameObject videoNodeObject in videoNodes)
      {
        var videoNode = videoNodeObject.GetComponent<VideoNode>();
        if(firstVideo) firstVideo = false;
        else sb.Append(",");
        sb.Append("{");
        sb.Append("\"videoID\":\"" + videoNode.getVideoID() + "\", ");
        sb.Append("\"videoFileName\":\"" + videoNode.getVideoFileName() + "\", ");
        sb.Append("\"actions\": [");
        sb.Append(parseActionStructure(videoNodeObject));
        sb.Append("]");
        sb.Append("}");

      }
      sb.Append("]");
      sb.Append("}");

      Debug.Log(sb.ToString());
      return sb.ToString();
    }

    private string parseActionStructure(GameObject VideoNode)
    {
      string actionStructure="";
      var firstAction = true;
      //List<GameObject> actionNodeList = VideoNode.GetComponent<VideoNode>().getActionNodeList();
      foreach (GameObject actionNode in VideoNode.GetComponent<VideoNode>().getActionNodeList())
      {
        ActionNode currentActionNode = actionNode.GetComponent<ActionNode>();
        if (firstAction) firstAction = false;
        else actionStructure += ",";
        actionStructure += "{";
        actionStructure += "\"actionText\":\""+currentActionNode.getActionText()+"\"";
        actionStructure += ", ";
        actionStructure += "\"nextVideoID\":"+currentActionNode.getNextVideoID();
        actionStructure += "}";
      }
      //Debug.Log(actionStructure);
      return actionStructure;
    }
}
