using UnityEngine;

public class WorldInspectorTarget : MonoBehaviour
{
    [SerializeField] WorldInspector inspector;

    public WorldInspector Inspector { get => inspector; set => inspector = value; }

    private void OnBecameVisible() => inspector.TargetVisible = true;
    private void OnBecameInvisible() => inspector.TargetVisible = false;
}
