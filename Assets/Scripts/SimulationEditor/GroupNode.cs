using System;
using System.Collections.Generic;
using UnityEngine;

public class GroupNode : Node
{
    int groupId = 1;
    [SerializeField]NodePort inPort;
    [SerializeField]NodePort outPort;
    List<int> nodes = new List<int>();
    GroupManager groupManager;
    GameObject groupParent;
    public override int NodeId { get => groupId;}
    public GameObject GroupParent { get => groupParent; }
    public NodePort InPort { get => inPort;}
    public NodePort OutPort { get => outPort;}

    public void Initialize(int groupId, GroupManager groupManager, GameObject groupParent, List<int> nodes)
    {
        this.groupId = groupId;
        this.groupManager = groupManager;
        this.groupParent = groupParent;
        this.nodes = nodes;
    }

    public void AddToGroup(int nodeId)
    {
        nodes.Add(nodeId);
        groupManager.RemoveNodeFromMainView(nodeId, groupId);
    }

    public void OpenGroup() => groupManager.OpenGroup(groupId);
}