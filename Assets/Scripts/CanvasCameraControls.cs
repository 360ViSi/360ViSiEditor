using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Hold middle mouse button to drag the camera and scroll to zoom in/out
///</summary>
public class CanvasCameraControls : MonoBehaviour
{
    Transform cameraTransform = default;
    Vector3 _oldMousePosition = default;
    [SerializeField] float _scrollSpeed = 1f;
    [SerializeField] float _maxZoom = 15;
    [SerializeField] float _minZoom = 2;
    private void Start() => cameraTransform = Camera.main.transform;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
            BeginCameraDrag();

        if (Input.GetMouseButton(2))
            DragCamera();

        CameraZoom();
    }

    void CameraZoom()
    {
        cameraTransform.Translate(0, 0, Input.mouseScrollDelta.y * _scrollSpeed);

        //Clamps
        if (cameraTransform.position.z < -_maxZoom)
            cameraTransform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, -_maxZoom);
        else if (cameraTransform.position.z > -_minZoom)
            cameraTransform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, -_minZoom);
    }

    void BeginCameraDrag()
    {
        _oldMousePosition = Input.mousePosition;
    }

    void DragCamera()
    {
        var camDistance = Mathf.Abs(cameraTransform.position.z);
        var newMousePosition = Input.mousePosition;
        var mouseDelta = Input.mousePosition - _oldMousePosition;
        cameraTransform.Translate(-mouseDelta / (1000 / camDistance));
        _oldMousePosition = newMousePosition;
    }
}
