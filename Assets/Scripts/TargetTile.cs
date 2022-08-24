using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTile : MonoBehaviour {
    public Main main;

    private bool didRenderIcons = false;

    void Start() {

    }

    void Update() {
        // follow mouse pointer
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        if (!didRenderIcons) {
            // render icons once
            didRenderIcons = true;
            main.RenderIconsForTarget(main._cell, gameObject);
        }

    }
}
