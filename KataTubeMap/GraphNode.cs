#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public class GraphNode<TNode, TEdge> : GraphElement<TNode, TEdge>
{
    public virtual TNode Data { get; protected set; }

    public GraphNode(TNode data)
    {
        Data = data;
    }
}
