using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class NodeConnection : MonoBehaviour,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    [SerializeField]
    private float lineWidth = 0.1f;
    [SerializeField]
    private Color lineColor = Color.red;
    [SerializeField]
    private Material lineMaterial;
//    [SerializeField]
//    private float cameraDistanceToCanvas = 5.0f;
//    [SerializeField]
//    private GameObject nodePoint;

    private LineRenderer connectLine;
    private List<Vector3> verticesPos = new List<Vector3>();
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
      connectLine = gameObject.AddComponent<LineRenderer>();
      connectLine.material = lineMaterial;
      verticesPos.Add(new Vector3(0,0,0));
      verticesPos.Add(new Vector3(1,0,0));
      connectLine.startWidth= lineWidth;
      connectLine.endWidth = lineWidth;
      connectLine.startColor = lineColor;
      connectLine.endColor = lineColor;
      connectLine.SetPositions(verticesPos.ToArray());
      connectLine.useWorldSpace = true;
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

    }

    void Update()
    {
      verticesPos[0] = nodePointRectTrans.transform.position;
      if (nodeConnected)
      {
        verticesPos[1]=endNodeRectTrans.transform.position;
      }
      connectLine.SetPositions(verticesPos.ToArray());
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
      connectLine.SetPositions(verticesPos.ToArray());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
//      Debug.Log("OnEndDrag");
      Debug.Log(eventData.pointerEnter);
      GameObject endNode = eventData.pointerEnter;
      if (endNode == null)
      {
        endNodeRectTrans=null;
        nodeConnected=false;
        connectLine.enabled=false;
      }
      else
      {
        endNodeRectTrans = endNode.GetComponent<RectTransform>();
        nodeConnected=true;
        connectLine.enabled =true;
      }

    }

    public void OnDrop(PointerEventData eventData){}
}
