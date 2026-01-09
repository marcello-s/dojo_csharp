#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataCarcassonne;

[TestFixture]
public class BoardAreaFixture
{
    private Board board = null!;

    [SetUp]
    public void Setup()
    {
        board = new Board();
    }

    [Test]
    public void DetectTwoTilesClosed()
    {
        board.AddTile(0, 0, CreateTileCityRight());
        board.AddTile(1, 0, CreateTileCityLeft());

        var firstOrDefault = board
            .Tiles.Where(t => t.Item1.Equals(0) && t.Item2.Equals(0))
            .FirstOrDefault();
        if (firstOrDefault != null)
        {
            var tile = firstOrDefault.Item3;
            var area = tile.TileAreas.Where(p => p.Name.Equals("city")).First();
            Assert.That(Board.IsAreaClosed(board, tile, area), Is.True);
        }
    }

    [Test]
    public void DetectTwoTilesOpen()
    {
        board.AddTile(0, 0, CreateTileCityRight());
        board.AddTile(1, 0, CreateTileCityLeftRight());

        var firstOrDefault = board
            .Tiles.Where(t => t.Item1.Equals(0) && t.Item2.Equals(0))
            .FirstOrDefault();
        if (firstOrDefault != null)
        {
            var tile = firstOrDefault.Item3;
            var area = tile.TileAreas.Where(p => p.Name.Equals("city")).First();
            Assert.That(Board.IsAreaClosed(board, tile, area), Is.False);
        }
    }

    [Test]
    public void DetectTwoTilesPropNotConnected()
    {
        board.AddTile(0, 0, CreateTileCityLeft());
        board.AddTile(1, 0, CreateTileCityRight());

        var firstOrDefault = board
            .Tiles.Where(t => t.Item1.Equals(0) && t.Item2.Equals(0))
            .FirstOrDefault();
        if (firstOrDefault != null)
        {
            var tile = firstOrDefault.Item3;
            var area = tile.TileAreas.Where(p => p.Name.Equals("city")).First();
            Assert.That(Board.IsAreaClosed(board, tile, area), Is.False);
        }
    }

    [Test]
    public void DetectTwoTilesOnePropClosed()
    {
        board.AddTile(0, 0, CreateTileCityRight());
        board.AddTile(1, 0, CreateTileCityLeftRightNoJunction());

        var firstOrDefault = board
            .Tiles.Where(t => t.Item1.Equals(0) && t.Item2.Equals(0))
            .FirstOrDefault();
        if (firstOrDefault != null)
        {
            var tile = firstOrDefault.Item3;
            var area = tile.TileAreas.Where(p => p.Name.Equals("city")).First();
            Assert.That(Board.IsAreaClosed(board, tile, area), Is.False);
        }
    }

    [Test]
    public void DetectThreeTilesClosed()
    {
        board.AddTile(0, 0, CreateTileCityRight());
        board.AddTile(1, 0, CreateTileCityLeftRight());
        board.AddTile(2, 0, CreateTileCityLeft());

        var firstOrDefault = board
            .Tiles.Where(t => t.Item1.Equals(0) && t.Item2.Equals(0))
            .FirstOrDefault();
        if (firstOrDefault != null)
        {
            var tile = firstOrDefault.Item3;
            var area = tile.TileAreas.Where(p => p.Name.Equals("city")).First();
            Assert.That(Board.IsAreaClosed(board, tile, area), Is.True);
        }
    }

    private static Tile CreateTileCityRight()
    {
        var areaLawn = new Dictionary<int, TileArea>();
        areaLawn.Add(1, new TileArea("lawn"));
        var areaCity = new Dictionary<int, TileArea>();
        areaCity.Add(1, new TileArea("city"));
        var tile = new Tile();
        tile.AddAreas(areaLawn, DirectionEnum.Up);
        tile.AddAreas(areaLawn, DirectionEnum.Left);
        tile.AddAreas(areaLawn, DirectionEnum.Down);
        tile.AddAreas(areaCity, DirectionEnum.Right);
        return tile;
    }

    private static Tile CreateTileCityLeft()
    {
        var areaLawn = new Dictionary<int, TileArea>();
        areaLawn.Add(1, new TileArea("lawn"));
        var areaCity = new Dictionary<int, TileArea>();
        areaCity.Add(1, new TileArea("city"));
        var tile = new Tile();
        tile.AddAreas(areaLawn, DirectionEnum.Up);
        tile.AddAreas(areaLawn, DirectionEnum.Right);
        tile.AddAreas(areaLawn, DirectionEnum.Down);
        tile.AddAreas(areaCity, DirectionEnum.Left);
        return tile;
    }

    private static Tile CreateTileCityLeftRight()
    {
        var areaLawn = new Dictionary<int, TileArea>();
        areaLawn.Add(1, new TileArea("lawn"));
        var areaCity = new Dictionary<int, TileArea>();
        areaCity.Add(1, new TileArea("city"));
        var tile = new Tile();
        tile.AddAreas(areaLawn, DirectionEnum.Up);
        tile.AddAreas(areaLawn, DirectionEnum.Down);
        tile.AddAreas(areaCity, DirectionEnum.Left);
        tile.AddAreas(areaCity, DirectionEnum.Right);
        return tile;
    }

    private static Tile CreateTileCityLeftRightNoJunction()
    {
        var areaLawn = new Dictionary<int, TileArea>();
        areaLawn.Add(1, new TileArea("lawn"));
        var areaCity1 = new Dictionary<int, TileArea>();
        areaCity1.Add(1, new TileArea("city1"));
        var areaCity2 = new Dictionary<int, TileArea>();
        areaCity2.Add(1, new TileArea("city2"));
        var tile = new Tile();
        tile.AddAreas(areaLawn, DirectionEnum.Up);
        tile.AddAreas(areaLawn, DirectionEnum.Down);
        tile.AddAreas(areaCity1, DirectionEnum.Left);
        tile.AddAreas(areaCity2, DirectionEnum.Right);
        return tile;
    }
}
