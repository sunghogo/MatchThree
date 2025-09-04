using System.Collections.Generic;

public enum TileType
{
    Yellow = 0,
    Purple = 1,
    Blue = 2,
    Red = 3,
    Green = 4
}

public static class TileTypeUtil
{
    public static readonly List<TileType> Types = new List<TileType>
    {
        TileType.Yellow,
        TileType.Purple,
        TileType.Blue,
        TileType.Red,
        TileType.Green,
    };
}
