using UnityEngine;

public class GroupEndNode : MonoBehaviour
{
    int groupId = 0;
    NodePort inPort;
    public int GroupId { get => groupId; set => groupId = value; }
    public NodePort InPort { get => inPort; set => inPort = value; }
}