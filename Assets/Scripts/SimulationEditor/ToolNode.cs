using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static Enums;

public class ToolNode : MonoBehaviour, INodeCopyPaste, ISelectable
{
    [SerializeField] int nodeId = -2;
    ToolType toolType;
    Question question;
    string infoText;
    StructureManager structureManager;
    [SerializeField] List<int> nextVideos = new List<int>(); //ummmmmmm only used for loading?
    [SerializeField] NodePort inPort;
    [SerializeField] List<NodePort> outPorts;
    [SerializeField] TMP_Text nodeName;
    [SerializeField] GameObject nodePortPrefab;
    [SerializeField] Transform outPortLayout;
    [SerializeField] GameEvent redrawLinesEvent;
    [SerializeField] GameObject[] outPortAmountButtons;
    public int NodeId { get => nodeId; set => nodeId = value; }
    public TMP_Text NodeName { get => nodeName; set => nodeName = value; }
    public NodePort InPort { get => inPort; set => inPort = value; }
    public List<NodePort> OutPorts { get => outPorts; set => outPorts = value; }
    public List<int> NextVideos { get => nextVideos; set => nextVideos = value; }
    public ToolType ToolType
    {
        get => toolType;
        set
        {
            toolType = value;
            nodeName.text = value.ToString();

            switch (value)
            {
                case ToolType.Random:
                    RemoveAllExcessOutPorts(2);
                    break;
                case ToolType.QuestionTask:
                    RemoveAllExcessOutPorts(2);
                    break;
                case ToolType.Info:
                    RemoveAllExcessOutPorts(0);
                    break;
            }

            SetOutPortAmountButtonsActive(
                toolType == ToolType.Random
                || toolType == ToolType.QuestionTask && question != null && !question.multichoice);
        }
    }

    public Question Question { get => question; set => question = value; }
    public string InfoText { get => infoText; set => infoText = value; }
    public StructureManager StructureManager { get => structureManager; set => structureManager = value; }

    internal int[] GetNextVideos()
    {
        var output = new int[outPorts.Count];

        for (int i = 0; i < output.Length; i++)
            output[i] = outPorts[i].getNextVideoID();

        return output;
    }

    public void CreateOutPort(bool isLoading)
    {
        var port = Instantiate(nodePortPrefab, outPortLayout).GetComponent<NodePort>();
        port.isInPort = false;
        port.isOutPort = true;

        OutPorts.Add(port);

        //if not loading -> creating more from editor, need to add member to int list also
        if (isLoading) return;

        NextVideos.Add(-2); //not connected, -2 will brake stuff
        redrawLinesEvent?.Raise();
    }

    public void RemoveLastOutPort()
    {
        if (OutPorts.Count == 2) return;

        OutPorts[OutPorts.Count - 1].disconnect();

        Destroy(outPortLayout.GetChild(outPortLayout.childCount - 1).gameObject);

        OutPorts.RemoveAt(OutPorts.Count - 1);
        NextVideos.RemoveAt(NextVideos.Count - 1);

        redrawLinesEvent?.Raise();
    }

    public void RemoveAllExcessOutPorts(int portCount = 2)
    {
        var count = OutPorts.Count - portCount;
        if (count > 0)
            for (int i = 0; i < count; i++)
            {

                OutPorts[OutPorts.Count - 1].disconnect();

                //child count doesnt refresh OnDestroy, only on Update
                Destroy(outPortLayout.GetChild(outPortLayout.childCount - 1 - i).gameObject);

                OutPorts.RemoveAt(OutPorts.Count - 1);

                if (NextVideos.Count - 1 >= 0)
                    NextVideos.RemoveAt(NextVideos.Count - 1);
            }
        else
            for (int i = 0; i < Mathf.Abs(count); i++)
                CreateOutPort(false);

        redrawLinesEvent?.Raise();
    }

    public void RemoveToolNode(bool fullClear = false)
    {
        foreach (var item in OutPorts)
            item.disconnect();

        inPort.disconnect();

        structureManager.RemoveToolNode(this);
        Destroy(gameObject);

        if (!fullClear)
            UndoRedoHandler.instance.SaveState();
    }

    public void OnSelect(bool manualSelect)
    {
        //single selection
        //happens on create of a new node or just when this is clicked without shift
        var holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!manualSelect || !holdingShift)
        {
            NodeInspector.instance.NodeSelectionHandler.SetSelectedNode(NodeId);
            NodeInspector.instance.RefreshSelection();
            return;
        }

        NodeInspector.instance.NodeSelectionHandler.AddRemoveSelectedNode(NodeId);
        NodeInspector.instance.RefreshSelection();
    }
    public void DragSelect() => NodeInspector.instance.NodeSelectionHandler.AddRemoveSelectedNode(NodeId);

    public int GetId() => NodeId;
    public NodeType GetNodeType() => NodeType.Tool;
    public Vector3 WorldPosition() => transform.position;
    public NodeMove GetNodeMove() => GetComponentInChildren<NodeMove>();
    public void Outline(bool active)
    {
        GetComponent<Outline>().enabled = active;
    }

    public void SetOutPortAmountButtonsActive(bool value)
    {
        for (int i = 0; i < outPortAmountButtons.Length; i++)
            outPortAmountButtons[i].SetActive(value);
    }
}


public enum ToolType
{
    Random,
    QuestionTask,
    Info
}