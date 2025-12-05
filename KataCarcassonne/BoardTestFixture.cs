#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataCarcassonne;

[TestFixture]
public class BoardTestFixture
{
    private Board board = null!;

    [SetUp]
    public void Setup()
    {
        board = new Board();
    }

    [Test]
    public void TestInstance()
    {
        Assert.That(board, Is.InstanceOf<Board>());
    }

    [Test]
    public void TestAddTileSingle()
    {
        var areaUp = new Dictionary<int, TileArea>();
        areaUp.Add(1, new TileArea("1"));
        areaUp.Add(2, new TileArea("2"));
        var areaRight = new Dictionary<int, TileArea>();
        areaRight.Add(1, new TileArea("1"));
        var tile = new Tile();
        tile.AddAreas(areaUp, DirectionEnum.Up);
        tile.AddAreas(areaRight, DirectionEnum.Right);
        board.AddTile(0, 0, tile);
        Assert.That(board.Tiles.Count(), Is.EqualTo(1));
        Assert.That(board.AreaTileMaps.Count(), Is.EqualTo(2));
    }

    [Test]
    public void TestAddTileSameCoordinates()
    {
        var tile1 = new Tile();
        board.AddTile(0, 0, tile1);
        board.AddTile(0, 0, tile1);
        Assert.That(board.Tiles.Count(), Is.EqualTo(1));
    }

    [Test]
    public void TestAddTileTwo()
    {
        var areaUp1 = new Dictionary<int, TileArea>();
        areaUp1.Add(1, new TileArea("1"));
        areaUp1.Add(2, new TileArea("2"));
        var areaRight1 = new Dictionary<int, TileArea>();
        areaRight1.Add(1, new TileArea("1"));
        var tile1 = new Tile();
        tile1.AddAreas(areaUp1, DirectionEnum.Up);
        tile1.AddAreas(areaRight1, DirectionEnum.Right);
        board.AddTile(0, 0, tile1);

        var areaUp2 = new Dictionary<int, TileArea>();
        areaUp2.Add(1, new TileArea("2"));
        areaUp2.Add(2, new TileArea("1"));
        var areaLeft2 = new Dictionary<int, TileArea>();
        areaLeft2.Add(1, new TileArea("1"));
        var tile2 = new Tile();
        tile2.AddAreas(areaUp2, DirectionEnum.Up);
        tile2.AddAreas(areaLeft2, DirectionEnum.Left);
        board.AddTile(1, 0, tile2);

        Assert.That(board.Tiles.Count(), Is.EqualTo(2));
        Assert.That(board.AreaTileMaps.Count(), Is.EqualTo(2));
    }
}
