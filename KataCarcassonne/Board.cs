#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCarcassonne;

public class Board
{
    public IList<Tuple<int, int, Tile>> Tiles { get; private set; }
    public IList<AreaTileMap> AreaTileMaps { get; private set; }

    public Board()
    {
        Tiles = new List<Tuple<int, int, Tile>>();
        AreaTileMaps = new List<AreaTileMap>();
    }

    public void AddTile(int x, int y, Tile tile)
    {
        var tuple = new Tuple<int, int, Tile>(x, y, tile);
        if (Tiles.Any(t => t.Item1.Equals(x) && t.Item2.Equals(y)))
        {
            return;
        }

        // get neighbours
        // check match
        // connect neighbours
        if (!MatchAndConnect(x, y, tile, DirectionEnum.Up))
        {
            return;
        }

        if (!MatchAndConnect(x, y, tile, DirectionEnum.Right))
        {
            return;
        }

        if (!MatchAndConnect(x, y, tile, DirectionEnum.Down))
        {
            return;
        }

        if (!MatchAndConnect(x, y, tile, DirectionEnum.Left))
        {
            return;
        }

        Tiles.Add(tuple);
        foreach (var prop in tile.TileAreas)
        {
            if (!AreaTileMaps.Any(map => map.Area.Equals(prop)))
            {
                var map = new AreaTileMap(prop);
                map.Tiles.Add(tile);
                AreaTileMaps.Add(map);
            }
            else
            {
                var map = AreaTileMaps.Where(m => m.Area.Equals(prop)).First();
                if (!map.Tiles.Contains(tile))
                {
                    map.Tiles.Add(tile);
                }
            }
        }
    }

    private bool MatchAndConnect(int x, int y, Tile a, DirectionEnum direction)
    {
        Tuple<int, int, Tile>? tuple = null;
        switch (direction)
        {
            case DirectionEnum.Up:
                tuple = Tiles.Where(t => t.Item1 == x && t.Item2 == y + 1).FirstOrDefault();
                break;
            case DirectionEnum.Right:
                tuple = Tiles.Where(t => t.Item1 == x + 1 && t.Item2 == y).FirstOrDefault();
                break;
            case DirectionEnum.Down:
                tuple = Tiles.Where(t => t.Item1 == x && t.Item2 == y - 1).FirstOrDefault();
                break;
            case DirectionEnum.Left:
                tuple = Tiles.Where(t => t.Item1 == x - 1 && t.Item2 == y).FirstOrDefault();
                break;
        }

        if (tuple == null)
        {
            return true;
        }

        var b = tuple.Item3;

        if (!Tile.IsNeighbourMatch(a, b, direction))
        {
            return false;
        }

        Tile.SetNeighbour(a, b, direction);
        return true;
    }

    public static bool IsAreaClosed(Board board, Tile tile, TileArea area)
    {
        var map = board.AreaTileMaps.Where(m => m.Area.Name == area.Name).FirstOrDefault();
        if (map != null)
        {
            var junctionTiles = map.Tiles.Where(t => !Tile.IsAreaEndpoint(t, map.Area));
            if (junctionTiles.Any())
            {
                Console.WriteLine("junctions found on prop: " + map.Area.Name);
                return junctionTiles.All(junction => IsTileConnectedForProp(junction, map.Area));
            }

            if (!IsTileConnectedForProp(tile, area))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsTileConnectedForProp(Tile tile, TileArea area)
    {
        var edgeCount = 0;
        edgeCount += (tile.SideUp.Any(p => p.Value.Name == area.Name) && tile.Up == null) ? 1 : 0;
        edgeCount +=
            (tile.SideRight.Any(p => p.Value.Name == area.Name) && tile.Right == null) ? 1 : 0;
        edgeCount +=
            (tile.SideDown.Any(p => p.Value.Name == area.Name) && tile.Down == null) ? 1 : 0;
        edgeCount +=
            (tile.SideLeft.Any(p => p.Value.Name == area.Name) && tile.Left == null) ? 1 : 0;
        return edgeCount == 0;
    }
}
