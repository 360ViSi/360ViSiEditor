using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCameraControls : MonoBehaviour
{
    Transform cameraTransform = default;
    Vector3 _oldMousePosition = default;
    [SerializeField] float _scrollSpeed = 1f;
    [SerializeField] float _maxAngle = 75;
    private void Start()
    {
        cameraTransform = transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            BeginCameraDrag();

        if (Input.GetMouseButton(1))
            DragCamera();
}

void BeginCameraDrag()
{
    _oldMousePosition = Input.mousePosition;
}

void DragCamera()
{
    var newMousePosition = Input.mousePosition;
    var mouseDelta = Input.mousePosition - _oldMousePosition;

    //Rotate cam, clamp in X-axis and always set Z-axis to zero, as some diagonal dragging can change it.
    cameraTransform.Rotate(new Vector3(mouseDelta.y, -mouseDelta.x) * _scrollSpeed);
    var camXRot = cameraTransform.localEulerAngles.x;
    if (camXRot > _maxAngle && camXRot < 250)
        cameraTransform.localEulerAngles = new Vector3(_maxAngle, cameraTransform.localEulerAngles.y, 0);
    else if (camXRot < 360 - _maxAngle && camXRot > 250)
        cameraTransform.localEulerAngles = new Vector3(-_maxAngle, cameraTransform.localEulerAngles.y, 0);
    else
        cameraTransform.localEulerAngles = new Vector3(cameraTransform.localEulerAngles.x, cameraTransform.localEulerAngles.y, 0);

    _oldMousePosition = newMousePosition;
}
}
