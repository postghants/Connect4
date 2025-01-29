using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private Image image;
    [SerializeField] private Image winMark;
    private float speed;

    private Vector2 destination;
    private bool destinationReached;
    private Tile.TileState state;

    private void Update()
    {
        if (destinationReached) { return; }

        speed += acceleration * Time.deltaTime;
        transform.Translate(0, -speed, 0);
        if (transform.localPosition.y <= destination.y)
        {
            transform.localPosition = destination;
            destinationReached = true;
        }
    }

    public void SetDestination(Vector2 _destination)
    {
        speed = 0;
        destination = _destination;
        destinationReached = false;
    }

    public void SetState(Tile.TileState _state)
    {
        state = _state;
        if (state == Tile.TileState.P1) { image.color = Color.red; }
        else { image.color = Color.yellow; }
    }

    public void Flip()
    {
        if (state == Tile.TileState.P1)
        {
            state = Tile.TileState.P2;
            image.color = Color.yellow;
        }
        else
        {
            state = Tile.TileState.P1;
            image.color = Color.red;
        }
    }

    public void EnableWinMark()
    {
        winMark.enabled = true;
    }
}
