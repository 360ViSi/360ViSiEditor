using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using TMPro;

public class ActionNode : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    public Color baseColor;
    public Color notConnectedColor;
    public Color endActionColor;


    private LineRenderer connectLine;
    private Vector3[] verticesPos = new Vector3[2];
    private RectTransform nodePointRectTrans;
    private RectTransform endNodeRectTrans;
    private bool nodeConnected=false;
    private RectTransform canvasRectTransform ;
    private Transform cameraTransform;


    void Awake()
    {
      //assign RectTransform
      nodePointRectTrans = gameObject.GetComponent<RectTransform>();

      //Initialize LineRenderer
      connectLine = gameObject.GetComponent<LineRenderer>();
      connectLine.GetPositions(verticesPos);
      connectLine.enabled = false;

      // get canvas RectTransform
      Canvas mainCanvas = GetComponentInParent<Canvas>();
      if (mainCanvas==null)
      {
        Debug.Log("Did not get Canvas");
        return;
      }
      canvasRectTransform = mainCanvas.GetComponent<RectTransform>();

      //get camera Transform
      cameraTransform = Camera.main.GetComponent<Transform>();

      //set color to notConnectedColor
      //Image image = GetComponent<Image>();

    }

    void Update()
    {
      verticesPos[0] = nodePointRectTrans.transform.position;
      if (nodeConnected)
      {
        verticesPos[1]=endNodeRectTrans.transform.position;
      }
      connectLine.SetPositions(verticesPos);
    }

    public void setActionText(string newActionText)
    {
      GetComponentInChildren<TextMeshProUGUI>().text=newActionText;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
//      Debug.Log("OnBeginDrag");
      connectLine.enabled =true;
    }

    public void OnDrag(PointerEventData eventData)
    {
      //Debug.Log("OnDrag");
      Vector3 mousePosition = Input.mousePosition;
      float cameraDistanceToCanvas=Mathf.Abs(cameraTransform.position.z-canvasRectTransform.position.z);
      mousePosition.z = cameraDistanceToCanvas;
      Vector3 nodePointPos = Camera.main.ScreenToWorldPoint(mousePosition);
      verticesPos[1] = nodePointPos;
      connectLine.SetPositions(verticesPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
//      Debug.Log("OnEndDrag");
      Debug.Log(eventData.pointerEnter);
      GameObject endNode = eventData.pointerEnter;
      if (endNode == null || endNode.GetComponent<VideoNode>()==null)
      {
        endNodeRectTrans=null;
        nodeConnected=false;
        connectLine.enabled=false;
        return;
      }

      endNodeRectTrans = endNode.GetComponent<RectTransform>();
      nodeConnected=true;
      connectLine.enabled =true;

    }

    public void OnDrop(PointerEventData eventData){}
}
