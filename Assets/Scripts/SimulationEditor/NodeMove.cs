using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class NodeMove : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    private RectTransform objectToMove;

    private ConnectionManager connectionManager;
    private StructureManager structureManager;
    private Transform cameraTransform;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private List<Connection> involvedConnections;


    void Awake()
    {
        // get canvas RectTransform
        Canvas mainCanvas = GetComponentInParent<Canvas>();
        if (mainCanvas == null)
        {
            Debug.Log("Did not get Canvas");
            return;
        }
        canvasRectTransform = mainCanvas.GetComponent<RectTransform>();

        //get camera Transform
        cameraTransform = Camera.main.GetComponent<Transform>();

        //this GameObject RectTransform
        rectTransform = GetComponent<RectTransform>();

        //get connectionManager
        connectionManager = GetComponentInParent<ConnectionManager>();
        structureManager = GetComponentInParent<StructureManager>();
        if (connectionManager == null)
        {
            Debug.Log(name + " Did not get ConnectionManager");
            return;
        }
        if (structureManager == null)
        {
            Debug.Log(name + " Did not get StructureManager");
            return;
        }
    }

    internal void Move(Vector2 delta)
    {
        involvedConnections = getInvolvedConnections();
        objectToMove.anchoredPosition += delta;
        {
            foreach (Connection connection in involvedConnections)
                connection.redrawConnectionLine();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        involvedConnections = getInvolvedConnections();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        Vector2 mouseMovement = eventData.delta;
        // canvas unit scale = how many canvas pixels is one Unity unit
        Vector3 canvasUnitScale = bitWiseInverse(canvasRectTransform.localScale);

        // Convert one screen unit to one canvas unit
        float cameraDistanceToCanvas = Mathf.Abs(cameraTransform.position.z - canvasRectTransform.position.z);
        Vector3 vec0 = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, cameraDistanceToCanvas));
        Vector3 vec1 = Camera.main.ScreenToWorldPoint(new Vector3(1.0f, 1.0f, cameraDistanceToCanvas));
        Vector3 screenToCanvasScale = bitWiseMultiplication((vec1 - vec0), canvasUnitScale);

        //scale mouse movement on screen to object movement on canvas
        float screenHeight = Screen.height;
        var delta = mouseMovement * screenToCanvasScale;

        //if not selected, move only this
        if (!NodeInspector.instance.NodeSelectionHandler.SelectedNodes.Contains(GetComponentInParent<Node>().NodeId))
            Move(delta);
        else
            structureManager.MoveSelected(delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UndoRedoHandler.instance.SaveState();
    }

    private List<Connection> getInvolvedConnections()
    {
        // get every connection that is connected to this movable nodes
        // Move option is only available in VideoNodes or for "StartNode" which is ActionNode

        List<Connection> nodeConnections = new List<Connection>();

        //connections to video node
        Node node = GetComponentInParent<Node>();

        //if ToolNode
        switch (node.NodeType)
        {
            case Enums.NodeType.Video:
                var videoNode = (VideoNode)node;
                //add connections that are connected to this video node
                nodeConnections.AddRange(connectionManager.getConnections(null, videoNode.GetNodePort()));

                //add connections that are connected from children actionNodes
                List<ActionNode> actionNodes = videoNode.GetActionNodeList();
                foreach (ActionNode actionNode in actionNodes)
                    nodeConnections.AddRange(connectionManager.getConnections(actionNode.getNodePort(), null));
                return nodeConnections;

            case Enums.NodeType.Tool:
                ToolNode thisToolNode = (ToolNode)node;
                for (int i = 0; i < thisToolNode.OutPorts.Count; i++)
                {
                    //if(thisToolNode.NextVideos[i] != -2)
                    nodeConnections.AddRange(connectionManager.getConnections(thisToolNode.OutPorts[i], null));
                }
                nodeConnections.AddRange(connectionManager.getConnections(null, thisToolNode.InPort));
                return nodeConnections;

            case Enums.NodeType.Group:
                GroupNode groupNode = (GroupNode)node;
                nodeConnections.AddRange(connectionManager.getConnections(groupNode.OutPort, null));
                nodeConnections.AddRange(connectionManager.getConnections(null, groupNode.InPort));
                return nodeConnections;

            case Enums.NodeType.Action:
                return null;

            case Enums.NodeType.StartEnd:
                NodePort startNodePort = GetComponentInParent<ActionNode>().getNodePort();
                return connectionManager.getConnections(startNodePort, null);
        }

        Debug.LogError("GetInvolvedConnections had an error, non-implemented switch case?");
        return null;
    }

    // Vector math things
    private Vector3 bitWiseInverse(Vector3 vec)
    {
        return new Vector3(1.0f / vec.x, 1.0f / vec.y, 1.0f / vec.z);
    }

    private Vector3 bitWiseMultiplication(Vector3 vec0, Vector3 vec1)
    {
        return new Vector3(vec0.x * vec1.x, vec0.y * vec1.y, vec0.z * vec1.z);
    }

}
