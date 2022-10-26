using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float speed;

    private Vector3 gripStartMousePosition;
    private bool shouldGripCamera = false;
    private Vector3 cameraMovementOffset = Vector3.zero;

    void Start() {
    }

    private void LateUpdate() {
        Vector3 finalPosition = transform.position + cameraMovementOffset;
        Vector3 lerpPosition = Vector3.Lerp(transform.position, finalPosition, speed);
        transform.position = lerpPosition;
    }

    void Update() {
        var arrowsInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"));
        Move(arrowsInput * Time.deltaTime * speed);
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

    void Move(Vector2 moveVector) {
        var newPos = transform.position;
        newPos.x += moveVector.x;
        newPos.y += moveVector.y;
        transform.position = newPos;
    }
}
