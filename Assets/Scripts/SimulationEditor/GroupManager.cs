using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    [SerializeField] GameObject groupNodePrefab;
    [SerializeField] GameObject groupParentPrefab;
    [SerializeField] StructureManager structureManager;
    [SerializeField] Transform nodeCanvasTransform;
    [SerializeField] List<GroupNode> groups = new List<GroupNode>();

    public List<GroupNode> Groups { get => groups; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseGroup();

        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            return;

        if (Input.GetKeyDown(KeyCode.G))
            CreateGroupFromSelected();
    }

    private void CloseGroup()
    {
        foreach (var item in groups)
            item.GroupParent.SetActive(false);

        nodeCanvasTransform.gameObject.SetActive(true);
    }

    public void LoadGroup()
    {
        throw new NotImplementedException();
    }

    public void OpenGroup(int groupId)
    {
        groups.Where(e => e.NodeId == groupId).First().GroupParent.SetActive(true);
        nodeCanvasTransform.gameObject.SetActive(false);
    }

    public void CreateGroupFromSelected()
    {
        var selectedNodeIds = NodeInspector.instance.NodeSelectionHandler.SelectedNodes;
        if (selectedNodeIds.Count == 0)
        {
            Debug.LogError("Nothing selected");
            return;
        }

        var nodes = new List<int>();
        foreach (var item in selectedNodeIds)
            nodes.Add(item);



        var groupNode = Instantiate(groupNodePrefab, nodeCanvasTransform).GetComponent<GroupNode>();
        groups.Add(groupNode);

        var groupParent = Instantiate(groupParentPrefab, transform);
        
        var freeNodeId = structureManager.GetFreeNodeID();
        groupNode.Initialize(freeNodeId, this, groupParent, nodes);

        foreach (var item in nodes)
            RemoveNodeFromMainView(item, freeNodeId);

        nodeCanvasTransform.gameObject.SetActive(false);
    }

    public void RemoveNodeFromMainView(int nodeId, int groupTarget)
    {
        var nodeTransform = structureManager.GetNode(nodeId).transform;
        nodeTransform.SetParent(groups.Where(e => e.NodeId == groupTarget).First().GroupParent.transform);
    }
}