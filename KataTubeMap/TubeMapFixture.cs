#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataTubeMap;

[TestFixture]
public class TubeMapFixture
{
    private static string CurrentDirectory = TestContext.CurrentContext.TestDirectory;
    private Graph<TubeMapStation, TubeMapTrack> graph = null!;

    [SetUp]
    public void Setup()
    {
        graph = new Graph<TubeMapStation, TubeMapTrack>();
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(graph, Is.InstanceOf<Graph<TubeMapStation, TubeMapTrack>>());
    }

    [Test]
    public void LoadLinesTest()
    {
        var map = MapLoader.DeserializeFromFile(CurrentDirectory);
        foreach (var line in map.Lines)
        {
            AddTrainLineToGraph(
                graph,
                line.Stops.Select(s => new Tuple<int, string>(s.Duration, s.Name!)),
                line.Name!
            );
        }

        PrintGraph(graph);
    }

    private static void PrintGraph(IGraph<TubeMapStation, TubeMapTrack> graph)
    {
        Console.WriteLine("NODES");
        Console.WriteLine("=====");
        foreach (var node in graph.Nodes)
        {
            Console.WriteLine("'{0}'", node.Data.Name);
        }

        Console.WriteLine("EDGES");
        Console.WriteLine("=====");
        foreach (var edge in graph.Edges)
        {
            Console.WriteLine(
                "'{0}'-'{1}', {2}",
                edge.FirstNode.Data.Name,
                edge.SecondNode.Data.Name,
                edge.Data.Weight
            );
        }
    }

    private static void AddTrainLineToGraph(
        IGraph<TubeMapStation, TubeMapTrack> graph,
        IEnumerable<Tuple<int, string>> line,
        string lineName
    )
    {
        GraphNode<TubeMapStation, TubeMapTrack> lastNode = null!;
        foreach (var station in line)
        {
            TubeMapStation newStation;
            var currentNode = graph.Nodes.FirstOrDefault(node =>
                !string.IsNullOrEmpty(node.Data.Name) && node.Data.Name.Equals(station.Item2)
            );
            if (currentNode == null)
            {
                newStation = new TubeMapStation { Name = station.Item2 };
                currentNode = new GraphNode<TubeMapStation, TubeMapTrack>(newStation);
                graph.Add(currentNode);
                if (lastNode == null)
                {
                    lastNode = currentNode;
                    continue;
                }
            }

            var track = new TubeMapTrack(station.Item1);
            track.Lines.Add(new Tuple<int, string>(0, lineName));
            var edge = new GraphEdge<TubeMapStation, TubeMapTrack>(
                track,
                lastNode,
                currentNode,
                EdgeDirectionEnum.OmniDirectional
            );
            graph.Add(edge);
            lastNode = currentNode;
        }
    }
}
