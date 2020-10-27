using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodePort : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    [SerializeField]
    private bool startPort = false;
    [SerializeField]
    private bool endPort = false;



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
      startPortRectTrans = GetComponent<RectTransform>();

      //Initialize LineRenderer
      connectLine = GetComponent<LineRenderer>();
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

    public void OnBeginDrag(PointerEventData eventData)
    {
//      Debug.Log("OnBeginDrag");
      if (!startPort)
      {
        return;
      }

      connectLine.enabled =true;
      nodeConnected=false;

    }

    public void OnDrag(PointerEventData eventData)
    {
      //Debug.Log("OnDrag");
      if (!startPort)
      {
        return;
      }

      Vector3 mousePosition = Input.mousePosition;
      float cameraDistanceToCanvas=Mathf.Abs(cameraTransform.position.z-canvasRectTransform.position.z);
      mousePosition.z = cameraDistanceToCanvas;
      Vector3 nodePointPos = Camera.main.ScreenToWorldPoint(mousePosition);
      verticesPos[1] = nodePointPos;
      connectLine.SetPositions(verticesPos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (!startPort)
      {
        return;
      }

//      Debug.Log("OnEndDrag");
      Debug.Log(eventData.pointerEnter);
      GameObject dropNode = eventData.pointerEnter;
      if (dropNode == null || !(dropNode.GetComponent<NodePort>().endPort))
      {
        endPortRectTrans=null;
        nodeConnected=false;
        connectLine.enabled=false;
        return;
      }

      endPortRectTrans = dropNode.GetComponent<RectTransform>();
      nodeConnected=true;
      connectLine.enabled =true;

    }

    public void OnDrop(PointerEventData eventData){}
}
