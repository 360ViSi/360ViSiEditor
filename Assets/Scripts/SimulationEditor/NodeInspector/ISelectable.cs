using UnityEngine;
using static Enums;
public interface ISelectable
{
    NodeType GetNodeType();
    void OnSelect(bool manualSelect);
    void DragSelect();
    Vector3 WorldPosition();
    int GetId();
    void Outline(bool active);
    NodeMove GetNodeMove();
}