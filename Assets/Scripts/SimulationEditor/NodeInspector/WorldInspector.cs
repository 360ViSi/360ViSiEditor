using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldInspector : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Camera videoCamera;
    [SerializeField] GameObject graphicsObject;

    bool targetVisible;

    public bool TargetVisible { get => targetVisible; set => targetVisible = value; }
    public Camera VideoCamera { get => videoCamera; }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
            return;

        var pos = videoCamera.WorldToScreenPoint(target.transform.position + Vector3.up * 5);
        transform.position = pos;
        graphicsObject.SetActive(targetVisible);
    }

    public void SetTarget(GameObject newTarget, bool isCanvas = false)
    {
        if (target != null)
        {
            Destroy(target.GetComponent<WorldInspectorTarget>());
            target = null;
        }
        if (newTarget == null) return;

        var targetComponent = newTarget.AddComponent<WorldInspectorTarget>();
        targetComponent.IsCanvas = isCanvas;
        targetComponent.Inspector = this;
        this.target = newTarget.transform;
    }

    public void UpdateValue(string value) => NodeInspector.instance.CurrentActionNode.SetActionText(value);
}
