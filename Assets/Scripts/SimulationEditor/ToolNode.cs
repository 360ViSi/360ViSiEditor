﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ToolNode : MonoBehaviour
{
    int nodeId = -2;
    [SerializeField] List<int> nextVideos = new List<int>();
    [SerializeField] NodePort inPort;
    [SerializeField] List<NodePort> outPorts;
    [SerializeField] TMP_Text nodeName;
    [SerializeField] GameObject nodePortPrefab;
    [SerializeField] Transform outPortLayout;
    [SerializeField] GameEvent redrawLinesEvent;
    public int NodeId { get => nodeId; set => nodeId = value; }
    public TMP_Text NodeName { get => nodeName; set => nodeName = value; }
    public NodePort InPort { get => inPort; set => inPort = value; }
    public List<NodePort> OutPorts { get => outPorts; set => outPorts = value; }
    public List<int> NextVideos { get => nextVideos; set => nextVideos = value; }

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

    public void RemoveToolNode()
    {
        foreach (var item in OutPorts)
            item.disconnect();

        Destroy(gameObject);
    }
}
