using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CopyPasteHandler : MonoBehaviour
{
    [SerializeField] StructureManager structureManager;
    List<Node> clipboard = new List<Node>();

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

    private void Paste(List<Node> nodes)
    {
        foreach (var item in nodes)
            Paste(item);
    }

    private void Paste(Node node)
    {
        if (node.NodeType == Enums.NodeType.Tool)
        {
            var tool = (ToolNode)node;
            var newTool = structureManager.CreateNewToolNode();
            newTool.Question = tool.Question;
            newTool.InfoText = tool.InfoText;
            newTool.SpriteData = tool.SpriteData;
            newTool.ImageVisible = tool.ImageVisible;
            newTool.ToolType = tool.ToolType;

            for (int i = 0; i < tool.OutPorts.Count - 2; i++)
                newTool.CreateOutPort(false);

            UndoRedoHandler.instance.SaveState();
            return;
        }

        //TODO shortcut for this?
        var video = (VideoNode)node;
        var newNode = structureManager.CreateNewVideoNode();
        newNode.SetStartTime(video.GetStartTime());
        newNode.SetEndTime(video.GetEndTime());
        newNode.SetLoop(video.GetLoop());
        newNode.SetLoopTime(video.GetLoopTime());
        newNode.SetVideoFileName(video.GetVideoFileName());
        newNode.SetVideoStartRotation(video.GetVideoStartRotation());

        var actions = video.GetActionNodeList();
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

    //S TODO !!! this doenst work if multiple are selected !!! 
    private List<Node> Copy()
    {
        List<Node> nodes = new List<Node>();
        foreach (var item in NodeInspector.instance.NodeSelectionHandler.SelectedNodes)
            nodes.Add(structureManager.GetNode(item));

        return nodes;
    }
}
