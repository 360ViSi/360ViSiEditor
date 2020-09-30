using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IDropHandler
{
  private RectTransform rectTransform;
  [SerializeField]
  private float scaleFactor = 1f;

  private void Awake()
  {
    rectTransform = GetComponent<RectTransform>();
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    Debug.Log("OnBeginDrag");
  }

  public void OnDrag(PointerEventData eventData)
  {
    //Debug.Log("OnDrag");
    Vector2 mouseMovement = eventData.delta;
    float screenHeight =Screen.height;
    rectTransform.anchoredPosition += mouseMovement*scaleFactor/screenHeight;
    Debug.Log("screen: "+screenHeight+ ", camera pixel H: "+ Camera.main.pixelHeight);

  }

  public void OnEndDrag(PointerEventData eventData)
  {
//    Debug.Log("OnEndDrag");
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    Debug.Log("OnPointerDown");
  }

  public void OnDrop(PointerEventData eventData)
  {
    Debug.Log("OnDrop");
  }
}
