using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell {
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

    public void RotateIconsClockwize() {
        RotateIcons(true);
    }

    public void RotateIconsCounterClockwize() {
        RotateIcons(false);
    }

    private void RotateIcons(bool isClockwize) {
        var newIcons = new Dictionary<Direction, Sprite>(icons);
        foreach (var direction in icons.Keys) {
            var newDirection = isClockwize
                ? DirectionUtils.NextClockwizeDirection(direction)
                : DirectionUtils.NextCounterClockwizeDirection(direction);
            newIcons[newDirection] = icons[direction];
        }
        icons = newIcons;
    }
}
