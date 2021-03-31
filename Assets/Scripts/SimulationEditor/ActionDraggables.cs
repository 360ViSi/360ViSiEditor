using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ActionDraggables : MonoBehaviour
{
    [SerializeField] GameObject actionDraggablePrefab;
    [SerializeField] float startHeight;
    [SerializeField] float heightPerAction;
    [SerializeField] RectTransform videoControlsRect;
    [SerializeField] SO_Icons icons;

    ///<summary>
    /// Creates action start-end time sliders for each action of the video
    /// Also sizes the background of VideoControls to fit all the actions
    ///</summary>
    public void CreateActionDraggables(VideoNode videoNode)
    {
        //Clear old ones
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        //Make new ones, height is placed in reverse order, so they are spawned in the same order as under videonodes
        List<ActionNode> list = videoNode.getActionNodeList();
        if (list.Count == 0)
        {
            videoControlsRect.sizeDelta = new Vector2(0, 75);
            return;
        }
        for (int i = 0; i < list.Count; i++)
            CreateActionDraggable(list[i], new Vector2(0, startHeight + (list.Count - 1 - i) * heightPerAction));

        videoControlsRect.sizeDelta = new Vector2(0, 75 + 14 + heightPerAction * (list.Count - 1));
    }

    void CreateActionDraggable(ActionNode item, Vector2 position)
    {
        var draggableGo = Instantiate(actionDraggablePrefab);

        var draggableRect = draggableGo.GetComponent<RectTransform>();
        draggableRect.SetParent(transform, false);
        draggableRect.localPosition = position;

        //Set colors
        var draggableImages = draggableGo.GetComponentsInChildren<Image>();
        foreach (var image in draggableImages)
            image.color = item.NodeColor;


        var draggable = draggableGo.GetComponent<ActionDraggable>();
        //Apply initial values
        draggable.start.Value = item.getStartTime();
        draggable.end.Value = item.getEndTime();

        //S LATER change placeholder sprite to something else, according to ActionType (probably?)
        draggable.actionImage.sprite = icons.GetIconSprite(item.getIconName());
        draggable.actionText.text = item.getActionText();

        //Set OnRelease event listeners
        draggable.start.OnRelease.AddListener((float value) => SetStartTime(item, value));
        draggable.end.OnRelease.AddListener((float value) => SetEndTime(item, value));
    }

    void SetStartTime(ActionNode node, float value)
    {
        node.setStartTime(value);
        NodeInspector.instance.CreateFields(node, true);
    }

    void SetEndTime(ActionNode node, float value)
    {
        node.setEndTime(value);
        NodeInspector.instance.CreateFields(node, true);
    }

    internal void Refresh()
    {
        if(NodeInspector.instance.CurrentVideoNode == null) return;
        
        CreateActionDraggables(NodeInspector.instance.CurrentVideoNode);
    }
}