using UnityEngine;
using static Enums;
public interface ISelectable
{
    NodeType GetNodeType();
    void OnSelect(bool manualSelect);
    Vector2 ScreenPosition();
    int GetId();
    void Outline(bool active);
}