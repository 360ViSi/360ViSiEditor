using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static Enums;

public class Node : MonoBehaviour
{
    [SerializeField] int nodeId = -2;
    [SerializeField] NodeType nodeType;
    public virtual int NodeId { get => nodeId; set => nodeId = value; }
    public virtual NodeType NodeType { get => nodeType; }

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
    public Vector3 WorldPosition() => transform.position;
    public NodeMove GetNodeMove() => GetComponentInChildren<NodeMove>();
    public void Outline(bool active) => GetComponent<Outline>().enabled = active;

    public void SetTopPanelColor(Color32 topPanelColor)
    {
        this.transform.Find("TopPanel").GetComponent<Image>().color=topPanelColor;
    }
    public void SetBottomPanelColor(Color32 bottomPanelColor)
    {
        this.transform.Find("BottomPanel").GetComponent<Image>().color=bottomPanelColor;
    }

    public Color32 GetTopPanelColor()
    {
        return this.transform.Find("TopPanel").GetComponent<Image>().color;
    }

    public Color32 GetBottomPanelColor()
    {
        return this.transform.Find("BottomPanel").GetComponent<Image>().color;
    }
}