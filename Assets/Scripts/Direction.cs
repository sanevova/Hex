using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction {
    LEFT,
    RIGHT,
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT,
}


public abstract class DirectionUtils {
    public static Vector2Int NeighbourPos(Cell src, Direction direction) {
        var diff = src.pos.y % 2 == 0
            ? _evenColNeighbourPosForDirection[direction]
            : _oddColNeighbourPosForDirection[direction];
        return src.pos + diff;
    }

    public static Direction OppositeDirection(Direction direction) {
        return _oppositeDirections[direction];
    }

    public static Vector3 CellEdgeIconToWorld(Tilemap map, Cell cell, Direction direction) {
        return map.CellToWorld((Vector3Int)cell.pos) + (Vector3)_pointsToCoords[direction];
    }

    public static Direction[] AllDirections() {
        if (_allDirections.Length == 0) {
            _allDirections = (Direction[])System.Enum.GetValues(typeof(Direction));
        }
        return _allDirections;
    }
    private static Dictionary<Direction, Vector2Int> _evenColNeighbourPosForDirection = new() {
        // clockwise
        {Direction.TOP_RIGHT, Vector2Int.up},
        {Direction.RIGHT, Vector2Int.right},
        {Direction.BOTTOM_RIGHT, Vector2Int.down},
        {Direction.BOTTOM_LEFT, Vector2Int.left + Vector2Int.down},
        {Direction.LEFT, Vector2Int.left},
        {Direction.TOP_LEFT, Vector2Int.left + Vector2Int.up},
    };

    private static Dictionary<Direction, Vector2Int> _oddColNeighbourPosForDirection = new() {
        // clockwise
        {Direction.TOP_RIGHT, Vector2Int.right + Vector2Int.up},
        {Direction.RIGHT, Vector2Int.right},
        {Direction.BOTTOM_RIGHT, Vector2Int.down + Vector2Int.right},
        {Direction.BOTTOM_LEFT, Vector2Int.down},
        {Direction.LEFT, Vector2Int.left},
        {Direction.TOP_LEFT, Vector2Int.up},
    };

    private static Dictionary<Direction, Direction> _oppositeDirections = new() {
        {Direction.LEFT, Direction.RIGHT},
        {Direction.RIGHT, Direction.LEFT},
        {Direction.TOP_LEFT, Direction.BOTTOM_RIGHT},
        {Direction.TOP_RIGHT, Direction.BOTTOM_LEFT},
        {Direction.BOTTOM_LEFT, Direction.TOP_RIGHT},
        {Direction.BOTTOM_RIGHT, Direction.TOP_LEFT},
    };

    private static Dictionary<Direction, Vector2> _pointsToCoords = new() {
        {Direction.TOP_RIGHT, new Vector2(0.16f, 0.25f)},
        {Direction.RIGHT, new Vector2(0.30f, 0)},
        {Direction.BOTTOM_RIGHT, new Vector2(0.16f, -0.25f)},
        {Direction.BOTTOM_LEFT, new Vector2(-0.16f, -0.25f)},
        {Direction.LEFT, new Vector2(-0.30f, 0)},
        {Direction.TOP_LEFT, new Vector2(-0.16f, 0.25f)},
    };


    private static Direction[] _allDirections = { };

}