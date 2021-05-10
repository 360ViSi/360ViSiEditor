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
        if(!NodeInspector.instance.NodeSelectionHandler.SelectedNodes.Contains(GetComponentInParent<ISelectable>().GetId()))
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
        VideoNode thisVideoNode = this.GetComponentInParent<VideoNode>();
        ActionNode thisActionNode = this.GetComponentInParent<ActionNode>();

        //if ToolNode
        if (thisVideoNode == null && thisActionNode == null)
        {
            ToolNode thisToolNode = GetComponentInParent<ToolNode>();
            for (int i = 0; i < thisToolNode.OutPorts.Count; i++)
            {
                //if(thisToolNode.NextVideos[i] != -2)
                nodeConnections.AddRange(connectionManager.getConnections(thisToolNode.OutPorts[i], null));
            }
            nodeConnections.AddRange(connectionManager.getConnections(null, thisToolNode.InPort));
            return nodeConnections; //S ToolNode
        }

        //if "Startnode" ?
        if (thisVideoNode == null)
        {
            NodePort startNodePort = thisActionNode.getNodePort();
            return connectionManager.getConnections(startNodePort, null);
        }

        // if VideoNode
        //add connections that are connected to this video node
        nodeConnections.AddRange(connectionManager.getConnections(null, thisVideoNode.getNodePort()));

        //add connections that are connected from children actionNodes
        List<ActionNode> actionNodes = thisVideoNode.getActionNodeList();
        foreach (ActionNode actionNode in actionNodes)
        {
            nodeConnections.AddRange(connectionManager.getConnections(actionNode.getNodePort(), null));
        }
        return nodeConnections;
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
