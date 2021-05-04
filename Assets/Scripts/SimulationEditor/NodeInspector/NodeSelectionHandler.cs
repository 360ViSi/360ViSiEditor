using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeSelectionHandler
{
    [SerializeField] List<int> selectedNodes; //ids for tools and videos
    [SerializeField] List<int> oldNodes;

    public NodeSelectionHandler()
    {
        selectedNodes = new List<int>();
        oldNodes = new List<int>();
    }

    public List<int> SelectedNodes => selectedNodes;
    public List<int> OldNodes => oldNodes;

    internal void SetSelectedNode(int id)
    {
        selectedNodes.Clear();
        selectedNodes.Add(id);
    }

    internal void AddRemoveSelectedNode(int id)
    {
        if (selectedNodes.Contains(id))
            selectedNodes.Remove(id);
        else
            selectedNodes.Add(id);
    }

    internal void SaveOldList()
    {
        oldNodes.Clear();
        foreach (var item in selectedNodes)
            oldNodes.Add(item);
    }
}