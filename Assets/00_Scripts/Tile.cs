using System;

public class Tile
{
    public enum TileState { EMPTY, P1, P2 };
    public TileState State = TileState.EMPTY;
    public bool IsEmpty => State == TileState.EMPTY;

    public void FlipTile()
    {
        switch (State)
        {
            case TileState.P1: State = TileState.P2; break;
            case TileState.P2: State = TileState.P1; break;
        }
    }
}
