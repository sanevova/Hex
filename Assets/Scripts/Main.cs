using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class Main : MonoBehaviour {
    public Tile blankTile;
    public Tile noTileStub;
    public GameObject iconPlaceholder;

    public Tilemap _map { get; private set; }
    public Sprite[] possibleIcons;

    private static Main _instance = null;

    public static Main GetInstance() {
        return _instance;
    }

    void Awake() {
        _instance = this;
        _map = GetComponent<Tilemap>();
        possibleIcons = Resources.LoadAll<Sprite>("Sprites/Icons");
    }

    void Start() {
        GeneratePuzzle();
    }

    public Cell _cell;

    void Update() {
        var mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseCell = _map.WorldToCell(mousePosWorld);
            var mouseCellInterpolated = _map.LocalToCellInterpolated(mousePosWorld);
        }
    }

    void GeneratePuzzle() {
        GeneratePuzzleForNumberOfLayers(3);
    }

    void GeneratePuzzleForNumberOfLayers(int n) {
        HashSet<Cell> cells = new(new Cell.CellEqualityComparer()) {
            Cell.CreateRandomCell(Vector2Int.zero)
        };
        List<Cell> outerLayer = new() { cells.First() };
        _cell = cells.First();
        for (int layer = 1; layer < n; ++layer) {
            List<Cell> nextOuterLayer = new();
            // create next outer layer from the previous one
            foreach (Cell outerCell in outerLayer) {
                // expand from cells of the previous layer
                foreach (var direction in AllDirections()) {
                    var newPos = DirectionUtils.NeighbourPos(outerCell, direction);
                    if (cells.Contains(newPos)) {
                        // skip if cell at this pos was already created before
                        continue;
                    }
                    var newCell = Cell.CreateRandomCell(newPos);
                    // match icons with icons of existing neighbour cells
                    foreach (var directionFromNewCell in AllDirections()) {
                        var neighbourToNewCellPos = DirectionUtils.NeighbourPos(newCell, directionFromNewCell);
                        bool isAdjecent = cells.TryGetValue(
                            neighbourToNewCellPos,
                            out Cell neighbourToNewCell);
                        if (isAdjecent) {
                            // Debug.Log(newCell.pos + " + " + directionFromNewCell + "  = " + neighbourToNewCell.pos);
                            var matchingEdge = DirectionUtils.OppositeDirection(directionFromNewCell);
                            newCell.icons[directionFromNewCell] = neighbourToNewCell.icons[matchingEdge];
                        }

                    }
                    nextOuterLayer.Add(newCell);
                    cells.Add(newCell);
                }
            }
            outerLayer = nextOuterLayer;
        }

        Debug.Log("Generated Puzzle with cells: " + cells.Count);
        foreach (var cell in cells) {
            RenderCell(cell);
        }
    }

    public void DestroyCellAt(Vector2Int pos) {
        var tile = GetTileAt(pos);
        if (tile == null) {
            return;
        }
        tile.DeleteAllIcons();
        _map.SetTile((Vector3Int)pos, noTileStub);
    }

    public bool TryPlaceCell(Cell cell, TargetTile tile) {
        if (!IsLegalPlacement(cell)) {
            return false;
        }
        RenderCell(cell);
        tile.ProceedToNextTargetTile();
        return true;
    }

    HexTile GetTileAt(Vector2Int pos) {
        return GetTileAtVector3((Vector3Int)pos);
    }

    HexTile GetTileAtVector3(Vector3Int pos) {
        return _map.GetTile<HexTile>(pos);
    }

    bool IsLegalPlacement(Cell cell) {
        if (_map.GetTile<HexTile>((Vector3Int)cell.pos) != null) {
            // can't place if there is a tile there already
            return false;
        }
        return !AllDirections().Any(d => !DoIconsMatch(cell, d));
    }

    bool DoIconsMatch(Cell cell, Direction direction) {
        var neighbourPos = DirectionUtils.NeighbourPos(cell, direction);
        var neighbour = _map.GetTile<HexTile>((Vector3Int)neighbourPos);
        if (neighbour == null) {
            return true;
        }
        var opposite = DirectionUtils.OppositeDirection(direction);
        return cell.icons[direction] == neighbour.cell.icons[opposite];
    }

    public void RenderCell(Cell cell) {
        _map.SetTile((Vector3Int)cell.pos, HexTile.CreateFromCell(cell));
        RenderIconForMap(cell);
    }

    void RenderIconForMap(Cell cell) {
        RenderIcons(cell, _map.transform, RenderIconsFor.Map);
    }

    public void RenderIconsForTarget(Cell cell, GameObject target) {
        RenderIcons(cell, target.transform, RenderIconsFor.Target);
    }

    private enum RenderIconsFor {
        Map,
        Target,
    }

    void RenderIcons(Cell cell, Transform parent, RenderIconsFor purpose) {
        HexTile tile = GetTileAt(cell.pos);
        foreach (var direction in AllDirections()) {
            var icon = cell.icons[direction];
            var iconPos = DirectionUtils.CellEdgeIconToWorld(_map, cell, direction);
            var iconObject = Instantiate(
                iconPlaceholder, iconPos, Quaternion.identity, parent);
            var iconSpriteRenderer = iconObject.GetComponent<SpriteRenderer>();
            iconSpriteRenderer.sprite = icon;
            // make sorting layer for icons higher than parent
            if (purpose == RenderIconsFor.Target) {
                iconObject.transform.localPosition = iconObject.transform.position;
                iconSpriteRenderer.sortingLayerName = "TargetTileIcons";
            }
            if (purpose == RenderIconsFor.Map) {
                // remember icons to remove on hex deletion
                tile.iconObjects.Add(iconObject);
                iconSpriteRenderer.sortingLayerName = "Icons";
            }
        }
    }

    Direction[] AllDirections() {
        return DirectionUtils.AllDirections();
    }
}
