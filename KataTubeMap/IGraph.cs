#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public interface IGraph<TNode, TEdge>
{
    IEnumerable<GraphNode<TNode, TEdge>> Nodes { get; }
    IEnumerable<GraphEdge<TNode, TEdge>> Edges { get; }

    void Add(GraphNode<TNode, TEdge> node);
    void Add(GraphEdge<TNode, TEdge> edge);
    void Remove(GraphNode<TNode, TEdge> node);
    void Remove(GraphEdge<TNode, TEdge> edge);

    IEnumerable<GraphEdge<TNode, TEdge>> GetEdges(GraphNode<TNode, TEdge> node);
    //IEnumerable<GraphEdge<TNode, TEdge>> GetOutgoingEdges(GraphNode<TNode, TEdge> node);
    //IEnumerable<GraphEdge<TNode, TEdge>> GetIncomingEdges(GraphNode<TNode, TEdge> node);
}
