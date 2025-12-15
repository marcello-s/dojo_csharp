#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataTubeMap;

[TestFixture]
public class DijkstraGraphAlgorithmFixture
{
    private IGraph<int, EdgeData> graph = null!;
    private IGraphAlgorithm<int, EdgeData> algorithm = null!;

    [SetUp]
    public void Setup()
    {
        graph = new Graph<int, EdgeData>();
        algorithm = new DijkstraGraphAlgorithm<int, EdgeData>();
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(graph, Is.InstanceOf<IGraph<int, EdgeData>>());
        Assert.That(algorithm, Is.InstanceOf<DijkstraGraphAlgorithm<int, EdgeData>>());
    }

    [Test]
    public void DijkstraExecuteTest()
    {
        LoadGraph(graph);
        PrintGraph(graph);
        var a = graph.Nodes.Single(n => n.Data.Equals(1));
        var b = graph.Nodes.Single(n => n.Data.Equals(5));
        Console.WriteLine(
            string.Format("source:{0} target:{1}", a.Data.ToString(), b.Data.ToString())
        );
        var path = algorithm.Execute(graph, a, b);
        PrintPath(path);
        Assert.That(path.ElementAt(0).Data, Is.EqualTo(1));
        Assert.That(path.ElementAt(1).Data, Is.EqualTo(3));
        Assert.That(path.ElementAt(2).Data, Is.EqualTo(6));
        Assert.That(path.ElementAt(3).Data, Is.EqualTo(5));
    }

    [Test]
    public void GraphNullTest()
    {
        var node = new GraphNode<int, EdgeData>(1);

        Assert.Throws<ArgumentNullException>(() => algorithm.Execute(null, node, node));
    }

    [Test]
    public void SourceNullTest()
    {
        var node1 = new GraphNode<int, EdgeData>(1);
        graph.Add(node1);

        Assert.Throws<ArgumentNullException>(() => algorithm.Execute(graph, null, node1));
    }

    [Test]
    public void TargetNullTest()
    {
        var node1 = new GraphNode<int, EdgeData>(1);
        graph.Add(node1);

        Assert.Throws<ArgumentNullException>(() => algorithm.Execute(graph, node1, null));
    }

    [Test]
    public void ZeroWeightOnEdgeTest()
    {
        var node1 = new GraphNode<int, EdgeData>(1);
        var node2 = new GraphNode<int, EdgeData>(2);
        graph.Add(node1);
        graph.Add(node2);
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(0),
                node1,
                node2,
                EdgeDirectionEnum.OmniDirectional
            )
        );

        Assert.Throws<InvalidOperationException>(() => algorithm.Execute(graph, node1, node2));
    }

    [Test]
    public void MassiveNodeTest()
    {
        LoadMassiveNodeGraph(graph);
        PrintGraph(graph);
        var source = graph.Nodes.First();
        var target = graph.Nodes.Last();
        Console.WriteLine(
            string.Format("source:{0} target:{1}", source.Data.ToString(), target.Data.ToString())
        );
        var path = algorithm.Execute(graph, source, target);
        PrintPath(path);
        Assert.That(path.First(), Is.EqualTo(source));
        Assert.That(path.Last(), Is.EqualTo(target));
    }

    internal class EdgeData : GraphEdgeData
    {
        public override double Weight { get; protected set; }

        public EdgeData(int weight)
        {
            Weight = weight;
        }
    }

    private static void LoadGraph(IGraph<int, EdgeData> graph)
    {
        var node1 = new GraphNode<int, EdgeData>(1);
        var node2 = new GraphNode<int, EdgeData>(2);
        var node3 = new GraphNode<int, EdgeData>(3);
        var node4 = new GraphNode<int, EdgeData>(4);
        var node5 = new GraphNode<int, EdgeData>(5);
        var node6 = new GraphNode<int, EdgeData>(6);
        graph.Add(node1);
        graph.Add(node2);
        graph.Add(node3);
        graph.Add(node4);
        graph.Add(node5);
        graph.Add(node6);
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(7),
                node1,
                node2,
                EdgeDirectionEnum.OmniDirectional
            )
        );
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(9),
                node1,
                node3,
                EdgeDirectionEnum.OmniDirectional
            )
        );
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(14),
                node1,
                node6,
                EdgeDirectionEnum.OmniDirectional
            )
        );
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(10),
                node2,
                node3,
                EdgeDirectionEnum.OmniDirectional
            )
        );
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(15),
                node2,
                node4,
                EdgeDirectionEnum.OmniDirectional
            )
        );
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(11),
                node3,
                node4,
                EdgeDirectionEnum.OmniDirectional
            )
        );
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(2),
                node3,
                node6,
                EdgeDirectionEnum.OmniDirectional
            )
        );
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(6),
                node4,
                node5,
                EdgeDirectionEnum.OmniDirectional
            )
        );
        graph.Add(
            new GraphEdge<int, EdgeData>(
                new EdgeData(9),
                node5,
                node6,
                EdgeDirectionEnum.OmniDirectional
            )
        );
    }

    private static void LoadMassiveNodeGraph(IGraph<int, EdgeData> graph)
    {
        var random = new Random();
        var nodeIndex = 0;
        List<GraphNode<int, EdgeData>> lastNodeList = null!;
        var source = new GraphNode<int, EdgeData>(nodeIndex++);
        graph.Add(source);
        for (var k = 0; k < 3; k++)
        {
            var nodeList = new List<GraphNode<int, EdgeData>>();
            for (var i = 0; i < 4; i++)
            {
                nodeList.Add(new GraphNode<int, EdgeData>(nodeIndex++));
            }

            foreach (var node in nodeList)
            {
                graph.Add(node);
            }

            if (k == 0)
            {
                foreach (var node in nodeList)
                {
                    graph.Add(
                        new GraphEdge<int, EdgeData>(
                            new EdgeData(random.Next(9) + 1),
                            source,
                            node,
                            EdgeDirectionEnum.OmniDirectional
                        )
                    );
                }
            }
            else
            {
                for (var j = 0; j < nodeList.Count(); j++)
                {
                    graph.Add(
                        new GraphEdge<int, EdgeData>(
                            new EdgeData(random.Next(9) + 1),
                            lastNodeList.ElementAt(j),
                            nodeList.ElementAt(j),
                            EdgeDirectionEnum.OmniDirectional
                        )
                    );
                }
            }

            lastNodeList = nodeList.ToList();
        }

        var target = new GraphNode<int, EdgeData>(nodeIndex++);
        graph.Add(target);
        foreach (var node in lastNodeList)
        {
            graph.Add(
                new GraphEdge<int, EdgeData>(
                    new EdgeData(random.Next(9) + 1),
                    node,
                    target,
                    EdgeDirectionEnum.OmniDirectional
                )
            );
        }
    }

    private static void PrintGraph(IGraph<int, EdgeData> graph)
    {
        Console.WriteLine("NODES");
        Console.WriteLine("=====");
        foreach (var node in graph.Nodes)
        {
            Console.WriteLine("'{0}'", node.Data.ToString());
        }

        Console.WriteLine("EDGES");
        Console.WriteLine("=====");
        foreach (var edge in graph.Edges)
        {
            Console.WriteLine(
                "'{0}'-'{1}', {2}",
                edge.FirstNode.Data.ToString(),
                edge.SecondNode.Data.ToString(),
                edge.Data.Weight
            );
        }
    }

    private static void PrintPath(IEnumerable<GraphNode<int, EdgeData>> path)
    {
        var aPath = path.Select(n => n.Data).ToArray();
        Console.WriteLine("path:{0}", string.Join("-", aPath));
    }
}
