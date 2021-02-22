using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

public class AreaButtonDragVertex : MonoBehaviour
{
    EditorAreaButton areaButton;
    Camera videoCamera;
    [SerializeField] LayerMask videoLayer;
    // Start is called before the first frame update

    public void Initialize(EditorAreaButton areaButton, Camera cam)
    {
        this.areaButton = areaButton;
        this.videoCamera = cam;
    }

    private void OnMouseDrag()
    {
        var ray = videoCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, videoLayer, QueryTriggerInteraction.Collide) && hit.collider.gameObject != gameObject)
            transform.position = hit.point;

        areaButton.UpdateMesh();
    }
}
