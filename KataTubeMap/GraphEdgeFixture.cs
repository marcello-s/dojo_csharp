#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataTubeMap;

[TestFixture]
public class GraphEdgeFixture
{
    private GraphNode<string, int> firstNode = null!;
    private GraphNode<string, int> secondNode = null!;
    private GraphEdge<string, int> edge = null!;

    [SetUp]
    public void Setup()
    {
        firstNode = new GraphNode<string, int>("first");
        secondNode = new GraphNode<string, int>("second");
    }

    [Test]
    public void InstanceTest()
    {
        edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );
        Assert.That(edge, Is.InstanceOf<GraphEdge<string, int>>());
    }

    [Test]
    public void InstanceFirstNodeNullTest()
    {
        Assert.Throws<ArgumentNullException>(() =>
            edge = new GraphEdge<string, int>(
                1,
                null,
                secondNode,
                EdgeDirectionEnum.OmniDirectional
            )
        );
    }

    [Test]
    public void InstanceSecondNodeNullTest()
    {
        Assert.Throws<ArgumentNullException>(() =>
            edge = new GraphEdge<string, int>(1, firstNode, null, EdgeDirectionEnum.OmniDirectional)
        );
    }
}
