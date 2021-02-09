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

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
            return;

        var pos = videoCamera.WorldToScreenPoint(target.transform.position + Vector3.up * 3);
        transform.position = pos;
        graphicsObject.SetActive(targetVisible);
    }

    public void SetTarget(GameObject newTarget)
    {
        if (target != null)
        {
            Destroy(target.GetComponent<WorldInspectorTarget>());
            target = null;
        }
        if (newTarget == null) return;

        var targetComponent = newTarget.AddComponent<WorldInspectorTarget>();
        targetComponent.Inspector = this;
        this.target = newTarget.transform;
    }

    public void UpdateValue(string value) => NodeInspector.instance.CurrentActionNode.setActionText(value);
}
