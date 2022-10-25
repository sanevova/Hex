using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexTile : Tile {
    public Cell cell;
    public List<GameObject> iconObjects = new();

    public static HexTile CreateFromCell(Cell fromCell) {
        HexTile newTile = CreateInstance<HexTile>();
        newTile.sprite = Main.GetInstance().blankTile.sprite;
        newTile.cell = fromCell;
        return newTile;
    }

    public void DeleteAllIcons() {
        foreach (var icon in iconObjects) {
            Destroy(icon);
        }
    }

    void Start() {

    }

    void Update() {

    }
}
