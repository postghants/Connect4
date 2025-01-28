using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private int row;
    [SerializeField] private int column;


    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse Up");
        GameManager.Instance.DropPiece(column, GetComponent<Image>());
    }
    public void OnPointerDown(PointerEventData eventData) { }
}
