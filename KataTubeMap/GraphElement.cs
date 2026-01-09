#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public abstract class GraphElement<TNode, TEdge>
{
    protected IGraph<TNode, TEdge> graph = null!;

    protected internal IGraph<TNode, TEdge> Graph
    {
        get
        {
            if (graph == null)
            {
                throw new InvalidOperationException(
                    "Element not connected to IGraph<TNode, TEdge> instance"
                );
            }

            return graph;
        }
        set { graph = value; }
    }
}
