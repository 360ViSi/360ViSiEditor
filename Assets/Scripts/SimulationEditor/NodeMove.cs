using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class NodeMove : MonoBehaviour,IDragHandler
{
<<<<<<< .merge_file_a19492
=======
  [SerializeField]
  private RectTransform objectToMove;

>>>>>>> .merge_file_a03716
  private Transform cameraTransform;
  private RectTransform canvasRectTransform;
  private RectTransform rectTransform;


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
<<<<<<< .merge_file_a19492
    rectTransform.anchoredPosition += mouseMovement*screenToCanvasScale;
=======
    objectToMove.anchoredPosition += mouseMovement*screenToCanvasScale;
>>>>>>> .merge_file_a03716
  }


  // Vector math things

  private Vector3 bitWiseInverse(Vector3 vec)
  {
    return new Vector3(1.0f/vec.x, 1.0f/vec.y, 1.0f/vec.z);
  }

  private Vector3 bitWiseMultiplication(Vector3 vec0, Vector3 vec1)
  {
    return new Vector3(vec0.x*vec1.x,vec0.y*vec1.y,vec0.z*vec1.z);
  }

}
