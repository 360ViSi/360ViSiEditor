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
    private VideoNode nextVideoNode=null;
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
      if (nodeConnected && endPortRectTrans != null)
      {
        verticesPos[1]=endPortRectTrans.transform.position;
      }
      
      connectLine.SetPositions(verticesPos);
    }

    public void setActionText(string newActionText)
    {
      GetComponentInChildren<TextMeshProUGUI>().text=newActionText;
    }

    public string getActionText()
    {
      return GetComponentInChildren<TextMeshProUGUI>().text;
    }

    public int getNextVideoID()
    {
      return nextVideoNode.getVideoID();
    }

    public void Disconnect()
    {
      endPortRectTrans=null;
      nodeConnected=false;
      connectLine.enabled=false;
      nextVideoNode.RemoveInputNode(this);
      nextVideoNode=null;
      if(!isStartNode) GetComponent<Image>().color=baseColor;
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
      nextVideoNode=null;
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
      GameObject dropNode = eventData.pointerEnter;
      if (dropNode == null || dropNode.GetComponent<VideoNode>()==null)
      {
        endPortRectTrans=null;
        nodeConnected=false;
        connectLine.enabled=false;
        nextVideoNode=null;

        //color change
        if(!isStartNode)
        {
          GetComponent<Image>().color=notConnectedColor;
        }
        return;
      }

      nextVideoNode =dropNode.GetComponent<VideoNode>();
      nextVideoNode.AddInputNode(this);
      endPortRectTrans = nextVideoNode.portGameObject;
      nodeConnected=true;
      connectLine.enabled =true;

      //color change

      if (isStartNode)
      {
        return;
      }
      if (dropNode.GetComponent<VideoNode>().isEndNode)
      {
          GetComponent<Image>().color=endActionColor;
          return;
      }

      GetComponent<Image>().color=baseColor;
    }

    public void OnDrop(PointerEventData eventData){}
}
