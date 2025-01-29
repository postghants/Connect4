using UnityEngine;
using UnityEngine.EventSystems;

public class TileButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private int row;
    [SerializeField] private int column;


    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse Up");
        Tile.TileState currentTurn = GameManager.Instance.currentTurn;
        if (PlayerUIManager.Instance.GetFlipTokenUI(currentTurn).toggle)
        {
            GameManager.Instance.FlipTile(new(column, row));
        }
        else
        {
            GameManager.Instance.DropPiece(column);
        }
    }
    public void OnPointerDown(PointerEventData eventData) { }
}
