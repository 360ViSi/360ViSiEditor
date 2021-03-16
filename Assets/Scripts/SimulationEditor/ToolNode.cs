using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ToolNode : MonoBehaviour
{
    int nodeId = -2;
    int nextVideo = -1;
    [SerializeField] NodePort inPort;
    [SerializeField] NodePort outPort;
    [SerializeField] TMP_Text nodeName;
    public int NodeId { get => nodeId; set => nodeId = value; }
    public TMP_Text NodeName { get => nodeName; set => nodeName = value; }
    public NodePort InPort { get => inPort; set => inPort = value; }
    public NodePort OutPort { get => outPort; set => outPort = value; }
    public int NextVideo { get => nextVideo; set => nextVideo = value; }
}
