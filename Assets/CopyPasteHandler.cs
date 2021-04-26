using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CopyPasteHandler : MonoBehaviour
{
    [SerializeField] NodeInspector inspector;
    [SerializeField] StructureManager structureManager;
    INodeCopyPaste clipboard;
    bool isTool;

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            var copy = Copy();
            if (copy != null)
                clipboard = copy;
        }

        if (Input.GetKeyDown(KeyCode.V) && clipboard != null)
            Paste(clipboard);

        if (Input.GetKeyDown(KeyCode.D))
        {
            var copy = Copy();
            if (copy != null)
                Paste(copy);
        }

    }

    private void Paste(INodeCopyPaste node)
    {
        if (isTool)
        {
            var tool = (ToolNode)node;
            var newTool = structureManager.CreateNewToolNode();
            newTool.Question = tool.Question;
            newTool.InfoText = tool.InfoText;
            newTool.ToolType = tool.ToolType;
            
            for (int i = 0; i < tool.OutPorts.Count - 2; i++)
                newTool.CreateOutPort(false);

            UndoRedoHandler.instance.SaveState();
            return;
        }

        //TODO shortcut for this?
        var video = (VideoNode)node;
        var newNode = structureManager.CreateNewVideoNode();
        newNode.setStartTime(video.getStartTime());
        newNode.setEndTime(video.getEndTime());
        newNode.setLoop(video.getLoop());
        newNode.setLoopTime(video.getLoopTime());
        newNode.setVideoFileName(video.getVideoFileName());
        newNode.setVideoStartRotation(video.getVideoStartRotation());

        var actions = video.getActionNodeList();
        foreach (var item in actions)
        {
            newNode.CreateNewActionNode(
                item.getActionText(),
                item.getAutoEnd(),
                -2,
                item.getStartTime(),
                item.getEndTime(),
                item.getActionType(),
                item.getWorldPosition(),
                item.getIconName(),
                item.getAreaMarkerVertices(),
                item.getActionTimer());
        }
        UndoRedoHandler.instance.SaveState();
    }

    private INodeCopyPaste Copy()
    {
        if (inspector.CurrentVideoNode != null)
        {
            isTool = false;
            return (INodeCopyPaste)inspector.CurrentVideoNode;
        }
        if (inspector.CurrentToolNode != null)
        {
            isTool = true;
            return (INodeCopyPaste)inspector.CurrentToolNode;
        }

        Debug.LogError("Nothing suitable selected");
        return null;
    }
}
