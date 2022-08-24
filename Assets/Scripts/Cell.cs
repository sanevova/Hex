using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Cell {
    public Vector2Int pos;
    public Dictionary<Direction, Sprite> icons;

    public static implicit operator Cell(Vector2Int p) {
        return new Cell() { pos = p, icons = new() };
    }

    public sealed class CellEqualityComparer : IEqualityComparer<Cell> {
        bool IEqualityComparer<Cell>.Equals(Cell x, Cell y) {
            return x.pos.Equals(y.pos);
        }

        int IEqualityComparer<Cell>.GetHashCode(Cell obj) {
            return obj.pos.GetHashCode();
        }
    }

}
