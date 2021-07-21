using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static Enums;

public class ToolNode : Node
{
    ToolType toolType;
    Question question;
    string infoText;
    string spriteData;
    StructureManager structureManager;
    [SerializeField] List<int> nextVideos = new List<int>(); //ummmmmmm only used for loading?
    [SerializeField] NodePort inPort;
    [SerializeField] List<NodePort> outPorts;
    [SerializeField] TMP_Text nodeName;
    [SerializeField] GameObject nodePortPrefab;
    [SerializeField] Transform outPortLayout;
    [SerializeField] GameEvent redrawLinesEvent;
    [SerializeField] GameObject[] outPortAmountButtons;
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
                    SetQuestionTitle();
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


    public Question Question
    {
        get => question;
        set
        {
            question = value;
            SetQuestionTitle();
        }
    }
    public string InfoText { get => infoText; set => infoText = value; }
    public string SpriteData { get => spriteData; set => spriteData = value; }
    public StructureManager StructureManager { get => structureManager; set => structureManager = value; }

    private void SetQuestionTitle()
    {
        if (question.questionTitleText != "")
        {
            nodeName.text = question.questionTitleText;
        }
    }

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