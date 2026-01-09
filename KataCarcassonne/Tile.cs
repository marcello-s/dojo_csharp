#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCarcassonne;

public class Tile
{
    private int directionPointer;
    private readonly Tile[] neighbours;
    private readonly IList<TileArea> tileAreas;
    private readonly IDictionary<int, TileArea>[] areaSideMap;

    public Tile()
    {
        directionPointer = (int)DirectionEnum.Up;
        neighbours = new Tile[4];
        tileAreas = new List<TileArea>();
        areaSideMap = new SortedList<int, TileArea>[4];
        for (var i = 0; i < 4; i++)
        {
            areaSideMap[i] = new SortedList<int, TileArea>();
        }
    }

    public Tile Up
    {
        get { return neighbours[0]; }
        set { neighbours[0] = value; }
    }
    public Tile Right
    {
        get { return neighbours[1]; }
        set { neighbours[1] = value; }
    }
    public Tile Down
    {
        get { return neighbours[2]; }
        set { neighbours[2] = value; }
    }
    public Tile Left
    {
        get { return neighbours[3]; }
        set { neighbours[3] = value; }
    }
    public bool IsConnected
    {
        get { return Up != null || Right != null || Down != null || Left != null; }
    }
    public IList<TileArea> TileAreas
    {
        get { return tileAreas; }
    }
    public IDictionary<int, TileArea> SideUp
    {
        get { return areaSideMap[directionPointer % 4]; }
    }
    public IEnumerable<KeyValuePair<int, TileArea>> SideLeft
    {
        get { return areaSideMap[(directionPointer + 1) % 4]; }
    }
    public IDictionary<int, TileArea> SideDown
    {
        get { return areaSideMap[(directionPointer + 2) % 4]; }
    }
    public IDictionary<int, TileArea> SideRight
    {
        get { return areaSideMap[(directionPointer + 3) % 4]; }
    }

    public DirectionEnum Direction
    {
        get { return (DirectionEnum)directionPointer; }
    }

    public void Rotate(DirectionEnum direction)
    {
        if (IsConnected)
        {
            return;
        }

        directionPointer = (int)direction;
    }

    public void AddAreas(
        IEnumerable<KeyValuePair<int, TileArea>> properties,
        DirectionEnum direction
    )
    {
        var areas = GetSide(direction);
        foreach (var kvp in properties)
        {
            if (!areas.Contains(kvp))
            {
                areas.Add(kvp);
            }

            if (!TileAreas.Any(tile => tile.Equals(kvp.Value)))
            {
                TileAreas.Add(kvp.Value);
            }
        }
    }

    protected IDictionary<int, TileArea> GetSide(DirectionEnum direction)
    {
        var directionNumber = (int)direction;
        // switch left/right
        if (directionNumber % 2 != 0)
        {
            directionNumber = (directionNumber + 2) % 4;
        }

        return areaSideMap[(directionPointer + directionNumber) % 4];
    }

    public static void SetNeighbour(Tile a, Tile b, DirectionEnum direction)
    {
        a.neighbours[(int)direction] = b;
        b.neighbours[(int)(direction + 2) % 4] = a;
    }

    public static bool IsNeighbourMatch(Tile a, Tile b, DirectionEnum direction)
    {
        var sideA = a.GetSide(direction);
        var sideB = b.GetSide((DirectionEnum)(((int)direction + 2) % 4));
        var countEqual = sideA.Count() == sideB.Count();
        if (!countEqual)
        {
            return false;
        }

        var index = 0;
        return sideB.Reverse().Any(prop => sideA.ElementAt(index++).Value.Name == prop.Value.Name);
    }

    public static bool IsAreaEndpoint(Tile tile, TileArea prop)
    {
        if (!tile.TileAreas.Any(t => t.Equals(prop)))
        {
            throw new ArgumentException("does not belong to tile", "prop");
        }

        var edgeCount = 0;
        edgeCount += tile.SideUp.Any(t => t.Value.Equals(prop)) ? 1 : 0;
        edgeCount += tile.SideRight.Any(t => t.Value.Equals(prop)) ? 1 : 0;
        edgeCount += tile.SideDown.Any(t => t.Value.Equals(prop)) ? 1 : 0;
        edgeCount += tile.SideLeft.Any(t => t.Value.Equals(prop)) ? 1 : 0;
        return edgeCount == 1;
    }
}
