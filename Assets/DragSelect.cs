using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSelect : MonoBehaviour
{
    private bool dragging;
    [SerializeField] Image selectionImage;
    Vector2 startPosition;
    Vector2 endPosition;
    Vector2 canvasSize;
    Vector2 delta;
    [SerializeField] StructureManager structureManager;

    // Start is called before the first frame update
    void Start()
    {
        canvasSize = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartDragSelection();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            EndDragSelection();
        }


        if (dragging)
            DragSelection();
    }

    private void DragSelection()
    {
        //Update the visual selection image
        endPosition = Input.mousePosition;

        delta = new Vector2(
            (endPosition.x - startPosition.x) * (canvasSize.x / (float)Screen.width),
            (endPosition.y - startPosition.y) * (canvasSize.y / (float)Screen.height));

        selectionImage.rectTransform.sizeDelta = delta;

        if (delta.x >= 0 && delta.y >= 0)
        {
            selectionImage.rectTransform.localScale = new Vector3(1, 1, 1);
        }
        else if (delta.x < 0 && delta.y < 0)
        {
            selectionImage.rectTransform.localScale = new Vector3(-1, -1, 1);
            selectionImage.rectTransform.sizeDelta = new Vector2(delta.x * -1, delta.y * -1);
        }
        else if (delta.x >= 0 && delta.y < 0)
        {
            selectionImage.rectTransform.localScale = new Vector3(1, -1, 1);
            selectionImage.rectTransform.sizeDelta = new Vector2(delta.x, delta.y * -1);
        }
        else if (delta.x < 0 && delta.y >= 0)
        {
            selectionImage.rectTransform.localScale = new Vector3(-1, 1, 1);
            selectionImage.rectTransform.sizeDelta = new Vector2(delta.x * -1, delta.y);
        }
    }

    private void EndDragSelection()
    {
        dragging = false;
        selectionImage.enabled = false;

        //Do the selection
        var bot = startPosition.y < endPosition.y ? startPosition.y : endPosition.y;
        var left = startPosition.x < endPosition.x ? startPosition.x : endPosition.x;
        var top = bot + delta.y;
        var right = left + delta.x;

        structureManager.SelectInScreenArea(new Vector2(left, bot), new Vector2(right, top));        
    }

    private void StartDragSelection()
    {
        dragging = true;
        selectionImage.enabled = true;
        selectionImage.rectTransform.position = Input.mousePosition;
        startPosition = Input.mousePosition;
    }
}
