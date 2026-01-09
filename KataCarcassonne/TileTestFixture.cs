#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataCarcassonne;

[TestFixture]
public class TileTestFixture
{
    private Tile tile = null!;

    [SetUp]
    public void Setup()
    {
        tile = new Tile();
    }

    [Test]
    public void TestTileInstance()
    {
        Assert.That(tile, Is.InstanceOf<Tile>());
    }

    [Test]
    public void TestTileDirection()
    {
        Assert.That(tile.Direction, Is.EqualTo(DirectionEnum.Up));
    }

    [Test]
    public void TestSetNeighbourUp()
    {
        var tile = new Tile();
        Assert.That(this.tile.IsConnected, Is.False);
        Tile.SetNeighbour(this.tile, tile, DirectionEnum.Up);
        Assert.That(this.tile.Up, Is.EqualTo(tile));
        Assert.That(tile.Down, Is.EqualTo(this.tile));
        Assert.That(this.tile.IsConnected, Is.True);
    }

    [Test]
    public void TestSetNeighbourRight()
    {
        var tile = new Tile();
        Assert.That(this.tile.IsConnected, Is.False);
        Tile.SetNeighbour(this.tile, tile, DirectionEnum.Right);
        Assert.That(this.tile.Right, Is.EqualTo(tile));
        Assert.That(tile.Left, Is.EqualTo(this.tile));
        Assert.That(this.tile.IsConnected, Is.True);
    }

    [Test]
    public void TestSetNeighbourDown()
    {
        var tile = new Tile();
        Assert.That(this.tile.IsConnected, Is.False);
        Tile.SetNeighbour(this.tile, tile, DirectionEnum.Down);
        Assert.That(this.tile.Down, Is.EqualTo(tile));
        Assert.That(tile.Up, Is.EqualTo(this.tile));
        Assert.That(this.tile.IsConnected, Is.True);
    }

    [Test]
    public void TestSetNeighbourLeft()
    {
        var tile = new Tile();
        Assert.That(this.tile.IsConnected, Is.False);
        Tile.SetNeighbour(this.tile, tile, DirectionEnum.Left);
        Assert.That(this.tile.Left, Is.EqualTo(tile));
        Assert.That(tile.Right, Is.EqualTo(this.tile));
        Assert.That(this.tile.IsConnected, Is.True);
    }

    [Test]
    public void TestAddPropertiesUp()
    {
        var areas = new Dictionary<int, TileArea>();
        areas.Add(1, new TileArea("1"));
        areas.Add(2, new TileArea("2"));
        tile.AddAreas(areas, DirectionEnum.Up);
        Assert.That(tile.SideUp, Is.EqualTo(areas));
        var propsList = areas.Select(p => p.Value).ToList();
        Assert.That(tile.TileAreas, Is.EqualTo(propsList));
    }

    [Test]
    public void TestAddPropertiesUpRight()
    {
        var areaUp = new Dictionary<int, TileArea>();
        areaUp.Add(1, new TileArea("1"));
        areaUp.Add(2, new TileArea("2"));
        var areaRight = new Dictionary<int, TileArea>();
        areaRight.Add(1, new TileArea("1"));
        tile.AddAreas(areaUp, DirectionEnum.Up);
        Assert.That(tile.SideUp, Is.EqualTo(areaUp));
        tile.AddAreas(areaRight, DirectionEnum.Right);
        Assert.That(tile.SideRight, Is.EqualTo(areaRight));
        var areaList = areaUp.Select(p => p.Value).ToList();
        Assert.That(tile.TileAreas, Is.EqualTo(areaList));
    }

