using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float speed;
    public float zoomSpeed;

    private Vector3 gripStartMousePosition;
    private bool shouldGripCamera = false;
    private Vector3 cameraMovementOffset = Vector3.zero;
    private float finalCameraSize;
    private const float MIN_CAMERA_SIZE = 1;
    private const float MAX_CAMERA_SIZE = 7;
    private const float ZOOM_INTERPOLATION_STEP = 0.007f;
    private Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
        finalCameraSize = cam.orthographicSize;
    }

    private void LateUpdate() {
        // smooth camera grip movement
        Vector3 finalPosition = transform.position + cameraMovementOffset;
        Vector3 lerpPosition = Vector3.Lerp(transform.position, finalPosition, speed);
        transform.position = lerpPosition;

        // smooth camera zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, finalCameraSize, ZOOM_INTERPOLATION_STEP);
    }

    void Update() {
        ArrowMovement();
        CameraGripMovement();
        CameraZoom();
    }

    void ArrowMovement() {
        // arrow movement
        var arrowsInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"));
        Move(arrowsInput * Time.deltaTime * speed);
    }

    void CameraGripMovement() {
        // camera grip
        if (Input.GetMouseButtonDown(2)) {
            gripStartMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + 10 * Vector3.forward;
            shouldGripCamera = true;
        }
        if (Input.GetMouseButtonUp(2)) {
            shouldGripCamera = false;
            cameraMovementOffset = Vector3.zero;
        }
        if (shouldGripCamera) {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + 10 * Vector3.forward;
            var moveVector = gripStartMousePosition - mousePos;
            cameraMovementOffset = moveVector;
        }
    }

    void CameraZoom() {
        var scrollOffset = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scrollOffset, 0)) {
            return;
        }
        var cameraZoomOffset = -scrollOffset * zoomSpeed;
        finalCameraSize = Mathf.Clamp(
            finalCameraSize + cameraZoomOffset,
            MIN_CAMERA_SIZE,
            MAX_CAMERA_SIZE);
    }

    void Move(Vector2 moveVector) {
        var newPos = transform.position;
        newPos.x += moveVector.x;
        newPos.y += moveVector.y;
        transform.position = newPos;
    }
}
