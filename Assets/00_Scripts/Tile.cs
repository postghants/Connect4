using System;
using System.Collections.Generic;

[Serializable]
public class Tile
{
    public enum TileState { EMPTY, P1, P2 };
    public TileState State = TileState.EMPTY;
    public int x, y;
    public bool IsEmpty => State == TileState.EMPTY;

    public Tile(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public void FlipTile()
    {
        switch (State)
        {
            case TileState.P1: State = TileState.P2; break;
            case TileState.P2: State = TileState.P1; break;
        }
    }
}

[Serializable]
public class TileCluster
{
    public List<Tile> TileList = new();
    public Tile.TileState State;

    public TileCluster(Tile.TileState _state)
    {
        State = _state;
    }
}
