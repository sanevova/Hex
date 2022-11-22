using UnityEngine;
using DG.Tweening;

public class TargetTile : MonoBehaviour {
    public Main main;
    private Cell cell;

    private bool didRenderIcons = false;
    private bool shouldDragAndDrop = true;
    private Animator animator;

    private SfxController sfx;
    private Tweener _rotationTweener;

    void Start() {
        animator = GetComponent<Animator>();
        sfx = GetComponent<SfxController>();
    }

    void FinishInitOnceEver() {
        if (didRenderIcons) {
            return;
        }
        // render icons once
        didRenderIcons = true;
        cell = main.GetNextFreeCell();
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
        if (_rotationTweener != null && _rotationTweener.IsActive()) {
            // already rotating
            return;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            cell.RotateIconsClockwize();
            TurnParentClockwize();
            sfx.OnTurn(this);
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            cell.RotateIconsCounterClockwize();
            TurnParentCounterClockwize();
            sfx.OnTurn(this);
        }
    }

    void TurnParentClockwize() {
        TurnParentClockwizeOrCounterClockwize(true);
    }

    void TurnParentCounterClockwize() {
        TurnParentClockwizeOrCounterClockwize(false);
    }

    void TurnParentClockwizeOrCounterClockwize(bool isClockwize) {
        var angleDiff = isClockwize ? -DirectionUtils.ONE_TURN_ANGLE : DirectionUtils.ONE_TURN_ANGLE;
        var endAngle = transform.parent.rotation.eulerAngles + Vector3.forward * angleDiff;
        _rotationTweener = transform.parent.DORotate(endAngle, 0.5f);
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
        bool didPlace = main.TryPlaceCell(droppedToCell, this);
        if (didPlace) {
            sfx.OnPlace(this);
        }
        // start dragging again
        shouldDragAndDrop = true;
        // ProceedToNextTargetTile();
    }

    public void ProceedToNextTargetTile() {
        _rotationTweener.Kill();
        transform.parent.rotation = Quaternion.identity;
        // delete all icons (child objects)
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        // // create new random target
        // cell = Cell.CreateRandomCell();
        cell = main.GetNextFreeCell();
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
        transform.parent.position = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
    }
}
