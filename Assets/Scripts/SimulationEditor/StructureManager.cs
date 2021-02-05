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
    [SerializeField]
    private GameObject startNode;
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
        if (connectionManager == null)
        {
            Debug.Log("There are no ConnectionManager as a child of " + name);
        }

        //S TODO remove when startup loadin is not great anymore
        JsonToSimulation();
    }
    public void createNewVideoNode()
    {
        //Initialize new Video node from prefab and add it to the list
        //With Unique video ID (setVideoID handles testing)

        GameObject newVideoObject = Instantiate(videoNodePrefab, transform);
        generatedVideoID++;//initialized to zero so first used will be 1
        setVideoID(newVideoObject, generatedVideoID);
        videoGameObjects.Add(newVideoObject);
        newVideoObject.GetComponent<VideoNode>().InspectorOpen();
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

        while (!isVideoIDFree(newVideoID))
        {
            Debug.Log("Video ID is already in use: " + newVideoID);
            newVideoID++;
        }
        videoGameObject.GetComponent<VideoNode>().setVideoID(newVideoID);
    }

    private bool isVideoIDFree(int testVideoID)
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
            //Debug.Log("video ID: "+ listedVideoID);
            if (testVideoID == listedVideoID)
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
    public VideoNode GetVideoNodeWithId(int videoID)
    {
        if(videoID == -1)
            return endNode.GetComponent<VideoNode>();
            
        return getVideoNodeList().Where(e => e.getVideoID() == videoID).First();
    }

    public void SimulationToJson()
    {
        //S TODO: replace all lines with @"C:\Unity\ with an actual folder that the user can set,
        //this is just used so it's faster to develop
        VideoJSONWrapper wrapper = new VideoJSONWrapper(
            getVideoNodeList(),
            startNode.GetComponent<ActionNode>().getNodePort().getNextVideoID()
        ); //S TODO get the id of the video that start goes to
        var json = JsonUtility.ToJson(wrapper);

        Debug.Log(json.ToString());

        if (File.Exists(@"C:\Unity\simu.json"))
            File.Delete(@"C:\Unity\simu.json");

        File.WriteAllText(@"C:\Unity\simu.json", json);
    }

    public void JsonToSimulation()
    {
        //S TODO Filebrowser
        if (File.Exists(@"C:\Unity\simu.json") == false) return;

        var fileText = File.ReadAllText(@"C:\Unity\simu.json");
        var wrapper = JsonUtility.FromJson<VideoJSONWrapper>(fileText);
        Debug.Log(JsonUtility.ToJson(wrapper));

        //To ease the loading process and make sure all the id's match up etc.
        //clearing the whole structure while loading new one.
        //could possibly generate a method to just apped loaded structure, but
        //not sure if that is realistic use case at this point
        ClearStructure();

        //Create nodes
        foreach (var item in wrapper.videos)
            LoadVideoNode(item);

        //Create connections
        foreach (var item in getVideoNodeList())
            foreach (var action in item.getActionNodeList())
                action.CreateLoadedConnection();

        connectionManager.redrawConnection(null, null);

        var startNodePort = startNode.GetComponent<ActionNode>().getNodePort();
        var firstVideoPort = GetVideoNodeWithId(wrapper.startId).getNodePort();
        connectionManager.createConnection(startNodePort, firstVideoPort);
        connectionManager.redrawConnection(startNodePort, firstVideoPort);
        connectionManager.showConnectionLine(startNodePort, firstVideoPort, true);
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
                                     item.endTime);
    }

    public void ClearStructure()
    {
        //S TODO
        Debug.LogError("Clearing is not yet implemented! If you had no nodes, you're fine");
    }

    public void removeVideoNode(GameObject nodeObject)
    {
        videoGameObjects.Remove(nodeObject);
    }
}
