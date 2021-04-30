using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeSelectionHandler
{
    [SerializeField] List<int> selectedNodes; //ids for tools and videos
    [SerializeField] List<int> oldList;

    public NodeSelectionHandler()
    {
        selectedNodes = new List<int>();
        oldList = new List<int>();
    }

    public List<int> SelectedNodes => selectedNodes;
    public List<int> OldNodes => oldList;

    internal void SetSelectedNode(int id)
    {
        selectedNodes.Clear();
        AddSelectedNode(id);
    }

    internal void AddSelectedNode(int id)
    {
        if (selectedNodes.Contains(id))
            selectedNodes.Remove(id);
        else
            selectedNodes.Add(id);
    }

    internal void SaveOldList() => oldList = selectedNodes;
}