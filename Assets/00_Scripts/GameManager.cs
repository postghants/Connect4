using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private int connectHowMany;

    private Tile[,] tiles;

    private void Awake()
    {
        Instance = this;

        tiles = new Tile[gridSize.x, gridSize.y];
        for (int j = 0; j < gridSize.y; j++)
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                tiles[i, j] = new();
            }
        }
    }

    public void DropPiece(int column, Tile.TileState newState)
    {
        Debug.Log("Dropping piece");
        for (int i = 0; i < gridSize.y; i++)
        {
            Tile tile = tiles[column, i];
            if (tile.IsEmpty)
            {
                Debug.Log("Dropped piece at " + i + column);
                tile.State = newState;
                EndTurn();
                return;
            }
        }
    }

    public void FlipTile(Vector2Int target)
    {
        Tile tile = tiles[target.x, target.y];
        if (tile == null || tile.IsEmpty) { return; }
        tile.FlipTile();
        EndTurn();
    }

    private void EndTurn()
    {

        CheckForWin();
        CheckForFullLine();
    }

    private void CheckForWin()
    {
        // Horizontal
        for (int j = 0; j < gridSize.y; j++)
        {
            for (int i = 0; i < gridSize.x - connectHowMany + 1; i++)
            {
                if (tiles[i, j].IsEmpty) { continue; }
                for (int k = 1; k < connectHowMany; k++)
                {
                    if (tiles[i, j].State != tiles[i + k, j].State)
                    {
                        i += k - 1;
                        break;
                    }
                    if (k == connectHowMany - 1)
                    {
                        WinForPlayer((int)tiles[i, j].State);
                    }
                }
            }
        }

        // Vertical
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y - connectHowMany + 1; j++)
            {
                if (tiles[i, j].IsEmpty) { continue; }
                for (int k = 1; k < connectHowMany; k++)
                {
                    if (tiles[i, j].State != tiles[i, j + k].State)
                    {
                        j += k - 1;
                        break;
                    }
                    if (k == connectHowMany - 1)
                    {
                        WinForPlayer((int)tiles[i, j].State);
                    }
                }
            }
        }

        // Ascending Diagonal
        for (int i = 0; i < gridSize.x - connectHowMany + 1; i++)
        {
            for (int j = 0; j < gridSize.y - connectHowMany + 1; j++)
            {
                if (tiles[i, j].IsEmpty) { continue; }
                for (int k = 1; k < connectHowMany; k++)
                {
                    if (tiles[i, j].State != tiles[i + k, j + k].State)
                    {
                        break;
                    }
                    if (k == connectHowMany - 1)
                    {
                        WinForPlayer((int)tiles[i, j].State);
                    }
                }
            }
        }

        // Descending Diagonal
        for (int i = 0; i < gridSize.x - connectHowMany + 1; i++)
        {
            for (int j = gridSize.y - 1; j >= connectHowMany; j--)
            {
                if (tiles[i, j].IsEmpty) { continue; }
                for (int k = 1; k < connectHowMany; k++)
                {
                    if (tiles[i, j].State != tiles[i + k, j - k].State)
                    {
                        break;
                    }
                    if (k == connectHowMany - 1)
                    {
                        WinForPlayer((int)tiles[i, j].State);
                    }
                }
            }
        }
    }

    private bool CheckForFullLine()
    {
        for (int j = 0; j < gridSize.y; j++)
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                if (tiles[i, j].IsEmpty) { break; }
                if (i == gridSize.x - 1) { return true; }
            }
        }
        return false;
    }

    private void WinForPlayer(int player)
    {
        Debug.Log($"Player {player} won!");
    }
}
