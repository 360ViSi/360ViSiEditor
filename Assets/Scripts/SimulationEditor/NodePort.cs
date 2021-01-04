﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodePort : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    public bool isOutPort = false;
    public bool isInPort = false;

    private ConnectionManager connectionManager;
    private RectTransform canvasRectTransform ;
    private Transform cameraTransform;
    private RectTransform nodeTransform;
    private ActionNode parentActionNode;
    private VideoNode parentVideoNode;



    void Awake()
    {
      //get connectionManager
      connectionManager = GetComponentInParent<ConnectionManager>();
      if (connectionManager==null)
      {
        Debug.Log(name +" Did not get ConnectionManager");
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
      // Start new connection drag if outPort type
      // Use ConnectionManager function when trying to create new connection
      if (!isOutPort)
      {
        return;
      }
      connectionManager.createConnection(this, null);
      connectionManager.showConnectionLine(this, null, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
      if (!isOutPort)
      {
        return;
      }

      // redraw connectionline (first in list, should be only one)
      // Use Connection Object to redraw (straight)

      Vector3 mousePosition = Input.mousePosition;
      float cameraDistanceToCanvas=Mathf.Abs(cameraTransform.position.z-canvasRectTransform.position.z);
      mousePosition.z = cameraDistanceToCanvas;
      Vector3 lineEnd = Camera.main.ScreenToWorldPoint(mousePosition);
      connectionManager.getConnections(this,null)[0].drawDragLine(lineEnd);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (!isOutPort)
      {
        return;
      }

      //Manage connections at the end of drag.

      GameObject dropNode = eventData.pointerEnter;

      try
      {
        NodePort dropPort = dropNode.GetComponent<NodePort>();
        Debug.Log("DropPort: "+dropPort);
        if (dropPort.isInPort)
        {
          //Create new connection
          connectionManager.createConnection(this, dropPort);
          connectionManager.redrawConnection(this,dropPort);
        }
        else
        {
          Debug.Log("DragEnd: not inPort");
          connectionManager.deleteConnection(this,null);
        }
      }

      // did not drop on NodePort
      catch(System.NullReferenceException e)
      {
          Debug.Log("DragEnd: null");
          connectionManager.deleteConnection(this,null);
      }

      if (parentActionNode!=null)
      {
        parentActionNode.setMode();
      }
    }

    public void OnDrop(PointerEventData eventData){}


    private bool isNodeConnected()
    {
      return (connectionManager.getConnections(this,null).Count!=0);
    }

    public Vector3 getPosition()
    {
      return nodeTransform.transform.position;
    }

}
