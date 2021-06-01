using UnityEngine;

public class GroupStartNode : MonoBehaviour
{
    int groupId = 0;
    NodePort outPort;
    public int GroupId { get => groupId; set => groupId = value; }
    public NodePort OutPort { get => outPort; set => outPort = value; }
}