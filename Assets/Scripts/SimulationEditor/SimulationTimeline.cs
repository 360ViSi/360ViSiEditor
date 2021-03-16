using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationTimeline : MonoBehaviour
{
    [SerializeField] RectTransform fullbar;
    [SerializeField] RectTransform activeBar;
    [Range(0, 1)]
    [SerializeField] float startTime = 0;
    [Range(0, 1)]
    [SerializeField] float endTime = 1;
    [Range(0, 1)]
    [SerializeField] float loopTime = 0;
    [SerializeField] TimelineDraggable startDraggable;
    [SerializeField] TimelineDraggable endDraggable;
    [SerializeField] TimelineDraggable loopDraggable;
    public float StartTime
    {
        get => startTime;
        set
        {
            startTime = value;
            startDraggable.Value = value;
            SetActiveBarVisual();
        }
    }
    public float EndTime
    {
        get => endTime;
        set
        {
            endTime = value;
            endDraggable.Value = value;
            SetActiveBarVisual();
        }
    }

    public float Looptime
    {
        get => loopTime;
        set
        {
            loopTime = value;
            loopDraggable.Value = value;
        }
    }
    private void SetActiveBarVisual()
    {
        var width = fullbar.rect.width;
        var leftPixels = width * startTime;
        var rightPixels = width * (1 - endTime);
        activeBar.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, leftPixels, 0);
        activeBar.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, rightPixels, width - leftPixels - rightPixels);
    }
}
