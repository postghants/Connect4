using System.Collections;
using UnityEngine;

// Handles visual pieces
public class PieceManager : MonoBehaviour
{
    public static PieceManager Instance;

    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private Hashtable pieceList = new();
    [SerializeField] private Transform tilesOrigin;
    [SerializeField] private Vector2 tilesOffset;
    [SerializeField] private float spawnY;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnNewPiece(Vector2Int destination, Tile.TileState state)
    {
        Piece piece = Instantiate(piecePrefab, tilesOrigin, false).GetComponent<Piece>();
        piece.transform.Translate(tilesOffset.x * destination.x, spawnY, -20);
        piece.SetDestination(destination * tilesOffset);
        piece.SetState(state);
        pieceList.Add(destination, piece);
    }

    public void SetPieceDestination(Vector2Int position, Vector2Int destination)
    {
        if (pieceList.ContainsKey(position))
        {
            Piece piece = (Piece)pieceList[position];
            piece.SetDestination(destination * tilesOffset);
            pieceList.Remove(position);
            pieceList.Add(destination, piece);
        }
    }

    public void FlipPiece(Vector2Int position)
    {
        if (pieceList.ContainsKey(position))
        {
            ((Piece)pieceList[position]).Flip();
        }
    }

    public void RemovePiece(Vector2Int position)
    {
        if (pieceList.ContainsKey(position))
        {
            GameObject obj = ((Piece)pieceList[position]).gameObject;
            pieceList.Remove(position);
            Destroy(obj);
        }
    }

    public void EnableWinMark(Vector2Int position)
    {
        if (pieceList.ContainsKey(position))
        {
            ((Piece)pieceList[position]).EnableWinMark();
        }
    }
}
