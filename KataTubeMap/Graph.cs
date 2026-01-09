#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public class Graph<TNode, TEdge> : IGraph<TNode, TEdge>
{
    protected readonly HashSet<GraphNode<TNode, TEdge>> _nodes;
    protected readonly HashSet<GraphEdge<TNode, TEdge>> _edges;

    public Graph()
    {
        _nodes = new HashSet<GraphNode<TNode, TEdge>>();
        _edges = new HashSet<GraphEdge<TNode, TEdge>>();
    }

    public virtual IEnumerable<GraphNode<TNode, TEdge>> Nodes
    {
        get { return _nodes; }
    }

    public virtual IEnumerable<GraphEdge<TNode, TEdge>> Edges
    {
        get { return _edges; }
    }

    public virtual void Add(GraphNode<TNode, TEdge> node)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        if (_nodes.Contains(node))
        {
            throw new ArgumentException("added already", "node");
        }

        node.Graph = this;
        _nodes.Add(node);
    }

    public virtual void Add(GraphEdge<TNode, TEdge> edge)
    {
        if (edge == null)
        {
            throw new ArgumentNullException("edge");
        }

        if (!Nodes.Contains(edge.FirstNode))
        {
            throw new ArgumentException("graph does not contain edge FirstNode", "edge");
        }

        if (!Nodes.Contains(edge.SecondNode))
        {
            throw new ArgumentException("graph does not contain edge SecondNode", "edge");
        }

        if (_edges.Contains(edge))
        {
            throw new ArgumentException("added already", "edge");
        }

        edge.Graph = this;
        _edges.Add(edge);
    }

    public virtual void Remove(GraphNode<TNode, TEdge> node)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        _edges.RemoveWhere(e => e.FirstNode.Equals(node) || e.SecondNode.Equals(node));
        if (_nodes.Contains(node))
        {
            _nodes.Remove(node);
        }
    }

    public virtual void Remove(GraphEdge<TNode, TEdge> edge)
    {
        if (edge == null)
        {
            throw new ArgumentNullException("edge");
        }

        if (_edges.Contains(edge))
        {
            _edges.Remove(edge);
        }
    }

    public virtual IEnumerable<GraphEdge<TNode, TEdge>> GetEdges(GraphNode<TNode, TEdge> node)
    {
        return _edges.Where(edge => edge.FirstNode.Equals(node) || edge.SecondNode.Equals(node));
    }

    //public virtual IEnumerable<GraphEdge<TNode, TEdge>> GetOutgoingEdges(GraphNode<TNode, TEdge> node)
    //{
    //    return _edges.Where(edge => edge.FirstNode.Equals(node));
    //}
}
