#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataTubeMap;

[TestFixture]
public class GraphNodeFixture
{
    private GraphNode<string, int> node = null!;

    [SetUp]
    public void Setup()
    {
        node = new GraphNode<string, int>("test");
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(node, Is.InstanceOf<GraphNode<string, int>>());
    }
}
