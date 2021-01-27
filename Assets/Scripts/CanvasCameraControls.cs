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
    [SerializeField] RectTransform canvasRectTransform;
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
        var newMousePosition = Input.mousePosition;
        var mouseDelta = newMousePosition - _oldMousePosition;

        // Convert one screen unit to one canvas unit
        float cameraDistanceToCanvas = Mathf.Abs(cameraTransform.position.z - canvasRectTransform.position.z);
        Vector3 vec0 = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, cameraDistanceToCanvas));
        Vector3 vec1 = Camera.main.ScreenToWorldPoint(new Vector3(1.0f, 1.0f, cameraDistanceToCanvas));

        //scale mouse movement on screen to object movement on canvas
        Vector2 mouseMovement = mouseDelta;
        Vector3 newPos = mouseMovement * (vec1 - vec0);
        transform.position -= newPos * _scrollSpeed;   

        _oldMousePosition = newMousePosition;
    }
}