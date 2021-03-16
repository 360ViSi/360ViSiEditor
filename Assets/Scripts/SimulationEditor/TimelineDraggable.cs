using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class UnityEventFloat : UnityEvent<float> { }

///<summary>
/// Draggable elements on the video timeline, clamped to the fullbar width
/// Use dynamic UnityEvents in the inspector to set values
///</summary>
public class TimelineDraggable : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Range(0, 1)]
    float value;
    RectTransform rectTransform;
    float fullbarPadding = 0;
    float draggedPosition = 0;
    [SerializeField] Color hoverColor = Color.yellow;
    Color defaultColor;
    Image image;
    [SerializeField] TimelineDraggable snapTarget;
    [SerializeField] float snapThreshold;
    public UnityEvent OnHold;
    public UnityEventFloat OnRelease;
    public float Value
    {
        get => value;
        set
        {
            //if setting the value before Start has chance to run
            if (rectTransform == null)
                Init();

            this.value = value;
            value = Mathf.Clamp(value, 0, 1);
            draggedPosition = value * (Screen.width - 2 * fullbarPadding) + fullbarPadding;

            rectTransform.position = new Vector2(draggedPosition, rectTransform.position.y);
        }
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        //S NOTE - Expecting the canvas to be scaled to 1080p and the padding for the full bar to be 64 px
        fullbarPadding = 64f * (Screen.width / 1920f);
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
        if (snapTarget != null)
            SnapToTarget();
        OnRelease?.Invoke(value);
    }

    void SnapToTarget()
    {
        var valueDifference = Mathf.Abs(value - snapTarget.value);
        if (snapThreshold >= valueDifference)
            value = snapTarget.value;
    }

    public void OnBeginDrag(PointerEventData eventData) => OnHold?.Invoke();

    public void OnPointerExit(PointerEventData eventData) => image.color = defaultColor;

    public void OnPointerEnter(PointerEventData eventData) => image.color = hoverColor;
}

