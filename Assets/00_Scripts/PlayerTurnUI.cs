using UnityEngine;

public class PlayerTurnUI : MonoBehaviour
{
    [SerializeField] private Tile.TileState onTurn;
    [SerializeField] private Vector2 offTurnOffset;
    [SerializeField] private float smoothTime = 0.5f;

    private Vector2 basePosition;
    private Tile.TileState currentTurn = Tile.TileState.P1;

    private Vector2 currentVelocity;

    private void Start()
    {
        basePosition = transform.position;
    }

    private void Update()
    {
        if (currentTurn == onTurn)
        {
            transform.position = Vector2.SmoothDamp(transform.position, basePosition, ref currentVelocity, smoothTime);
        }
        else
        {
            transform.position = Vector2.SmoothDamp(transform.position, basePosition + offTurnOffset, ref currentVelocity, smoothTime);
        }
    }

    public void SetTurn(Tile.TileState state)
    {
        currentTurn = state;
    }
}
