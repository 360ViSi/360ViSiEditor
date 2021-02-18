using UnityEngine;

public class WorldInspectorTarget : MonoBehaviour
{
    WorldInspector inspector;
    bool isCanvas;
    RectTransform rect;

    public WorldInspector Inspector { get => inspector; set => inspector = value; }
    public bool IsCanvas
    {
        get => isCanvas; set
        {
            isCanvas = value;
            if(value)
                rect = GetComponent<RectTransform>();
        }
    }

    private void Update() 
    {
        if(isCanvas) inspector.TargetVisible = rect.IsFullyVisibleFrom(inspector.VideoCamera);
    }

    private void OnBecameVisible() => inspector.TargetVisible = true;
    private void OnBecameInvisible() => inspector.TargetVisible = false;
}