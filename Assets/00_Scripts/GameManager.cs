using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Handles game state & logic
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game rules")]
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private int connectHowMany;
    [SerializeField] private int clusterSize;

    [Header("References")]
    [SerializeField] private GameObject endTextObj;
    [SerializeField] private TMP_Text endText;

    [Header("Timing")]
    [SerializeField] private float turnActionTime;
    [SerializeField] private float removeTilesPauseTime;
    [SerializeField] private float updateTilesTime;


    [Header("Debug")]
    [SerializeField] private List<TileCluster> clusters = new();

    public Tile.TileState currentTurn = Tile.TileState.P1;

    private bool processingTurn;
    private Tile[,] tiles;

    private void Awake()
    {
        Instance = this;

        tiles = new Tile[gridSize.x, gridSize.y];
        for (int j = 0; j < gridSize.y; j++)
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                tiles[i, j] = new(i, j);
            }
        }
    }

    public void DropPiece(int column)
    {
        if (processingTurn) { return; }

        Debug.Log("Dropping piece");
        for (int i = 0; i < gridSize.y; i++)
        {
            Tile tile = tiles[column, i];
            if (tile.IsEmpty)
            {
                Debug.Log("Dropped piece at " + column + i);
                tile.State = currentTurn;
                PieceManager.Instance.SpawnNewPiece(new(column, i), currentTurn);
                StartCoroutine(EndTurn());
                return;
            }
        }
    }

    public void FlipTile(Vector2Int target)
    {
        if (processingTurn) { return; }

        Tile tile = tiles[target.x, target.y];
        if (tile == null || tile.IsEmpty) { return; }
        tile.FlipTile();
        PieceManager.Instance.FlipPiece(target);
        PlayerUIManager.Instance.GetFlipTokenUI(currentTurn).RemoveToken();
        StartCoroutine(EndTurn());
    }

    private IEnumerator EndTurn()
    {
        processingTurn = true;
        bool checking = true;
        yield return new WaitForSeconds(turnActionTime);
        while (checking)
        {
            checking = false;
            if (CheckForWin()) { yield break; }
            var tilesToRemove = CheckForFullRow().ToList();
            tilesToRemove.AddRange(CheckForClusters());
            if (tilesToRemove.Count > 0)
            {
                RemoveTiles(tilesToRemove.ToArray());
                yield return new WaitForSeconds(removeTilesPauseTime);
                UpdateTiles();
                yield return new WaitForSeconds(updateTilesTime);
                checking = true;
            }
        }

        switch (currentTurn)
        {
            case Tile.TileState.P1: currentTurn = Tile.TileState.P2; break;
            case Tile.TileState.P2: currentTurn = Tile.TileState.P1; break;
        }
        PlayerUIManager.Instance.SetTurn(currentTurn);
        processingTurn = false;
    }

    private void RemoveTiles(Tile[] tilesToRemove)
    {
        foreach (Tile tile in tilesToRemove)
        {
            tiles[tile.x, tile.y].State = Tile.TileState.EMPTY;
            PieceManager.Instance.RemovePiece(new(tile.x, tile.y));
        }
    }

    private void UpdateTiles()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                if (tiles[i, j].IsEmpty)
                {
                    for (int k = j + 1; k < gridSize.y; k++)
                    {
                        if (tiles[i, k].IsEmpty) { continue; }

                        tiles[i, j].State = tiles[i, k].State;
                        tiles[i, k].State = Tile.TileState.EMPTY;
                        PieceManager.Instance.SetPieceDestination(new(i, k), new(i, j));
                        break;
                    }
                }
                if (tiles[i, j].IsEmpty) { break; }
            }
        }

    }

    private void AddFlipToken()
    {
        PlayerUIManager.Instance.AddFlipToken(currentTurn);
    }

    private bool CheckForWin()
    {
        List<Tile> winTiles = new();
        List<Tile.TileState> winStates = new();

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
                        winStates.Add(tiles[i, j].State);
                        for (int l = 0; l < connectHowMany; l++)
                        {
                            winTiles.Add(tiles[i + l, j]);
                        }
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
                        winStates.Add(tiles[i, j].State);
                        for (int l = 0; l < connectHowMany; l++)
                        {
                            winTiles.Add(tiles[i, j + l]);
                        }
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
                        winStates.Add(tiles[i, j].State);
                        for (int l = 0; l < connectHowMany; l++)
                        {
                            winTiles.Add(tiles[i + l, j + l]);
                        }
                    }
                }
            }
        }

        // Descending Diagonal
        for (int i = 0; i < gridSize.x - connectHowMany + 1; i++)
        {
            for (int j = gridSize.y - 1; j >= connectHowMany - 1; j--)
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
                        winStates.Add(tiles[i, j].State);
                        for (int l = 0; l < connectHowMany; l++)
                        {
                            winTiles.Add(tiles[i + l, j - l]);
                        }
                    }
                }
            }
        }

        if (winStates.Count > 0)
        {
            foreach (Tile tile in winTiles)
            {
                PieceManager.Instance.EnableWinMark(new(tile.x, tile.y));
            }
            winStates = winStates.Distinct().ToList();
            if (winStates.Count > 1)
            {
                WinForPlayer(0);
                return true;
            }
            WinForPlayer((int)winStates[0]);
            return true;
        }

        return false;

    }

    private Tile[] CheckForFullRow()
    {
        List<Tile> tilesInFullRows = new();
        for (int j = 0; j < gridSize.y; j++)
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                if (tiles[i, j].IsEmpty) { break; }
                if (i == gridSize.x - 1)
                {
                    AddFlipToken();
                    for (int k = 0; k < gridSize.x; k++)
                    {
                        tilesInFullRows.Add(tiles[k, j]);
                    }
                }
            }
        }
        return tilesInFullRows.Distinct().ToArray();

    }

    private Tile[] CheckForClusters()
    {
        clusters.Clear();
        // Connected-component
        bool[,] processed = new bool[gridSize.x, gridSize.y];
        List<Tile> queue1 = new();
        List<Tile> queue2 = new();
        Tile.TileState currentClusterState;

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                if (processed[i, j]) { continue; }
                if (tiles[i, j].IsEmpty)
                {
                    processed[i, j] = true;
                    continue;
                }
                currentClusterState = tiles[i, j].State;
                clusters.Add(new TileCluster(currentClusterState));
                clusters[^1].TileList.Add(tiles[i, j]);
                processed[i, j] = true;
                queue1.Add(tiles[i, j]);

                while (queue1.Count > 0)
                {
                    foreach (Tile tile in queue1)
                    {
                        foreach (Vector2Int n in GetNeighbours(tile.x, tile.y))
                        {
                            if (processed[n.x, n.y]) { continue; }
                            if (tiles[n.x, n.y].IsEmpty)
                            {
                                processed[n.x, n.y] = true;
                                continue;
                            }
                            if (tiles[n.x, n.y].State == currentClusterState)
                            {
                                clusters[^1].TileList.Add(tiles[n.x, n.y]);
                                processed[n.x, n.y] = true;
                                queue2.Add(tiles[n.x, n.y]);
                            }
                        }
                    }
                    queue1.Clear();
                    queue1.AddRange(queue2);
                    queue2.Clear();
                }

            }
        }

        List<Tile> tilesInClusters = new();

        foreach (TileCluster cluster in clusters)
        {
            if (cluster.TileList.Count >= clusterSize)
            {
                AddFlipToken();
                tilesInClusters.AddRange(cluster.TileList);
            }
        }
        return tilesInClusters.Distinct().ToArray();
    }

    public Vector2Int[] GetNeighbours(int x, int y)
    {
        List<Vector2Int> neighbours = new();
        if (x - 1 >= 0) { neighbours.Add(new(x - 1, y)); }
        if (x + 1 < gridSize.x) { neighbours.Add(new(x + 1, y)); }
        if (y - 1 >= 0) { neighbours.Add(new(x, y - 1)); }
        if (y + 1 < gridSize.y) { neighbours.Add(new(x, y + 1)); }
        return neighbours.ToArray();
    }

    private void WinForPlayer(int player)
    {
        endTextObj.SetActive(true);
        if (player == 0)
        {
            endText.text = "Draw!";
        }
        else
        {
            endText.text = $"P{player} wins!";
        }
    }
}
