using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaButtonDragVertex : MonoBehaviour
{
    AreaButton areaButton;
    [SerializeField] Camera videoCamera;
    [SerializeField] LayerMask videoLayer;
    // Start is called before the first frame update
    void Start()
    {
        //S TODO fix this
        videoCamera = GameObject.Find("VideoCamera").GetComponent<Camera>();
        areaButton = transform.parent.GetComponent<AreaButton>();
    }

    private void OnMouseDrag()
    {
        var ray = videoCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, videoLayer, QueryTriggerInteraction.Collide) && hit.collider.gameObject != gameObject)
            transform.position = hit.point;

        areaButton.UpdateMesh();
    }
}
