using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTile : MonoBehaviour {
    public Main main;

    private Cell cell;

    private bool didRenderIcons = false;
    private bool shouldDragAndDrop = true;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void FinishInitOnceEver() {
        if (didRenderIcons) {
            return;
        }
        // render icons once
        didRenderIcons = true;
        cell = Cell.CreateRandomCell();
        main.RenderIconsForTarget(cell, gameObject);
    }

    void Update() {
        FinishInitOnceEver();
        if (Input.GetMouseButtonDown(0)) {
            TryPlaceTile();
        }
        if (Input.GetMouseButtonDown(1)) {
            TryDeleteTile();
        }
        if (shouldDragAndDrop) {
            FollowMouse();
        }

        ProcessTurnInput();
    }

    void ProcessTurnInput() {
        // turning tile on mouse scroll
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TargetTile_Turn")) {
            return;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("TargetTile_Turn_CounterClockwize")) {
            return;
        }
        if (Input.mouseScrollDelta.y < -Mathf.Epsilon) {
            cell.RotateIconsClockwize();
            animator.Play("TargetTile_Turn");
        }
        if (Input.mouseScrollDelta.y > Mathf.Epsilon) {
            cell.RotateIconsCounterClockwize();
            animator.Play("TargetTile_Turn_CounterClockwize");
        }
    }

    void OnTurnClockwizeAnimationEnd() {
        TurnParentClockwizeOrCounterClockwize(true);
    }

    void OnTurnCounterClockwizeAnimationEnd() {
        TurnParentClockwizeOrCounterClockwize(false);
    }

    void TurnParentClockwizeOrCounterClockwize(bool isClockwize) {
        transform.parent.Rotate(
            new Vector3(0, 0, isClockwize
                ? -DirectionUtils.ONE_TURN_ANGLE
                : DirectionUtils.ONE_TURN_ANGLE
            )
        );
    }

    void TryPlaceTile() {
        shouldDragAndDrop = !shouldDragAndDrop;
        if (shouldDragAndDrop) {
            return;
        }
        // did drop
        var map = main._map;
        var mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mouseCell = map.WorldToCell(mousePosWorld);
        Cell droppedToCell = new() {
            pos = (Vector2Int)mouseCell,
            icons = cell.icons,
        };
        main.TryPlaceCell(droppedToCell, this);
        // start dragging again
        shouldDragAndDrop = true;
        // ProceedToNextTargetTile();
    }

    public void ProceedToNextTargetTile() {
        transform.parent.rotation = Quaternion.Euler(0, 0, 0);
        // delete all icons (child objects)
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
        // create new random target
        cell = Cell.CreateRandomCell();
        // render new icons
        main.RenderIconsForTarget(cell, gameObject);
    }

    void TryDeleteTile() {
        var mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mouseCell = main._map.WorldToCell(mousePosWorld);
        main.DestroyCellAt((Vector2Int)mouseCell);
    }

    void FollowMouse() {
        // follow mouse pointer
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
    }
}
