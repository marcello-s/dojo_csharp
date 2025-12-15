#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public class GraphEdge<TNode, TEdge> : GraphElement<TNode, TEdge>
{
    public virtual TEdge Data { get; protected set; }
    public EdgeDirectionEnum EdgeDirection { get; protected set; }
    public GraphNode<TNode, TEdge> FirstNode { get; protected set; }
    public GraphNode<TNode, TEdge> SecondNode { get; protected set; }

    public GraphEdge(
        TEdge data,
        GraphNode<TNode, TEdge>? firstNode,
        GraphNode<TNode, TEdge>? secondNode,
        EdgeDirectionEnum edgeDirection
    )
    {
        if (firstNode == null)
        {
            throw new ArgumentNullException("firstNode");
        }

        if (secondNode == null)
        {
            throw new ArgumentNullException("secondNode");
        }

        Data = data;
        EdgeDirection = edgeDirection;
        FirstNode = firstNode;
        SecondNode = secondNode;
    }
}
