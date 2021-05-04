using static Enums;
public interface ISelectable
{
    NodeType GetNodeType();
    void OnSelect(bool manualSelect);
    int GetId();
    void Outline(bool active);
}