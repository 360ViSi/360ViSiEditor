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
        Debug.Log("Single select");
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

    internal void SaveOldList() => oldNodes = selectedNodes;
}