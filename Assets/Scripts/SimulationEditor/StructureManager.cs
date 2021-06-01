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
    private GroupManager groupManager;
    Camera cam;
    [SerializeField] Transform mainNodeLayerTransform;


    // Start is called before the first frame update
    void Start()
    {
        // get ConnectionManager
        connectionManager = GetComponent<ConnectionManager>();
        groupManager = GetComponent<GroupManager>();
        if (connectionManager == null)
        {
            Debug.Log("There are no ConnectionManager as a child of " + name);
        }
        cam = Camera.main;
    }

    public void ButtonCreateNewVideoNode()
    {
        //Initialize new Video node from prefab and add it to the list
        //With Unique video ID (setVideoID handles testing)

        GameObject newVideoObject = Instantiate(videoNodePrefab, mainNodeLayerTransform);
        var newVideoNode = newVideoObject.GetComponent<VideoNode>();
        newVideoNode.SetVideoID(GetFreeNodeID());
        videoGameObjects.Add(newVideoObject);
        newVideoNode.OnSelect(false);
        UndoRedoHandler.instance.SaveState();
    }
    public VideoNode CreateNewVideoNode()
    {
        //Initialize new Video node from prefab and add it to the list
        //With Unique video ID (setVideoID handles testing)

        GameObject newVideoObject = Instantiate(videoNodePrefab, mainNodeLayerTransform);
        var newVideoNode = newVideoObject.GetComponent<VideoNode>();
        newVideoNode.SetVideoID(GetFreeNodeID());
        videoGameObjects.Add(newVideoObject);
        newVideoNode.OnSelect(false);
        UndoRedoHandler.instance.SaveState();
        return newVideoNode;
    }
    public void ButtonCreateNewToolNode()
    {
        ToolNode newToolNode = Instantiate(toolNodePrefab, mainNodeLayerTransform).GetComponent<ToolNode>();
        toolNodes.Add(newToolNode);

        var newVideoID = generatedNodeID;

        newToolNode.NodeId = GetFreeNodeID();
        newToolNode.StructureManager = this;
        UndoRedoHandler.instance.SaveState();
    }
    public ToolNode CreateNewToolNode()
    {
        ToolNode newToolNode = Instantiate(toolNodePrefab, mainNodeLayerTransform).GetComponent<ToolNode>();
        toolNodes.Add(newToolNode);

        var newVideoID = generatedNodeID;

        newToolNode.NodeId = GetFreeNodeID();
        newToolNode.StructureManager = this;
        UndoRedoHandler.instance.SaveState();
        return newToolNode;
    }

    public GameObject getActionNodePrefab()
    {
        //Used when creating new action inside VideoNode Class
        return actionNodePrefab;
    }


    public int GetFreeNodeID()
    {
        //check that the video ID is unique
        //and asign it to video node
        var newNodeId = 0;
        while (!isNodeIDFree(newNodeId))
            newNodeId++;

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
        List<VideoNode> videoNodes = GetVideoNodeList();
        foreach (VideoNode listedVideoNode in videoNodes)
        {
            int listedVideoID = listedVideoNode.GetVideoID();
            if (testVideoID == listedVideoID)
                return false;
        }
        //check through tool nodes
        foreach (var item in toolNodes)
        {
            if (testVideoID == item.NodeId)
                return false;
        }

        foreach (var item in groupManager.Groups)
        {
            if (testVideoID == item.NodeId)
                return false;
        }

        return true;
    }

    public List<VideoNode> GetVideoNodeList()
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
            return endNode.GetComponent<VideoNode>().GetNodePort();

        var video = GetVideoNodeList().Where(e => e.GetVideoID() == videoID);
        if (video.Count() > 0)
            return video.First().GetNodePort();

        var tool = toolNodes.Where(e => e.NodeId == videoID);
        if (tool.Count() > 0)
            return tool.First().InPort;

        return null;
    }

    public Node GetNode(int nodeId)
    {
        var videos = GetVideoNodeList().Where(e => e.GetVideoID() == nodeId);
        if (videos.Count() > 0)
            return videos.First();

        var tools = toolNodes.Where(e => e.NodeId == nodeId);
        if (tools.Count() > 0)
            return tools.First();

        var groups = groupManager.Groups.Where(e => e.NodeId == nodeId);
        if(groups.Count() > 0)
            return groups.First();

        Debug.LogError($"Node not found with id {nodeId}");
        return null;
    }

    public void SelectInScreenArea(Vector2 bottomLeft, Vector2 topRight)
    {
        var selectablesInArea = new List<Node>();

        var holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!holdingShift)
            NodeInspector.instance.NodeSelectionHandler.Clear();

        foreach (var item in GetVideoNodeList())
            if (IsInArea(item.WorldPosition(), bottomLeft, topRight))
                selectablesInArea.Add(item);

        foreach (var item in toolNodes)
            if (IsInArea(item.WorldPosition(), bottomLeft, topRight))
                selectablesInArea.Add(item);

        for (int i = 0; i < selectablesInArea.Count; i++)
        {
            Node item = selectablesInArea[i];
            item.DragSelect();
        }

        NodeInspector.instance.RefreshSelection();
        UndoRedoHandler.instance.SaveState();
    }

    public void MoveSelected(Vector2 delta)
    {
        foreach (var item in NodeInspector.instance.NodeSelectionHandler.SelectedNodes)
            GetNode(item).GetNodeMove().Move(delta);
    }


    bool IsInArea(Vector3 position, Vector2 bottomLeft, Vector2 topRight)
    {
        var screenPos = cam.WorldToScreenPoint(position);
        if (screenPos.x > bottomLeft.x && screenPos.x < topRight.x)
            if (screenPos.y > bottomLeft.y && screenPos.y < topRight.y)
                return true;

        return false;
    }


    public void SimulationToJson(string path)
    {
        VideoJSONWrapper wrapper = new VideoJSONWrapper(
            GetVideoNodeList(),
            toolNodes,
            startNode.GetComponent<ActionNode>().getNodePort().getNextVideoID(),
            startNode.GetComponent<RectTransform>().anchoredPosition,
            endNode.GetComponent<RectTransform>().anchoredPosition
        );
        var json = JsonUtility.ToJson(wrapper);

        Debug.Log($"Saving to path: {path}\nContent: {json}");

        if (File.Exists(path))
            File.Delete(path);

        File.WriteAllText(path, json);
    }

    public VideoJSONWrapper CreateSaveState()
    {
        return new VideoJSONWrapper(

                GetVideoNodeList(),
                toolNodes,
                startNode.GetComponent<ActionNode>().getNodePort().getNextVideoID(),
                startNode.GetComponent<RectTransform>().anchoredPosition,
                endNode.GetComponent<RectTransform>().anchoredPosition
            );
    }

    public void LoadSaveState(VideoJSONWrapper wrapper)
    {
        ClearStructure();

        //Create nodes
        foreach (var item in wrapper.videos)
            LoadVideoNode(item);

        if (wrapper.tools != null)
            foreach (var item in wrapper.tools)
            {
                if (item == null)
                {
                    Debug.Log("tool == null");
                    continue;
                }
                LoadToolNode(item);
            }

        //Create connections
        foreach (var item in GetVideoNodeList())
            foreach (var action in item.GetActionNodeList())
            {
                action.CreateLoadedConnection();
                action.setMode();
            }
        for (int i = 0; i < toolNodes.Count; i++)
            for (int j = 0; j < toolNodes[i].OutPorts.Count; j++)
                toolNodes[i].OutPorts[j].CreateConnection(toolNodes[i].NextVideos[j]);

        connectionManager.redrawConnection(null, null);

        var startNodePort = startNode.GetComponent<ActionNode>().getNodePort();

        if (wrapper.startId == -2)
        {
            Debug.LogWarning("No video connected to the start");
            return;
        }

        var firstVideoPort = GetVideoInNodePortWithId(wrapper.startId);
        connectionManager.createConnection(startNodePort, firstVideoPort);
        connectionManager.showConnectionLine(startNodePort, firstVideoPort, true);
        startNode.GetComponent<RectTransform>().anchoredPosition = wrapper.startNodePosition;
        endNode.GetComponent<RectTransform>().anchoredPosition = wrapper.endNodePosition;
        StartCoroutine(ReDrawLinesAfterFrame());
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
        foreach (var item in GetVideoNodeList())
            foreach (var action in item.GetActionNodeList())
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
        startNode.GetComponent<RectTransform>().anchoredPosition = wrapper.startNodePosition;
        endNode.GetComponent<RectTransform>().anchoredPosition = wrapper.endNodePosition;
        StartCoroutine(ReDrawLinesAfterFrame());
        UndoRedoHandler.instance.SaveState();
    }

    IEnumerator ReDrawLinesAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        connectionManager.redrawConnection(null, null);
    }

    private void LoadToolNode(VideoJSONWrapper.ToolJSONObject item)
    {
        var newToolObject = Instantiate(toolNodePrefab, mainNodeLayerTransform);
        var node = newToolObject.GetComponent<ToolNode>();
        node.NodeId = item.nodeId;
        node.NextVideos = item.nextNodes.ToList();
        node.NodeName.text = "Randomizer";
        node.ToolType = (ToolType)item.toolTypeInt;
        node.Question = item.question;
        node.InfoText = item.infoText;
        node.StructureManager = this;

        for (int i = 2; i < node.NextVideos.Count; i++)
            node.CreateOutPort(true);

        newToolObject.GetComponent<RectTransform>().anchoredPosition = item.nodePosition;
        toolNodes.Add(node);
    }

    public void LoadVideoNode(VideoJSONWrapper.VideoJSONObject videoJSONObject)
    {
        var newVideoObject = Instantiate(videoNodePrefab, mainNodeLayerTransform);
        var node = newVideoObject.GetComponent<VideoNode>();
        node.SetVideoID(videoJSONObject.nodeId);
        node.SetVideoFileName(videoJSONObject.videoFileName);
        node.SetLoop(videoJSONObject.loop);
        node.SetLoopTime(videoJSONObject.loopTime);
        node.SetStartTime(videoJSONObject.startTime);
        node.SetEndTime(videoJSONObject.endTime);
        node.SetVideoStartRotation(videoJSONObject.videoStartRotation);
        newVideoObject.GetComponent<RectTransform>().anchoredPosition = videoJSONObject.nodePosition;
        videoGameObjects.Add(newVideoObject);

        foreach (var item in videoJSONObject.actions)
            node.CreateNewActionNode(item.actionText,
                                     item.autoEnd,
                                     item.nextNode,
                                     item.startTime,
                                     item.endTime,
                                     item.actionType,
                                     item.worldPosition,
                                     item.iconName,
                                     item.areaMarkerVertices,
                                     item.timer);
    }

    public void ClearStructure()
    {
        for (int i = videoGameObjects.Count - 1; i > -1; i--)
            videoGameObjects[i].GetComponent<VideoNode>().DeleteNode(true);
        for (int i = toolNodes.Count - 1; i >= 0; i--)
            toolNodes[i].RemoveToolNode(true);

        NodeInspector.instance.NullCurrentNodes();
    }

    public void removeVideoNode(GameObject nodeObject)
    {
        videoGameObjects.Remove(nodeObject);
    }

    internal void RemoveToolNode(ToolNode node)
    {
        toolNodes.Remove(node);
    }
}
