#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataTubeMap;

[TestFixture]
public class GraphFixture
{
    private Graph<string, int> graph = null!;

    [SetUp]
    public void Setup()
    {
        graph = new Graph<string, int>();
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(graph, Is.InstanceOf<Graph<string, int>>());
    }

    [Test]
    public void AddNodeTest()
    {
        var node = new GraphNode<string, int>("test");
        graph.Add(node);
        Assert.That(graph.Nodes.Contains(node), Is.True);
    }

    [Test]
    public void AddNullNodeTest()
    {
        GraphNode<string, int> node = null!;

        Assert.Throws<ArgumentNullException>(() => graph.Add(node));
    }

    [Test]
    public void AddSameNodeTwiceTest()
    {
        var node = new GraphNode<string, int>("test");
        graph.Add(node);

        Assert.Throws<ArgumentException>(() => graph.Add(node));
    }

    [Test]
    public void AddEdgeTest()
    {
        var firstNode = new GraphNode<string, int>("first");
        var secondNode = new GraphNode<string, int>("second");
        graph.Add(firstNode);
        graph.Add(secondNode);
        var edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );
        graph.Add(edge);
        Assert.That(graph.Edges.Contains(edge), Is.True);
    }

    [Test]
    public void AddNullEdgeTest()
    {
        GraphEdge<string, int> edge = null!;

        Assert.Throws<ArgumentNullException>(() => graph.Add(edge));
    }

    [Test]
    public void AddEdgeFirstNodeMissingTest()
    {
        var firstNode = new GraphNode<string, int>("first");
        var secondNode = new GraphNode<string, int>("second");
        graph.Add(secondNode);
        var edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );

        Assert.Throws<ArgumentException>(() => graph.Add(edge));
    }

    [Test]
    public void AddEdgeSecondNodeMissingTest()
    {
        var firstNode = new GraphNode<string, int>("first");
        var secondNode = new GraphNode<string, int>("second");
        graph.Add(firstNode);
        var edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );

        Assert.Throws<ArgumentException>(() => graph.Add(edge));
    }

    [Test]
    public void AddSameEdgeTwiceTest()
    {
        var firstNode = new GraphNode<string, int>("first");
        var secondNode = new GraphNode<string, int>("second");
        graph.Add(firstNode);
        graph.Add(secondNode);
        var edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );
        graph.Add(edge);

        Assert.Throws<ArgumentException>(() => graph.Add(edge));
    }

    [Test]
    public void RemoveNodeTest()
    {
        var firstNode = new GraphNode<string, int>("first");
        var secondNode = new GraphNode<string, int>("second");
        graph.Add(firstNode);
        graph.Add(secondNode);
        var edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );
        graph.Add(edge);
        Assert.That(graph.Edges.Contains(edge), Is.True);

        graph.Remove(firstNode);
        Assert.That(graph.Nodes.Contains(firstNode), Is.False);
        Assert.That(graph.Edges.Contains(edge), Is.False);
    }

    [Test]
    public void RemoveNullNodeTest()
    {
        var firstNode = new GraphNode<string, int>("first");
        var secondNode = new GraphNode<string, int>("second");
        graph.Add(firstNode);
        graph.Add(secondNode);
        var edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );
        graph.Add(edge);
        firstNode = null!;

        Assert.Throws<ArgumentNullException>(() => graph.Remove(firstNode));
    }

    [Test]
    public void RemoveEdgeTest()
    {
        var firstNode = new GraphNode<string, int>("first");
        var secondNode = new GraphNode<string, int>("second");
        graph.Add(firstNode);
        graph.Add(secondNode);
        var edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );
        graph.Add(edge);
        Assert.That(graph.Edges.Contains(edge), Is.True);

        graph.Remove(edge);
        Assert.That(graph.Edges.Contains(edge), Is.False);
    }

    [Test]
    public void RemoveNullEdgeTest()
    {
        var firstNode = new GraphNode<string, int>("first");
        var secondNode = new GraphNode<string, int>("second");
        graph.Add(firstNode);
        graph.Add(secondNode);
        var edge = new GraphEdge<string, int>(
            1,
            firstNode,
            secondNode,
            EdgeDirectionEnum.OmniDirectional
        );
        graph.Add(edge);
        Assert.That(graph.Edges.Contains(edge), Is.True);
        edge = null!;

        Assert.Throws<ArgumentNullException>(() => graph.Remove(edge));
    }
}
