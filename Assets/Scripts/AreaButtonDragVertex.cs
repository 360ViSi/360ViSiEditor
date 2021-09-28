using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms;
using UnityEngine;

public class AreaButtonDragVertex : MonoBehaviour
{
    EditorAreaButton areaButton;
    Camera videoCamera;
    MeshRenderer meshRenderer;
    [SerializeField] LayerMask videoLayer;

    // Start is called before the first frame update
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Initialize(EditorAreaButton areaButton, Camera cam)
    {
        this.areaButton = areaButton;
        this.videoCamera = cam;
    }


    private void OnMouseDrag()
    {
        if(areaButton == null) return;

        var ray = videoCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, videoLayer, QueryTriggerInteraction.Collide) && hit.collider.gameObject != gameObject)
            transform.position = hit.point;

        areaButton.UpdateMesh();
    }
}
