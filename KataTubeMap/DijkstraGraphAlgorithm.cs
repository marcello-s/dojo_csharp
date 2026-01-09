#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

/// <summary>
/// http://de.wikipedia.org/wiki/Dijkstra-Algorithmus
/// http://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
/// </summary>
/// <typeparam name="TNode"></typeparam>
/// <typeparam name="TEdge"></typeparam>
public class DijkstraGraphAlgorithm<TNode, TEdge> : IGraphAlgorithm<TNode, TEdge>
    where TEdge : GraphEdgeData
{
    public IEnumerable<GraphNode<TNode, TEdge>> Execute(
        IGraph<TNode, TEdge>? graph,
        GraphNode<TNode, TEdge>? source,
        GraphNode<TNode, TEdge>? target
    )
    {
        //return Execute2(graph, source, target);

        // check for null instances
        if (graph == null)
        {
            throw new ArgumentNullException("graph");
        }

        if (source == null)
        {
            throw new ArgumentNullException("source");
        }

        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        // weight of edges must be positive
        if (graph.Edges.Any(edge => edge.Data.Weight <= 0.0))
        {
            throw new InvalidOperationException("all graph edges must have positive weight");
        }

        // create heap of unvisited nodes
        var heapNodes = new HashSet<HeapNode<GraphNode<TNode, TEdge>, double>>();
        var unvisitedNodesHeap = new FibonacciHeap<GraphNode<TNode, TEdge>, double>();
        HeapNode<GraphNode<TNode, TEdge>, double>? sourceHeapNode = null;
        foreach (var node in graph.Nodes)
        {
            var heapNode = unvisitedNodesHeap.Insert(node, double.PositiveInfinity);
            heapNodes.Add(heapNode);
            if (heapNode.Data.Equals(source))
            {
                sourceHeapNode = heapNode;
            }
        }

        if (sourceHeapNode != null)
        {
            unvisitedNodesHeap.DecreaseKey(sourceHeapNode, 0.0);
        }

        // initialize node/distance dictionary to node/infinity, source/zero
        var node2DistanceDictionary = graph.Nodes.ToDictionary(
            node => node,
            distance => double.PositiveInfinity
        );
        node2DistanceDictionary[source] = 0.0;
        // initialize predecessor dictionary
        var predecessorDictionary = graph.Nodes.ToDictionary(node => node, value => value = null!);

        // algorithm starts here
        while (unvisitedNodesHeap.Count > 0)
        {
            // remove this node
            var heapNode = unvisitedNodesHeap.ExtractMin();
            heapNodes.Remove(heapNode!);
            var node = heapNode!.Data;
            var weight = heapNode.Key;
            //Debug.WriteLine("node:{0}, weight:{1}", node.Data, heapNode.Key);

            // exit loop when target has been reached
            if (node.Equals(target))
            {
                break;
            }

            // for each neighbour, update distance to current node weight plus neighbour weight
            foreach (var edge in graph.GetEdges(node))
            {
                var targetNode = edge.FirstNode.Equals(heapNode.Data)
                    ? edge.SecondNode
                    : edge.FirstNode;
                if (heapNodes.Any(n => n.Data.Equals(targetNode)))
                {
                    var newWeight = weight + edge.Data.Weight;

                    // update to new distance
                    if (newWeight < node2DistanceDictionary[targetNode])
                    {
                        node2DistanceDictionary[targetNode] = newWeight;
                        var targetHeapNode = heapNodes
                            .Where(n => n.Data.Equals(targetNode))
                            .First();
                        unvisitedNodesHeap.DecreaseKey(targetHeapNode, newWeight);
                        predecessorDictionary[targetNode] = node;
                    }
                }
            }
        }

        // create shortest path
        var path = new List<GraphNode<TNode, TEdge>>();
        path.Add(target);
        var currentNode = target;
        while (predecessorDictionary[currentNode] != null)
        {
            currentNode = predecessorDictionary[currentNode];
            path.Insert(0, currentNode);
        }

        return path;
    }

    public IEnumerable<GraphNode<TNode, TEdge>> Execute2(
        IGraph<TNode, TEdge>? graph,
        GraphNode<TNode, TEdge>? source,
        GraphNode<TNode, TEdge>? target
    )
    {
        // check for null instances
        if (graph == null)
        {
            throw new ArgumentNullException("graph");
        }

        if (source == null)
        {
            throw new ArgumentNullException("source");
        }

        if (target == null)
        {
            throw new ArgumentNullException("target");
        }

        // weight of edges must be positive
        if (graph.Edges.Any(edge => edge.Data.Weight <= 0.0))
        {
            throw new InvalidOperationException("all graph edges must have positive weight");
        }

        // create list of unvisited nodes
        var unvisitedNodes = new HashSet<GraphNode<TNode, TEdge>>();
        foreach (var node in graph.Nodes)
        {
            unvisitedNodes.Add(node);
        }

        // initialize node/distance dictionary to node/infinity, a/zero
        var node2DistanceDictionary = graph.Nodes.ToDictionary(
            node => node,
            distance => double.PositiveInfinity
        );
        node2DistanceDictionary[source] = 0.0;
        // initialize predecessor dictionary
        var predecessorDictionary = graph.Nodes.ToDictionary(node => node, value => value = null!);

        // algorithm starts here
        while (unvisitedNodes.Any())
        {
            //Debug.WriteLine(string.Empty);
            var node = unvisitedNodes.OrderBy(n => node2DistanceDictionary[n]).FirstOrDefault();
            var weight = node2DistanceDictionary[node!];
            //Debug.WriteLine("node:{0}", node.Data);

            // remove this node
            unvisitedNodes.Remove(node!);

            // exit loop when target has been reached
            if (node!.Equals(target))
            {
                break;
            }

            // for each neighbour, update distance to current node weight plus neighbour weight
            foreach (var edge in graph.GetEdges(node))
            {
                var targetNode = edge.FirstNode.Equals(node) ? edge.SecondNode : edge.FirstNode;
                if (unvisitedNodes.Contains(targetNode))
                {
                    //Debug.WriteLine("targetNode:{0}", targetNode.Data);
                    var newWeight = weight + edge.Data.Weight;
                    //Debug.WriteLine("newWeight:{0}", newWeight);

                    // update to new distance
                    if (newWeight < node2DistanceDictionary[targetNode])
                    {
                        //Debug.WriteLine("update weight:{0} -> {1}", node2DistanceDictionary[targetNode], newWeight);
                        node2DistanceDictionary[targetNode] = newWeight;
                        predecessorDictionary[targetNode] = node;
                    }
                }
            }
        }

        // create shortest path
        var path = new List<GraphNode<TNode, TEdge>>();
        path.Add(target);
        var currentNode = target;
        while (predecessorDictionary[currentNode] != null)
        {
            currentNode = predecessorDictionary[currentNode];
            path.Insert(0, currentNode);
        }

        return path;
    }
}
