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
    //(and special nodes: start[=actioNode], end[=VideoNode])

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
    private List<FileInfo> videoFiles = new List<FileInfo>();

    // Start is called before the first frame update
    void Start()
    {
        // get ConnectionManager
        connectionManager = GetComponent<ConnectionManager>();
        if (connectionManager == null)
        {
            Debug.Log("There are no ConnectionManager as a child of " + name);
        }
        GetFilesInFolder();
    }
    public void createNewVideoNode()
    {
        //Initialize new Video node from prefab and add it to the list
        //With Unique video ID (setVideoID handles testing)

        GameObject newVideoObject = Instantiate(videoNodePrefab, transform);
        generatedVideoID++;//initialized to zero so first used will be 1
        setVideoID(newVideoObject, generatedVideoID);
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

    public void SimulationToJson()
    {
        VideoJSONWrapper wrapper = new VideoJSONWrapper(getVideoNodeList(), startNode.GetComponent<ActionNode>().getNodePort().getNextVideoID()); //S TODO get the id of the video that start goes to
        var json = JsonUtility.ToJson(wrapper);

        Debug.Log(json.ToString());
        var defaultFileName = "test";
        var counter = 0;

        while (File.Exists(@"C:\Unity\" + defaultFileName + counter))
            counter++;
            
        var file = File.Create(@"C:\Unity\" + defaultFileName + counter + ".json");
        file.Close();
        File.WriteAllText(@"C:\Unity\" + defaultFileName + counter + ".json", json);
    }

    public void removeVideoNode(GameObject nodeObject)
    {
        videoGameObjects.Remove(nodeObject);
    }

    [ContextMenu("Test Folder")]
    public void GetFilesInFolder()
    {
        videoFiles.Clear();
        string path = @"C:\Unity";
        var info = new DirectoryInfo(path);
        var fileInfo = info.GetFiles();

        foreach (var file in fileInfo)
        {
            if (file.Extension == ".webm")
            {
//                print(file.Name.Split('.')[0]);
                videoFiles.Add(file);
            }
   //         else
  //              print(file.Extension + " not supported");
        }
    }

    public List<string> GetVideoFilenames()
    {
        var output = new List<string>();

        foreach (var item in videoFiles)
            output.Add(item.Name.Split('.')[0]);
        return output;
    }
}
