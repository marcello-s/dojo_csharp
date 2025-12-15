#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public interface IGraphAlgorithm<TNode, TEdge>
{
    IEnumerable<GraphNode<TNode, TEdge>> Execute(
        IGraph<TNode, TEdge>? graph,
        GraphNode<TNode, TEdge>? a,
        GraphNode<TNode, TEdge>? b
    );
}
