#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataHeap;

public class LinkedList<T>
{
    private ListNode<T>? root;
    private ListNode<T>? tail;

    public int Count { get; private set; }

    public void Add(ListNode<T>? node)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        if (root == null && tail == null)
        {
            root = node;
            tail = node;
            Count++;
            return;
        }

        if (tail != null)
        {
            tail.Next = node;
            tail = node;
            tail.Next = null;
            Count++;
        }
    }

    public void InsertAt(ListNode<T>? positionNode, ListNode<T>? node)
    {
        if (positionNode == null)
        {
            throw new ArgumentNullException("positionNode");
        }
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        if (!Contains(positionNode))
        {
            throw new ArgumentException("List does not contain node.", "positionNode");
        }

        if (positionNode == tail)
        {
            Add(node);
            return;
        }

        var temp = positionNode.Next;
        positionNode.Next = node;
        node.Next = temp;
        Count++;
    }

    public void Remove(ListNode<T>? node)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        if (!Contains(node))
        {
            throw new ArgumentException("List does not contain node.", "node");
        }

        if (root == tail)
        {
            root = null;
            tail = null;
            Count--;
            return;
        }

        if (root == node)
        {
            root = root.Next;
            Count--;
            return;
        }

        var previousNode = Traverse(root, n => n.Next == node);
        if (previousNode != null)
        {
            previousNode.Next = node.Next;
        }

        if (tail == node)
        {
            tail = previousNode;
        }

        Count--;
    }

    public IEnumerable<ListNode<T>> Find(T key)
    {
        if (Equals(key, default(T)))
        {
            throw new ArgumentNullException("key");
        }

        var result = new List<ListNode<T>>();
        Traverse(
            root,
            n =>
            {
                if (n.Key != null && n.Key.Equals(key))
                {
                    result.Add(n);
                }
            }
        );
        return result;
    }

    private static void Traverse(ListNode<T>? node, Action<ListNode<T>> action)
    {
        if (node == null)
        {
            return;
        }

        action(node);
        Traverse(node.Next, action);
    }

    private static ListNode<T>? Traverse(ListNode<T>? node, Func<ListNode<T>, bool> func)
    {
        if (node == null)
        {
            return null;
        }

        return func(node) ? node : Traverse(node.Next, func);
    }

    private bool Contains(ListNode<T> node)
    {
        var containedNode = Traverse(root, n => n == node);
        return containedNode != null;
    }
}
