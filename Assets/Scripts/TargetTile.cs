using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTile : MonoBehaviour {
    public Main main;

    private Cell cell;

    private bool didRenderIcons = false;
    private bool shouldDragAndDrop = true;
    void Start() {

    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            shouldDragAndDrop = !shouldDragAndDrop;
            if (!shouldDragAndDrop) {
                // did drop
                var map = main._map;
                var mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var mouseCell = map.WorldToCell(mousePosWorld);
                Cell droppedToCell = new() {
                    pos = (Vector2Int)mouseCell,
                    icons = cell.icons,
                };
                main.RenderCell(droppedToCell);
                // start dragging again
                shouldDragAndDrop = true;
            }
        }

        if (shouldDragAndDrop) {
            // follow mouse pointer
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        }

        if (!didRenderIcons) {
            // render icons once
            didRenderIcons = true;
            cell = main._cell;
            main.RenderIconsForTarget(cell, gameObject);
        }

    }

}
