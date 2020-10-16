using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VideoNode : MonoBehaviour,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IDropHandler
{
  [SerializeField]
  private GameObject actionNodePrefab;
  [SerializeField]
  private int defaultVideoID = -2;
  [SerializeField]
  public bool isEndNode = false;
  [SerializeField]
  public RectTransform portGameObject;

  private Transform cameraTransform;
  private RectTransform canvasRectTransform;
  private RectTransform rectTransform;
  private List<GameObject> actionNodes = new List<GameObject>();

  // video structure parameters
  private int videoID=-2; //no ID -2 should give error when parsing
  private string videoFileName = "None"; //no video file

  void Awake()
  {
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

    //this GameObject RectTransform
    rectTransform = GetComponent<RectTransform>();

    //set default videoID
    videoID = defaultVideoID;
  }

  public void createNewActionNode()
  {
    GameObject newNodeObject = Instantiate(actionNodePrefab, rectTransform);
    actionNodes.Add(newNodeObject);
    RectTransform newNodeRectTransform=newNodeObject.GetComponent<RectTransform>();
    newNodeRectTransform.anchoredPosition =calculatePosition(newNodeRectTransform);
    newNodeObject.GetComponent<ActionNode>().setActionText("Action node");
  }

  public void setVideoID(int newVideoID)
  {
    videoID = newVideoID;
  }
  public int getVideoID()
  {
    return videoID;
  }

  public void setVideoFileName(string newVideoFileName)
  {
    videoFileName = newVideoFileName;
  }
  public string getVideoFilePath()
  {
    return videoFileName;
  }

  public void OnDrag(PointerEventData eventData)
  {
    // canvas unit scale = how many canvas pixels is one Unity unit
    Vector3 canvasUnitScale = bitWiseInverse(canvasRectTransform.localScale);

    // Convert one screen unit to one canvas unit
    float cameraDistanceToCanvas=Mathf.Abs(cameraTransform.position.z-canvasRectTransform.position.z);
    Vector3 vec0=Camera.main.ScreenToWorldPoint(new Vector3(0.0f,0.0f,cameraDistanceToCanvas));
    Vector3 vec1=Camera.main.ScreenToWorldPoint(new Vector3(1.0f,1.0f,cameraDistanceToCanvas));
    Vector3 screenToCanvasScale = bitWiseMultiplication((vec1-vec0),canvasUnitScale);

    //scale mouse movement on screen to object movement on canvas
    Vector2 mouseMovement = eventData.delta;
    float screenHeight =Screen.height;
    rectTransform.anchoredPosition += mouseMovement*screenToCanvasScale;
  }

  public void OnBeginDrag(PointerEventData eventData){}

  public void OnEndDrag(PointerEventData eventData){}

  public void OnPointerDown(PointerEventData eventData){}

  public void OnDrop(PointerEventData eventData){}


  // Vector math things

  private Vector3 bitWiseInverse(Vector3 vec)
  {
    return new Vector3(1.0f/vec.x, 1.0f/vec.y, 1.0f/vec.z);
  }

  private Vector3 bitWiseMultiplication(Vector3 vec0, Vector3 vec1)
  {
    return new Vector3(vec0.x*vec1.x,vec0.y*vec1.y,vec0.z*vec1.z);
  }

  private Vector2 calculatePosition(RectTransform newNodeRectTransform)
  {
    Rect newNodeRect = newNodeRectTransform.rect;
    Vector3 newNodeScale = newNodeRectTransform.localScale;
    Vector2 realDimensions=new Vector2(newNodeRect.width*newNodeScale.x, newNodeRect.height*newNodeScale.y);
    return new Vector2(realDimensions.x/2, -realDimensions.y*(actionNodes.Count-0.5f));
  }

}
