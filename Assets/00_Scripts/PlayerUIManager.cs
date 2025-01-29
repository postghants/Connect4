using System.Collections.Generic;
using UnityEngine;

// Handles UI for player turns and flip tokens
public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;
    [SerializeField] private List<PlayerTurnUI> turnUI = new();
    [SerializeField] private List<FlipTokenUI> flipTokenUI = new();

    private void Awake()
    {
        Instance = this;
    }

    public void SetTurn(Tile.TileState newTurn)
    {
        foreach (var ui in turnUI)
        {
            ui.SetTurn(newTurn);
        }
    }

    public void AddFlipToken(Tile.TileState targetPlayer)
    {
        flipTokenUI[(int)targetPlayer - 1].AddToken();
    }

    public FlipTokenUI GetFlipTokenUI(Tile.TileState targetPlayer)
    {
        if (targetPlayer == Tile.TileState.EMPTY) { return null; }

        return flipTokenUI[(int)targetPlayer - 1];
    }
}
