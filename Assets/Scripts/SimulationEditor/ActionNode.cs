using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ActionNode : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    [SerializeField]
    private Color baseColor;
    [SerializeField]
    private Color notConnectedColor;
    [SerializeField]
    private Color endActionColor;
    [SerializeField]
    private bool isStartNode = false;
    [SerializeField]
    public RectTransform portGameObject;


    private LineRenderer connectLine;
    private Vector3[] verticesPos = new Vector3[2];
    private RectTransform startPortRectTrans;
    private RectTransform endPortRectTrans;
    private bool nodeConnected=false;
    private RectTransform canvasRectTransform ;
    private Transform cameraTransform;


    void Awake()
    {
      //assign RectTransform
      startPortRectTrans = portGameObject;

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
      if (!isStartNode)
      {
        GetComponent<Image>().color=notConnectedColor;
      }


    }

    void Update()
    {
      verticesPos[0] = startPortRectTrans.transform.position;
      if (nodeConnected)
      {
        verticesPos[1]=endPortRectTrans.transform.position;
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
      nodeConnected=false;
      if(!isStartNode)
      {
        GetComponent<Image>().color=baseColor;
      }
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
        endPortRectTrans=null;
        nodeConnected=false;
        connectLine.enabled=false;

        //color change
        if(!isStartNode)
        {
          GetComponent<Image>().color=notConnectedColor;
        }
        return;
      }

      endPortRectTrans = endNode.GetComponent<VideoNode>().portGameObject;
      nodeConnected=true;
      connectLine.enabled =true;

      //color change

      if (isStartNode)
      {
        return;
      }
      if (endNode.GetComponent<VideoNode>().isEndNode)
      {
          GetComponent<Image>().color=endActionColor;
          return;
      }

      GetComponent<Image>().color=baseColor;



    }

    public void OnDrop(PointerEventData eventData){}
}
