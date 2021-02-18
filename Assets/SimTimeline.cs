using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimTimeline : MonoBehaviour
{
    [SerializeField] RectTransform fullbar;
    [SerializeField] RectTransform activeBar;
    [Range(0,1)]
    [SerializeField] float startTime = 0;
    [Range(0,1)]
    [SerializeField] float endTime = 1;

    public float StartTime
    {
        get => startTime; set
        {
            startTime = value;
            SetActiveBarVisual();
        }
    }
    public float EndTime
    {
        get => endTime; set
        {
            endTime = value;
            SetActiveBarVisual();
        }
    }

    private void SetActiveBarVisual()
    {
        Debug.Log("Setting visual");
        var width = fullbar.rect.width;
        var leftPixels = width * startTime;
        var rightPixels = width * (1 - endTime);
        activeBar.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, leftPixels, 0);
        activeBar.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, rightPixels, width - leftPixels - rightPixels);
    }
}
