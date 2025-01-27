using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private int connectHowMany;

    private Tile[,] tiles;

    private void Awake()
    {
        tiles = new Tile[gridSize.x, gridSize.y];
        foreach (var tile in tiles)
        {
            tile.State = Tile.TileState.EMPTY;
        }
    }

    public void DropPiece(int column, Tile.TileState newState)
    {
        for(int i = 0; i > gridSize.y; i++)
        {
            Tile tile = tiles[column, i];
            if(tile.State == Tile.TileState.EMPTY)
            {
                tile.State = newState;
                return;
            }
        }
    }

    public void FlipTile(Vector2Int target)
    {
        Tile tile = tiles[target.x, target.y];
        if (tile == null || tile.State == Tile.TileState.EMPTY) { return; }
        tile.FlipTile();
    }
}
