using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//S LATER optimize through use of events, doesn't need to update all the time
public class CanvasLine : MonoBehaviour
{
    RectTransform rectTransform;
    [Tooltip("If set, startPosition is ignored")]
    [SerializeField] RectTransform startPoint;
    [Tooltip("If set, endPosition is ignored")]
    [SerializeField] RectTransform endPoint;
    [SerializeField] Vector2 startPosition;
    [SerializeField] Vector2 endPosition;
    [Tooltip("Offsets both points but a set amount")]
    [SerializeField] Vector2 offset;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (startPoint) startPosition = startPoint.position;
        if (endPoint) endPosition = endPoint.position;

        if (startPosition == endPosition) enabled = false;

        CalculateLine();
    }

    public void CalculateLine()
    {
        enabled = true;
        rectTransform.position = startPosition + offset;

        var difference = new Vector3(endPosition.x + offset.x, endPosition.y + offset.y, 0) - rectTransform.position;
        var x = difference.x;
        var y = difference.y;
        var inner = Mathf.Pow(x, 2) + Mathf.Pow(y, 2);
        var width = Mathf.Pow(inner, .5f);

        var zRot = Mathf.Atan(y / x) * Mathf.Rad2Deg;
        zRot = x > 0 ? zRot : zRot + 180f;
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        rectTransform.localEulerAngles = new Vector3(0, 0, zRot);
    }
}
