using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class Main : MonoBehaviour {
    public Tile blankTile;
    public GameObject iconPlaceholder;

    private Tilemap _map;
    private Sprite[] _icons;

    void Awake() {
        _map = GetComponent<Tilemap>();
        _icons = Resources.LoadAll<Sprite>("Sprites/Icons");
    }

    void Start() {
        GeneratePuzzle();
    }

    void Update() {

        if (Input.GetMouseButtonDown(0)) {
            var mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseCell = _map.WorldToCell(mousePosWorld);
            var mouseCellInterpolated = _map.LocalToCellInterpolated(mousePosWorld);

            Debug.Log(_map.WorldToCell(mousePosWorld));
        }
    }

    void GeneratePuzzle() {
        GeneratePuzzleForNumberOfLayers(3);
    }

    void GeneratePuzzleForNumberOfLayers(int n) {
        HashSet<Cell> cells = new(new Cell.CellEqualityComparer()) {
            CreateRandomCell(Vector2Int.zero)
        };
        List<Cell> outerLayer = new() { cells.First() };
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
                    var newCell = CreateRandomCell(newPos);
                    // match icons with icons of existing neighbour cells
                    foreach (var directionFromNewCell in AllDirections()) {
                        var neighbourToNewCellPos = DirectionUtils.NeighbourPos(newCell, directionFromNewCell);
                        bool isAdjecent = cells.TryGetValue(
                            neighbourToNewCellPos,
                            out Cell neighbourToNewCell);
                        if (isAdjecent) {
                            Debug.Log(newCell.pos + " + " + directionFromNewCell + "  = " + neighbourToNewCell.pos);
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

    void RenderCell(Cell cell) {
        _map.SetTile((Vector3Int)cell.pos, blankTile);
        foreach (var direction in AllDirections()) {
            var icon = cell.icons[direction];
            var iconPos = DirectionUtils.CellEdgeIconToWorld(_map, cell, direction);
            var iconObject = GameObject.Instantiate(iconPlaceholder, iconPos, Quaternion.identity);

            iconObject.GetComponent<SpriteRenderer>().sprite = icon;
        }
    }

    Cell CreateRandomCell(Vector2Int pos) {
        Cell cell = pos;
        foreach (Direction direction in AllDirections()) {
            var randomIconIndex = UnityEngine.Random.Range(0, _icons.Length);
            cell.icons[direction] = _icons[randomIconIndex];
        }
        return cell;
    }

    Direction[] AllDirections() {
        return DirectionUtils.AllDirections();
    }
}
