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
    [SerializeField]
    private float cameraDistanceToCanvas = 5.0f;
//    [SerializeField]
//    private GameObject nodePoint;

    private LineRenderer connectLine;
    private List<Vector3> verticesPos = new List<Vector3>();
    private RectTransform nodePointRectTrans;
    private RectTransform endNodeRactTrans;
    private bool nodeConnected=false;


    void Awake()
    {
      nodePointRectTrans = gameObject.GetComponent<RectTransform>();
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
    }

    void Update()
    {
      //image position in canvas pixels
      Vector3 imagePosition = nodePointRectTrans.transform.position;
      imagePosition.z = cameraDistanceToCanvas;
      Vector3 nodepointPos = Camera.main.ScreenToWorldPoint(imagePosition);
      //Vector3[] worldCorners = new Vector3[4];
      //nodeRectTrans.GetWorldCorners(worldCorners);
      //Debug.Log(nodepointPos);
      verticesPos[0] = nodepointPos;
      if (nodeConnected)
      {
        imagePosition = endNodeRactTrans.transform.position;
        imagePosition.z = cameraDistanceToCanvas;
        nodepointPos = Camera.main.ScreenToWorldPoint(imagePosition);
        verticesPos[1]=nodepointPos;
      }
      connectLine.SetPositions(verticesPos.ToArray());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
      Debug.Log("OnBeginDrag");
      connectLine.enabled =true;
    }

    public void OnDrag(PointerEventData eventData)
    {
      //Debug.Log("OnDrag");
      Vector3 mousePosition = Input.mousePosition;
      mousePosition.z = cameraDistanceToCanvas;
      Vector3 nodepointPos = Camera.main.ScreenToWorldPoint(mousePosition);
      verticesPos[1] = nodepointPos;
      connectLine.SetPositions(verticesPos.ToArray());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      Debug.Log("OnEndDrag");
      Debug.Log(eventData.pointerEnter);
      GameObject endNode = eventData.pointerEnter;
      if (endNode == null)
      {
        endNodeRactTrans=null;
        nodeConnected=false;
        connectLine.enabled=false;
      }
      else
      {
        endNodeRactTrans = endNode.GetComponent<RectTransform>();
        nodeConnected=true;
        connectLine.enabled =true;
      }

    }

    public void OnDrop(PointerEventData eventData)
    {
      Debug.Log("OnDrop");
    }
}
