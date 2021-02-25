using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class UnityEventFloat : UnityEvent<float> { }

public class TimelineDraggable : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Range(0, 1)]
    float value;
    RectTransform rectTransform;
    float fullbarPadding = 64;
    float draggedPosition = 0;
    [SerializeField] Color hoverColor = Color.yellow;
    Color defaultColor;
    Image image;

    public UnityEvent OnHold;
    public UnityEventFloat OnRelease;
    public float Value
    {
        get => value; 
        set
        {
            this.value = value;
            value = Mathf.Clamp(value, 0, 1);
            draggedPosition = value * (Screen.width - 2 * fullbarPadding) + fullbarPadding;
            rectTransform.position = new Vector2(draggedPosition, rectTransform.position.y);
        }
    }

    void Start()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
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
        OnRelease?.Invoke(value);
    }

    public void OnBeginDrag(PointerEventData eventData) => OnHold?.Invoke();

    public void OnPointerExit(PointerEventData eventData) => image.color = defaultColor;

    public void OnPointerEnter(PointerEventData eventData) => image.color = hoverColor;
}

