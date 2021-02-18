using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class TimelineDraggable : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [Range(0, 1)]
    [SerializeField] float value;
    RectTransform rectTransform;
    float fullbarPadding = 64;
    float draggedPosition = 0;
    public UnityEvent OnHold;
    public UnityEvent OnRelease;
    public float Value
    {
        get => value; set
        {
            this.value = value;
            value = Mathf.Clamp(value, 0, 1);
            //S TODO if there is a problem on set here, take the screen width at start
            draggedPosition = value * (Screen.width - 2 * fullbarPadding) + fullbarPadding;
            rectTransform.position = new Vector2(draggedPosition, rectTransform.position.y);
        }
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggedPosition = eventData.position.x;
        draggedPosition = Mathf.Clamp(draggedPosition, fullbarPadding, Screen.width - fullbarPadding);
        rectTransform.position = new Vector2(draggedPosition, rectTransform.position.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        value = (draggedPosition - fullbarPadding) / (Screen.width - fullbarPadding * 2);
        OnRelease?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnHold?.Invoke();
    }
}
