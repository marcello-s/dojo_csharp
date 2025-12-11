#region license and copyright
/*
The MIT License, Copyright (c) 2011-2025 Marcel Schneider
for details see License.txt
 */
#endregion

namespace KataHeap;

public class SuffixTreeNode(int begin, int end, char edge)
{
    public int Begin { get; private set; } = begin;
    public int End { get; init; } = end;
    public char Edge { get; init; } = edge;
    public IDictionary<char, IList<SuffixTreeNode>> Children { get; private set; } =
        new Dictionary<char, IList<SuffixTreeNode>>();
    public SuffixTreeNode? Link { get; set; }

    public void AddChildNode(char edge, SuffixTreeNode node)
    {
        if (Children.ContainsKey(edge))
        {
            var nodeList = Children[edge];
            nodeList.Add(node);
        }
        else
        {
            var nodeList = new List<SuffixTreeNode> { node };
            Children.Add(edge, nodeList);
        }
    }

    public int EdgeLength(int position)
    {
        return Math.Min(End, position + 1) - Begin;
    }

    public void SetBegin(int begin)
    {
        Begin = begin;
    }
}
