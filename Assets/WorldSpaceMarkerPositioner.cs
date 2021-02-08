using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceMarkerPositioner : MonoBehaviour
{
    [SerializeField] Camera videoCamera;
    [SerializeField] LayerMask videoLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) == false) return;
        Debug.Log("Click");
        var ray = videoCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, videoLayer, QueryTriggerInteraction.Collide))
        {
            Debug.Log(hit.transform.name);
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.layer = 9;
            go.transform.SetPositionAndRotation(hit.point, Quaternion.identity);
            go.transform.localScale = new Vector3(10, .5f, 10);
            Debug.Log(go.transform.position);
        }
    }
}
