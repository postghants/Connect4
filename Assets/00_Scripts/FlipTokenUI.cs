using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlipTokenUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Tile.TileState player;
    [SerializeField] private Image activeBackdrop;
    [SerializeField] private TMP_Text countText;
    private int tokenCount;
    public bool toggle;

    public void AddToken()
    {
        gameObject.SetActive(true);
        tokenCount++;
        countText.text = tokenCount.ToString();
    }


    public void RemoveToken()
    {
        tokenCount--;
        countText.text = tokenCount.ToString();
        if (tokenCount == 0 && toggle)
        {
            toggle = false;
            activeBackdrop.enabled = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tokenCount == 0 || GameManager.Instance.currentTurn != player) { return; }
        toggle = !toggle;
        activeBackdrop.enabled = toggle;

    }
}
