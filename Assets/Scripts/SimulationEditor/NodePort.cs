using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodePort : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    public bool isOutPort = false;
    public bool isInPort = false;

    [SerializeField]
    private GameObject connectionLinePrefab;

    private ConnectionLine connectionLine;
    private NodePort connectedPort;
    private RectTransform canvasRectTransform ;
    private Transform cameraTransform;
    private RectTransform nodeTransform;
    //parrent nodes is either Video or Action node
    private ActionNode parentActionNode;
    private VideoNode parentVideoNode;


    void Awake()
    {
      //Instantiate LineConnection
      GameObject newConnectionLine = Instantiate(connectionLinePrefab, GetComponent<Transform>());
      connectionLine = newConnectionLine.GetComponent<ConnectionLine>();
      if (connectionLine==null)
      {
        Debug.Log(name +" Did not get ConnectionLine");
        return;
      }

      // get parent node - can be nulls!
      parentActionNode = GetComponentInParent<ActionNode>();;
      parentVideoNode = GetComponentInParent<VideoNode>();;


      //get this node's rectTransform
      nodeTransform = GetComponent<RectTransform>();
      if (nodeTransform==null)
      {
        Debug.Log(name +" Did not get nodeTransform");
        return;
      }

      // get canvas RectTransform
      Canvas mainCanvas = GetComponentInParent<Canvas>();
      if (mainCanvas==null)
      {
        Debug.Log(name +" Did not get Canvas");
        return;
      }
      canvasRectTransform = mainCanvas.GetComponent<RectTransform>();

      //get camera Transform
      cameraTransform = Camera.main.GetComponent<Transform>();
    }

    void Update()
    {
      if (isNodeConnected())
      {
        Vector3 lineStart = nodeTransform.transform.position;
        Vector3 lineEnd = connectedPort.getNodeRectTransform().transform.position;
        connectionLine.redrawLine(lineStart,lineEnd);
      }
    }

    public RectTransform getNodeRectTransform()
    {
      return nodeTransform;
    }

    public VideoNode getParentVideoNode()
    {
      return parentVideoNode;
    }

    public ActionNode getParentActionNode()
    {
      return parentActionNode;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

      if (!isOutPort)
      {
        return;
      }
      connectedPort=null;
      connectionLine.show();
    }

    public void OnDrag(PointerEventData eventData)
    {
      Vector3 mousePosition = Input.mousePosition;
      float cameraDistanceToCanvas=Mathf.Abs(cameraTransform.position.z-canvasRectTransform.position.z);
      mousePosition.z = cameraDistanceToCanvas;
      Vector3 lineEnd = Camera.main.ScreenToWorldPoint(mousePosition);
      Vector3 lineStart = nodeTransform.transform.position;
      connectionLine.redrawLine(lineStart,lineEnd);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (!isOutPort)
      {
        return;
      }

//      Debug.Log("OnEndDrag");

      GameObject dropNode = eventData.pointerEnter;
      //Debug.Log(dropNode);

      try
      {
        NodePort dropPort = dropNode.GetComponent<NodePort>();
        if (dropPort.isInPort)
        {
          connectedPort = dropPort;
          connectionLine.show();
        }
        else
        {
          connectedPort=null;
          connectionLine.hide();
        }
      }
      // did not drop on NodePort
      catch(System.NullReferenceException e)
      {
        connectedPort=null;
        connectionLine.hide();
      }

      if (parentActionNode!=null)
      {
        parentActionNode.setMode();
      }



    }

    public void OnDrop(PointerEventData eventData){}

    public NodePort getConnectedPort()
    {
      return connectedPort;
    }

    private bool isNodeConnected()
    {
      return (connectedPort!=null);
    }

}
