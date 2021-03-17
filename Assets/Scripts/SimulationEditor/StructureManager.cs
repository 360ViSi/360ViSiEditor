using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class StructureManager : MonoBehaviour
{
    //1. StructureManager use VideoNode gameobjects and ConnectionManager
    //to keep record of the simulation structure.
    //(VideoNodes keep track of their own action nodes)
    //2. Creates Simulation JSON-fileStructureJSON
    //3. Hold prefabs for VideoNodes and ActionNode gameobjects
    //(and special nodes: start[=actionNode], end[=VideoNode])

    [SerializeField]
    private GameObject videoNodePrefab;
    [SerializeField]
    private GameObject actionNodePrefab;
    [SerializeField] GameObject toolNodePrefab;
    [SerializeField]
    private GameObject startNode;
    [SerializeField]
    private GameObject endNode;
    private List<GameObject> videoGameObjects = new List<GameObject>();
    private List<ToolNode> toolNodes = new List<ToolNode>();
    private int generatedNodeID = 0;
    private ConnectionManager connectionManager;
    

    // Start is called before the first frame update
    void Start()
    {
        // get ConnectionManager
        connectionManager = GetComponent<ConnectionManager>();
        if (connectionManager == null)
        {
            Debug.Log("There are no ConnectionManager as a child of " + name);
        }
    }
    
    public void createNewVideoNode()
    {
        //Initialize new Video node from prefab and add it to the list
        //With Unique video ID (setVideoID handles testing)

        GameObject newVideoObject = Instantiate(videoNodePrefab, transform);
        newVideoObject.GetComponent<VideoNode>().setVideoID(getFreeNodeID());
        videoGameObjects.Add(newVideoObject);
        newVideoObject.GetComponent<VideoNode>().InspectorOpen();
    }

    public void CreateNewToolNode()
    {
        ToolNode newToolNode = Instantiate(toolNodePrefab, transform).GetComponent<ToolNode>();
        toolNodes.Add(newToolNode);

        var newVideoID = generatedNodeID;

        newToolNode.NodeId = getFreeNodeID();
    }

    public GameObject getActionNodePrefab()
    {
        //Used when creating new action inside VideoNode Class
        return actionNodePrefab;
    }


    private int getFreeNodeID()
    {
        //check that the video ID is unique
        //and asign it to video node
        var newNodeId = 0;
        while (!isNodeIDFree(newNodeId))
        {
            Debug.Log("Node ID is already in use: " + newNodeId);
            newNodeId++;
        }
        return newNodeId;
    }

    private bool isNodeIDFree(int testVideoID)
    {
        //-1 reserved for "end"
        if (testVideoID == -1)
        {
            return false;
        }

        //check through video nodes
        List<VideoNode> videoNodes = getVideoNodeList();
        foreach (VideoNode listedVideoNode in videoNodes)
        {
            int listedVideoID = listedVideoNode.getVideoID();
            if (testVideoID == listedVideoID)
            {
                return false;
            }
        }
        //check through tool nodes
        foreach (var item in toolNodes)
        {
            if (testVideoID == item.NodeId)
                return false;
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
    public NodePort GetVideoInNodePortWithId(int videoID)
    {
        if (videoID == -1)
            return endNode.GetComponent<VideoNode>().getNodePort();

        var video = getVideoNodeList().Where(e => e.getVideoID() == videoID);
        if (video.Count() > 0)
            return video.First().getNodePort();

        var tool = toolNodes.Where(e => e.NodeId == videoID);
        if (tool.Count() > 0)
            return tool.First().InPort;

        return null;
    }

    public void SimulationToJson(string path)
    {
        VideoJSONWrapper wrapper = new VideoJSONWrapper(
            getVideoNodeList(),
            toolNodes,
            startNode.GetComponent<ActionNode>().getNodePort().getNextVideoID()
        );
        var json = JsonUtility.ToJson(wrapper);

        Debug.Log($"Saving to path: {path}\nContent: {json}");

        if (File.Exists(path))
            File.Delete(path);

        File.WriteAllText(path, json);
    }

    public void JsonToSimulation()
    {
        //S TODO Filebrowser
        if (File.Exists(ProjectManager.instance.FullPath) == false)
        {
            Debug.LogError($"File not found at: {ProjectManager.instance.FullPath}");
            return;
        }
        var fileText = File.ReadAllText(ProjectManager.instance.FullPath);
        var wrapper = JsonUtility.FromJson<VideoJSONWrapper>(fileText);

        //To ease the loading process and make sure all the id's match up etc.
        //clearing the whole structure while loading new one.
        //could possibly generate a method to just apped loaded structure, but
        //not sure if that is realistic use case at this point
        ClearStructure();

        //Create nodes
        foreach (var item in wrapper.videos)
            LoadVideoNode(item);

        foreach (var item in wrapper.tools)
            LoadToolNode(item);

        //Create connections
        foreach (var item in getVideoNodeList())
            foreach (var action in item.getActionNodeList())
            {
                action.CreateLoadedConnection();
                action.setMode();
            }
        for (int i = 0; i < toolNodes.Count; i++)
            for (int j = 0; j < toolNodes[i].OutPorts.Count; j++)
                toolNodes[i].OutPorts[j].CreateConnection(toolNodes[i].NextVideos[j]);

        //connectionManager.redrawConnection(null, null);

        var startNodePort = startNode.GetComponent<ActionNode>().getNodePort();

        if (wrapper.startId == -2)
        {
            Debug.LogWarning("No video connected to the start");
            return;
        }

        var firstVideoPort = GetVideoInNodePortWithId(wrapper.startId);
        connectionManager.createConnection(startNodePort, firstVideoPort);
        connectionManager.showConnectionLine(startNodePort, firstVideoPort, true);
        StartCoroutine(ReDrawLinesAfterFrame());
    }

    IEnumerator ReDrawLinesAfterFrame(){
        yield return new WaitForEndOfFrame();
        connectionManager.redrawConnection(null, null);
    }

    private void LoadToolNode(VideoJSONWrapper.ToolJSONObject item)
    {
        var newToolObject = Instantiate(toolNodePrefab, transform);
        var node = newToolObject.GetComponent<ToolNode>();
        node.NodeId = item.nodeId;
        node.NextVideos = item.nextVideos.ToList();
        node.NodeName.text = "Randomizer";
        
         for (int i = 2; i < node.NextVideos.Count; i++)
             node.CreateOutPort(true);

        newToolObject.GetComponent<RectTransform>().anchoredPosition = item.position;
        toolNodes.Add(node);
    }

    public void LoadVideoNode(VideoJSONWrapper.VideoJSONObject videoJSONObject)
    {
        var newVideoObject = Instantiate(videoNodePrefab, transform);
        //generatedVideoID++;//initialized to zero so first used will be 1
        //setVideoID(newVideoObject, generatedVideoID);
        var node = newVideoObject.GetComponent<VideoNode>();
        node.setVideoID(videoJSONObject.videoID);
        node.setVideoFileName(videoJSONObject.videoFileName);
        node.setLoop(videoJSONObject.loop);
        node.setLoopTime(videoJSONObject.loopTime);
        node.setStartTime(videoJSONObject.startTime);
        node.setEndTime(videoJSONObject.endTime);
        newVideoObject.GetComponent<RectTransform>().anchoredPosition = videoJSONObject.position;
        videoGameObjects.Add(newVideoObject);

        foreach (var item in videoJSONObject.actions)
            node.CreateNewActionNode(item.actionText,
                                     item.autoEnd,
                                     item.nextVideo,
                                     item.startTime,
                                     item.endTime,
                                     item.actionType,
                                     item.worldPosition,
                                     item.iconName,
                                     item.areaMarkerVertices,
                                     item.interactable);
    }

    public void ClearStructure()
    {
        for (int i = videoGameObjects.Count - 1; i > -1; i--)
            videoGameObjects[i].GetComponent<VideoNode>().deleteNode();

        NodeInspector.instance.NullCurrentNodes();
    }

    public void removeVideoNode(GameObject nodeObject)
    {
        videoGameObjects.Remove(nodeObject);
    }
}
