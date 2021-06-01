using System;
using System.Collections.Generic;
using UnityEngine;

public class GroupNode : Node
{
    int groupId = 1;

    //These are the nodes in the main layer
    [SerializeField] NodePort inPort;
    [SerializeField] NodePort outPort;

    //These are the nodes in the actaul group
    [SerializeField] NodePort startPort;
    [SerializeField] NodePort endPort;

    List<int> nodes = new List<int>();
    GroupManager groupManager;
    GameObject groupParent;
    public override int NodeId { get => groupId; }
    public GameObject GroupParent { get => groupParent; }
    public NodePort InPort { get => inPort; }
    public NodePort OutPort { get => outPort; }
    public NodePort StartPort { get => startPort; }
    public NodePort EndPort { get => endPort; }

    public void Initialize(int groupId, GroupManager groupManager, GameObject groupParent, List<int> nodes)
    {
        this.groupId = groupId;
        this.groupManager = groupManager;
        this.groupParent = groupParent;
        this.nodes = nodes;


        var startNode = groupParent.GetComponentInChildren<GroupStartNode>();
        startNode.GroupId = groupId;
        startPort = startNode.OutPort;

        var endNode = groupParent.GetComponentInChildren<GroupEndNode>();
        endNode.GroupId = groupId;
        endPort = endNode.InPort;
    }

    public void AddToGroup(int nodeId)
    {
        nodes.Add(nodeId);
        groupManager.RemoveNodeFromMainView(nodeId, groupId);
    }

    public void OpenGroup()
    {
        groupManager.OpenGroup(groupId);
    }


}