    [Test]
    public void TestAddPropertiesUpRightRotate()
    {
        var areaUp = new Dictionary<int, TileArea>();
        areaUp.Add(1, new TileArea("1"));
        areaUp.Add(2, new TileArea("2"));
        var areaRight = new Dictionary<int, TileArea>();
        areaRight.Add(1, new TileArea("1"));
        tile.AddAreas(areaUp, DirectionEnum.Up);
        tile.AddAreas(areaRight, DirectionEnum.Right);
        tile.Rotate(DirectionEnum.Right);
        Assert.That(tile.Direction, Is.EqualTo(DirectionEnum.Right));
        Assert.That(tile.SideRight, Is.EqualTo(areaUp));
        Assert.That(tile.SideDown, Is.EqualTo(areaRight));
        var areaList = areaUp.Select(p => p.Value).ToList();
        Assert.That(tile.TileAreas, Is.EqualTo(areaList));
    }

    [Test]
    public void TestIsNeighbourMatchUpDownTrue()
    {
        // prepare tile 1
        var areaUp = new Dictionary<int, TileArea>();
        areaUp.Add(1, new TileArea("1"));
        areaUp.Add(2, new TileArea("2"));
        this.tile.AddAreas(areaUp, DirectionEnum.Up);

        // prepare tile 2
        var tile = new Tile();
        var areaDown = new Dictionary<int, TileArea>();
        areaDown.Add(1, new TileArea("2"));
        areaDown.Add(2, new TileArea("11"));
        tile.AddAreas(areaDown, DirectionEnum.Down);

        Assert.That(Tile.IsNeighbourMatch(this.tile, tile, DirectionEnum.Up), Is.True);
    }

    [Test]
    public void TestIsNeighbourMatchUpDownFalse()
    {
        // prepare tile 1
        var areaUp = new Dictionary<int, TileArea>();
        areaUp.Add(1, new TileArea("1"));
        areaUp.Add(2, new TileArea("2"));
        this.tile.AddAreas(areaUp, DirectionEnum.Up);

        // prepare tile 2
        var tile = new Tile();
        var areaDown = new Dictionary<int, TileArea>();
        areaDown.Add(1, new TileArea("1"));
        areaDown.Add(2, new TileArea("2"));
        tile.AddAreas(areaDown, DirectionEnum.Down);

        Assert.That(Tile.IsNeighbourMatch(this.tile, tile, DirectionEnum.Up), Is.False);
    }

    [Test]
    public void TestIsNeighbourMatchRightLeftTrue()
    {
        // prepare tile 1
        var areaRight = new Dictionary<int, TileArea>();
        areaRight.Add(1, new TileArea("1"));
        areaRight.Add(2, new TileArea("2"));
        this.tile.AddAreas(areaRight, DirectionEnum.Right);

        // prepare tile 2
        var tile = new Tile();
        var areaLeft = new Dictionary<int, TileArea>();
        areaLeft.Add(1, new TileArea("2"));
        areaLeft.Add(2, new TileArea("1"));
        tile.AddAreas(areaLeft, DirectionEnum.Left);

        Assert.That(Tile.IsNeighbourMatch(this.tile, tile, DirectionEnum.Right), Is.True);
    }

    [Test]
    public void TestIsPropertyEndpointTrue()
    {
        var areaUp = new Dictionary<int, TileArea>();
        areaUp.Add(1, new TileArea("1"));
        areaUp.Add(2, new TileArea("2"));
        tile.AddAreas(areaUp, DirectionEnum.Up);

        Assert.That(Tile.IsAreaEndpoint(tile, areaUp[1]), Is.True);
    }

    [Test]
    public void TestIsPropertyEndpointFalse()
    {
        var areaUp = new Dictionary<int, TileArea>();
        areaUp.Add(1, new TileArea("1"));
        areaUp.Add(2, new TileArea("2"));
        tile.AddAreas(areaUp, DirectionEnum.Up);

        var areaRight = new Dictionary<int, TileArea>();
        areaRight.Add(1, new TileArea("1"));
        tile.AddAreas(areaRight, DirectionEnum.Right);

        Assert.That(Tile.IsAreaEndpoint(tile, areaUp[1]), Is.False);
        Assert.That(Tile.IsAreaEndpoint(tile, areaUp[2]), Is.True);
    }
}
