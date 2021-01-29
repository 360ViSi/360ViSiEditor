using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class VideoNode : MonoBehaviour
{
    [SerializeField]
    private int defaultVideoID = -2;
    [SerializeField] private List<GameObject> actionGameObjects = new List<GameObject>();

    // video structure parameters
    private int videoID = -2; //no ID -2 should give error when parsing
    private string videoFileName = "None"; //no video file
    private NodePort nodePort;
    private StructureManager structureManager;
    [SerializeField] private TMP_Text videoFilenameText;

    private bool loopingVideo = true;
    private float loopTime; //loopTime is 0-1 of the video length
    private GameObject autoEndAction = null;

    void Awake()
    {
        //set default videoID
        videoID = defaultVideoID;

        // get NodePort
        nodePort = GetComponentInChildren<NodePort>();
        structureManager = GetComponentInParent<StructureManager>();
        if (nodePort == null) Debug.Log("There are no NodePort in " + name);
        if (structureManager == null) Debug.LogError("There is no StructureManager in parent");
    }

    public void createNewActionNode()
    {
        createNewActionNode("action", false);
    }

    public void createNewActionNode(string actionText, bool isAutoEnd, int nextVideoId = -2)
    {
        //get prefab from structureManager and initilize that
        //add action node to the list and position it in UI
        StructureManager structureManager = GetComponentInParent<StructureManager>();
        GameObject newActionGameObject = Instantiate(structureManager.getActionNodePrefab(), GetComponent<RectTransform>());
        
        if(isAutoEnd)
         actionGameObjects.Insert(0, newActionGameObject);
        else
         actionGameObjects.Add(newActionGameObject);

        repositionActionNodes();
        var actionNode = newActionGameObject.GetComponent<ActionNode>();
        actionNode.setActionText(actionText);
        actionNode.setLoadedVideoID(nextVideoId);

        if (isAutoEnd) {
            autoEndAction = newActionGameObject;
            actionNode.DisableRemoveButton();
        }
    }

    public void setVideoID(int newVideoID)
    {
        videoID = newVideoID;
    }

    public int getVideoID()
    {
        return videoID;
    }

    public void setVideoFileName(string newVideoFileName)
    {
        videoFileName = newVideoFileName;
        videoFilenameText.text = videoFileName;
    }
    public string getVideoFileName()
    {
        return videoFileName;
    }

    public NodePort getNodePort()
    {
        return nodePort;
    }

    public bool getLoop()
    {
        return loopingVideo;
    }

    public void setLoop(bool value, bool onLoad = false)
    {
        if(loopingVideo == value && !onLoad)
            return;

        loopingVideo = value;
        if (value == false)
        {
            createNewActionNode("AUTOEND", true);
            return;
        }

        if (autoEndAction != null)
            removeActionNode(autoEndAction);
    }

    public float getLoopTime()
    {
        return loopTime;
    }

    public void setLoopTime(float value)
    {
        loopTime = value;
    }

    public List<ActionNode> getActionNodeList()
    {
        List<ActionNode> actionNodes = new List<ActionNode>();
        foreach (GameObject actionGameObject in actionGameObjects)
        {
            actionNodes.Add(actionGameObject.GetComponent<ActionNode>());
        }
        return actionNodes;
    }

    public void removeActionNode(GameObject actionGameObject)
    {
        if (actionGameObjects.Contains(actionGameObject))
            actionGameObjects.Remove(actionGameObject);

        Destroy(actionGameObject);

        repositionActionNodes();
    }
    public void repositionActionNodes()
    {
        for (int i = 0; i < actionGameObjects.Count; i++)
        {
            var actionNodeRectTransform = actionGameObjects[i].GetComponent<RectTransform>();
            actionNodeRectTransform.anchoredPosition = calculateActionImagePosition(actionNodeRectTransform, i + 1);
            actionGameObjects[i].GetComponent<ActionNode>().getNodePort().redraw();
        }

    }

    private Vector2 calculateActionNodePosition(RectTransform newNodeRectTransform)
    {
        //Calculates position for the next actionNode GameObject
        //Assumes that every action gameobject is in same size
        //Uses rectTransforms scale and rect to calculate the actual size
        Rect newNodeRect = newNodeRectTransform.rect;
        Vector3 newNodeScale = newNodeRectTransform.localScale;
        Vector2 realDimensions = new Vector2(newNodeRect.width * newNodeScale.x, newNodeRect.height * newNodeScale.y);
        return new Vector2(realDimensions.x / 2, -realDimensions.y * (actionGameObjects.Count - 0.5f));
    }

    private Vector2 calculateActionImagePosition(RectTransform newNodeRectTransform, int position)
    {
        //Calculates position for the next action GameObject
        //Assumes that every action gameobject is in same size
        Rect newNodeRect = newNodeRectTransform.rect;
        Vector3 newNodeScale = newNodeRectTransform.localScale;
        Vector2 realDimensions = new Vector2(newNodeRect.width * newNodeScale.x, newNodeRect.height * newNodeScale.y);
        return new Vector2(realDimensions.x / 2, -realDimensions.y * (position - 0.5f));
    }

    public void deleteNode()
    {
        //delete connections to this video node & remove from simulation structure
        nodePort.disconnect();
        structureManager.removeVideoNode(gameObject);

        //go through all the actions and remove them (disconnect first)
        foreach (var item in actionGameObjects)
        {
            item.GetComponent<ActionNode>().getNodePort().disconnect();
            Destroy(item);
            //S TODO: REMEMBER TO REFRESH ACTIONNODE INDEXING?
        }

        //delete this node only when it's not connected to anything
        Destroy(gameObject);
    }

    public void InspectorOpen()
    {
        //Open this video to the videoplayer
        NodeInspector.instance.CreateFields(this);
    }
}
