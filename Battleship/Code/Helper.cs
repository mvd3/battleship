using System.Collections.Generic;
using System.Linq;

namespace Battleship.Platform.Helper;

public readonly struct Coordinate
{
    public int X { get; init; }
    public int Y { get; init; }
}

public class Ship
{
    public int Size { get; init; }
    public bool IsDestroyed { get; set; }
    public List<Coordinate> Position { get; set; }

    public Ship()
    {
        IsDestroyed = false;
        Position = [];
    }
}