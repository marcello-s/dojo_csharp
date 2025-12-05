#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCarcassonne;

public class AreaTileMap
{
    public TileArea Area { get; private set; }
    public IList<Tile> Tiles { get; private set; }

    public AreaTileMap(TileArea area)
    {
        Area = area;
        Tiles = new List<Tile>();
    }
}
