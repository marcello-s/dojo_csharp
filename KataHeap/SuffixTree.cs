#region license and copyright
/*
The MIT License, Copyright (c) 2011-2025 Marcel Schneider
for details see License.txt
 */
#endregion

namespace KataHeap;

public class SuffixTree
{
    /*
     * Rule 1:
     * After an insertion from root and active_length > 0
     *  - active_node remains root
     *  - active_edge is set (shifted right) to the first character of the new suffix we need to insert
     *  - active_length is reduced by 1
     *
     * Rule 2:
     * If we split an edge an insert a new node, and if that is not the first node created
     * during the current step, we connect the previously inserted node and the new node
     * through a special pointer, a suffix link.
     * If we create a new internal node or make an insert from an internal node, and this is
     * not the first such internal node at the current step, then we link the previous such
     * node with this one through a suffix link.
     *
     * Rule 3:
     * After splitting an edge from an active_node that is not the root node, we follow the
     * suffix link going out of that node, if there is any, and reset the active_node to
     * the node it points to. If there is no suffix link, we set the active_node to the root.
     * Active_edge and active_length remain unchanged.
     * After an insert from the active_node which is not the root node, we must follow the
     * suffix link and set the active_node to the node it points to. If there is no suffix
     * link, set the active_node to the root node. Active_edge and active_length stay unchanged.
     *
     * Observation 1:
     * When the final suffix we need to insert is found to exist in the tree already, the
     * tree itself is not changed at all (we only update the active point and the remainder).
     *
     * Observation 2:
     * If at some point active_length is greater or equal to the lenght of the current edge
     * (edge_length), we move our active point down until edge_length is not strictly greater
     * than active_length.
     *
     * Observation 3:
     * When the symbol we want to add to the tree is already on the edge, we, according to
     * observation 1, update only active point and remainder, leaving the tree unchanged.
     * But if there is an internal node marked as needing suffix link, we must connect that node
     * with our current active_node through a suffix link.
     */

    private const int OO = int.MaxValue / 2;
    private int position;
    private readonly SuffixTreeNode root;
    private readonly IList<char> text;

    // active point
    private SuffixTreeNode activeNode;
    private int activeEdge;
    private int activeLength;

    // remaining nodes to insert
    private int remainder;
    private SuffixTreeNode? suffixLinkNode;

    public SuffixTree()
    {
        text = new List<char>();

        activeEdge = 0;
        activeLength = 0;
        position = -1;

        root = new SuffixTreeNode(position, position, '\0');
        activeNode = root;
    }

    public void Add(char edge)
    {
        text.Add(edge);
        position++;
        suffixLinkNode = null;
        remainder++;

        while (remainder > 0)
        {
            if (activeLength == 0)
            {
                activeEdge = position;
            }

            if (!activeNode.Children.ContainsKey(text[activeEdge]))
            {
                var node = new SuffixTreeNode(position, OO, text[activeEdge]);
                activeNode.AddChildNode(node.Edge, node);
                AddSuffixLink(activeNode); // rule 2
            }
            else
            {
                var next = activeNode.Children[text[activeEdge]].First();
                if (Walkdown(next))
                {
                    // observation 2
                    continue;
                }

                if (text[next.Begin + activeLength] == edge)
                {
                    // observation 1
                    activeLength++;
                    // observation 3
                    AddSuffixLink(activeNode);
                    break;
                }

                var splitNode = new SuffixTreeNode(
                    next.Begin,
                    next.Begin + activeLength,
                    text[activeEdge]
                );
                activeNode.AddChildNode(splitNode.Edge, splitNode);
                var leaf = new SuffixTreeNode(position, OO, edge);
                splitNode.AddChildNode(edge, leaf);
                next.SetBegin(next.Begin + activeLength);
                splitNode.AddChildNode(text[next.Begin], next);
                // rule 2
                AddSuffixLink(splitNode);
            }

            if (activeNode == root && activeLength > 0)
            {
                // rule 1
                activeLength--;
                activeEdge = position - remainder + 1;
            }
            else
            {
                // rule 3
                activeNode = activeNode.Link ?? root;
            }

            remainder--;
        }
    }

    private void AddSuffixLink(SuffixTreeNode node)
    {
        if (suffixLinkNode != null)
        {
            suffixLinkNode.Link = node;
        }

        suffixLinkNode = node;
    }

    private bool Walkdown(SuffixTreeNode node)
    {
        var edgeLength = node.EdgeLength(position);
        if (activeLength >= edgeLength)
        {
            activeEdge += edgeLength;
            activeLength -= edgeLength;
            activeNode = node;
            return true;
        }

        return false;
    }
}
