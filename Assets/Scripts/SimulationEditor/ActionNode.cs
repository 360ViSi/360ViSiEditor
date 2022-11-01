using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


[System.Serializable]
public class ActionNode : MonoBehaviour
{
    [Header("Action node colors")]    
    [SerializeField] Color32 topPanelInfo = new Color32(0x93, 0x27, 0x8F,0xFF);
    [SerializeField] Color32 bottomPanelInfo = new Color32(0xC8, 0x2D, 0xDB,0xFF);
    [SerializeField] private Color connectedColor;
    [SerializeField] private Color notConnectedColor;
    [SerializeField] private Color endActionColor;
    [SerializeField] private Color toolColor;

    
    private NodePort nodePort;
    private ConnectionManager connectionManager;
    private TextMeshProUGUI actionText;
    private Image thisImage;
    private int loadedNextVideoID = -2;
    private bool autoEnd;
    private float startTime = 0;
    private float endTime = 1;
    private ActionType actionType = ActionType.ScreenButton;
    private Vector3 worldPosition = Vector3.zero;
    private string iconName = "walk";
    private Vector3[] areaMarkerVertices = null;
    [SerializeField] GameObject removeButton = null;
    float actionTimer = 0;

    Color nodeColor;
    public Color NodeColor { get => nodeColor; }

    void Awake()
    {

        // get NodePort
        nodePort = GetComponentInChildren<NodePort>();
        if (nodePort == null)
        {
            Debug.Log("There are no NodePort in " + name);
        }

        //get Text object for action Text
        actionText = GetComponentInChildren<TextMeshProUGUI>();
        if (actionText == null)
        {
            Debug.Log("There are no text object in " + name);
        }

        //get connectionManager
        connectionManager = GetComponentInParent<ConnectionManager>();
        if (connectionManager == null)
        {
            Debug.Log(name + " Did not get ConnectionManager");
            return;
        }

        //setup ActionNode GameObject color
        thisImage = GetComponent<Image>();
        this.setMode();
    }

    public void setMode()
    {
        var connections = connectionManager.getEveryPortConnection(nodePort);
        if (connections.Count == 0)
        {
            SetColor(notConnectedColor);
            return;
        }
        NodePort connectedPort = connections[0].getToNode();
        if (connectedPort == null)
        {
            SetColor(notConnectedColor);
            return;
        }
        VideoNode connectedVideoNode = connectedPort.GetComponentInParent<VideoNode>();
        if (connectedVideoNode != null && connectedVideoNode.GetVideoID() == -1)
        {
            // Changes the color regarding to connection status
            // Colors are predefined in Prefab (Unity Inspector)
            List<Connection> portConnections = connectionManager.getConnections(this.nodePort, null);

            //empty connection list == not connected
            if (portConnections.Count == 0)
            {
                SetColor(notConnectedColor);
                return;
            }
            // get connection "to" node and its VideoNode
            //VideoNode connectedVideoNode = portConnections[0].getToNode().getParentVideoNode();
            if (connectedVideoNode != null && connectedVideoNode.GetVideoID() == -1)
            {
                SetColor(endActionColor);
                return;
            }
        }
        if (connectedVideoNode == null)
            SetColor(toolColor);
        else
            SetColor(connectedColor);
    }

    void SetColor(Color color)
    {
        nodeColor = color;
        thisImage.color = color;
        if (NodeInspector.instance.ActionDraggables != null)
            NodeInspector.instance.ActionDraggables.Refresh();
    }
    public void setAutoEnd(bool value)
    {
        //Disable autoend for everything else if setting true
        if (value)
            foreach (var item in GetComponentInParent<VideoNode>().GetActionNodeList())
                item.setAutoEnd(false);

        autoEnd = value;
    }

    public bool getAutoEnd() => autoEnd;

    public void SetActionText(string newActionText)
    {
        actionText.text = newActionText;
    }

    public string getActionText()
    {
        return actionText.text;
    }

    public int getNextVideoID()
    {
        // get video node from connected port
        // and get that VideoID (-2 if not connected)

        if (nodePort == null)
        {
            Debug.Log(this.name + "Have no port game object");
            return -2;
        }

        //NodePort connectedPort = nodePort.getConnectedPort(this, null);
        if (nodePort == null)
        {
            return -2;
        }
        else
        {
            return nodePort.getNextVideoID();
        }
    }

    ///<summary>
    /// Need to create connections when loading a structure from a file
    ///</summary>
    public void SetLoadedVideoID(int id)
    {
        if (nodePort == null)
        {
            Debug.Log(this.name + "Have no port game object");
        }
        loadedNextVideoID = id;
    }

    public void CreateLoadedConnection()
    {
        if (loadedNextVideoID != -2)
            nodePort.CreateConnection(loadedNextVideoID);
        else Debug.LogError("loadedNextVideoID was -2 and that's illegal");
    }

    public NodePort getNodePort()
    {
        return nodePort;
    }

    public void removeActionNode()
    {
        nodePort.disconnect();
        VideoNode videoNode = GetComponentInParent<VideoNode>();
        videoNode.RemoveActionNode(gameObject);
    }

    public float getEndTime() => endTime;
    public void SetEndTime(float value) => endTime = value;
    public float getStartTime() => startTime;
    public void SetStartTime(float value) => startTime = value;
    public ActionType getActionType() => actionType;
    public void SetActionType(ActionType value) => actionType = value;
    public Vector3 getWorldPosition() => worldPosition;
    public void SetWorldPosition(Vector3 value) => worldPosition = value;
    public string getIconName() => iconName;
    public void SetIconName(string value) => iconName = value;
    public Vector3[] getAreaMarkerVertices() => areaMarkerVertices;
    public void SetAreaMarkerVertices(Vector3[] value) => areaMarkerVertices = value;
    public float getActionTimer() => actionTimer;
    public void SetActionTimer(float value) => actionTimer = value;
    public void InspectorOpen() => NodeInspector.instance.CreateFields(this);

    public void SetTopPanelColor(Color32 topPanelColor)
    {
        this.transform.Find("TopPanel").GetComponent<Image>().color=topPanelColor;
    }
    public void SetBottomPanelColor(Color32 bottomPanelColor)
    {
        this.transform.Find("BottomPanel").GetComponent<Image>().color=bottomPanelColor;
    }

    public Color32 GetTopPanelColor()
    {
        return this.transform.Find("TopPanel").GetComponent<Image>().color;
    }

    public Color32 GetBottomPanelColor()
    {
        return this.transform.Find("BottomPanel").GetComponent<Image>().color;
    }
}
public enum ActionType
{
    ScreenButton,   // In VR this would be the panel in front of the player
    WorldButton,    //somewhere in world - commonly PoI in other 360 tour programs
    FloorButton,    //Marker rotated on the floor
    AreaButton,     //this will not be included in a while
    Timer,          //triggered after specified amount of seconds are elapsed after video's start
}