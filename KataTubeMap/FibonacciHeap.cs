#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Text;

namespace KataTubeMap;

public class FibonacciHeap<TData, TKey>
    where TKey : IComparable
{
    public int Count { get; protected set; }
    public HeapNode<TData, TKey>? Min { get; protected set; }

    public HeapNode<TData, TKey> Insert(TData? data, TKey key)
    {
        var node = new HeapNode<TData, TKey>(data, key);
        if (Min != null)
        {
            node.Right = Min;
            node.Left = Min.Left;
            Min.Left = node;
            node.Left.Right = node;
            if (key.CompareTo(Min.Key) < 0)
            {
                Min = node;
            }
        }
        else
        {
            Min = node;
        }

        Count++;
        return node;
    }

    public HeapNode<TData, TKey>? Remove(HeapNode<TData, TKey>? node)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        if (Min != null)
        {
            DecreaseKey(node, Min.Key, true);
        }

        return ExtractMin();
    }

    public HeapNode<TData, TKey>? ExtractMin()
    {
        if (Min == null)
        {
            return null;
        }

        var min = Min;
        if (min.Child != null)
        {
            min.Child.Parent = null;
            // clear parent of all children of Min
            var current = min.Child;
            while (!current.Right.Equals(min.Child))
            {
                current = current.Right;
                current.Parent = null;
            }
            // merge children into root list
            var MinLeft = Min.Left;
            var minChildLeft = min.Child.Left;
            Min.Left = minChildLeft;
            minChildLeft.Right = Min;
            min.Child.Left = MinLeft;
            MinLeft.Right = min.Child;
        }
        // remove min from heap
        min.Left.Right = min.Right;
        min.Right.Left = min.Left;
        if (min.Equals(min.Right))
        {
            Min = null;
        }
        else
        {
            Min = min.Right;
            Consolidate();
        }

        Count--;
        return min;
    }

    private void Consolidate()
    {
        // 45 comes from log base phi of int.MaxValue
        var aNodes = new HeapNode<TData, TKey>?[Count];
        var start = Min;
        var current = Min;
        if (current == null)
        {
            return;
        }

        do
        {
            var currentCopy = current;
            var next = current.Right;
            var d = currentCopy.Degree;
            while (aNodes[d] != null)
            {
                // make one of the nodes a child of the other
                var node = aNodes[d];
                if (currentCopy.Key.CompareTo(node!.Key) > 0)
                {
                    var temp = node;
                    node = currentCopy;
                    currentCopy = temp;
                }

                if (node.Equals(start))
                {
                    start = start.Right;
                }

                if (node.Equals(next))
                {
                    next = next.Right;
                }

                Link(node, currentCopy);
                aNodes[d] = null;
                d++;
            }

            aNodes[d] = currentCopy;
            current = next;
        } while (!current.Equals(start));

        Min = null;
        foreach (var node in aNodes)
        {
            if (Min == null || (node != null && node.Key.CompareTo(Min.Key) < 0))
            {
                Min = node;
            }
        }
    }

    private void Link(HeapNode<TData, TKey> node1, HeapNode<TData, TKey> node2)
    {
        // remove node1 from circular list
        node1.Left.Right = node1.Right;
        node1.Right.Left = node1.Left;
        // make node1 a child of node2
        node1.Parent = node2;
        if (node2.Child == null)
        {
            node2.Child = node1;
            node1.Right = node1;
            node1.Left = node1;
        }
        else
        {
            node1.Left = node2.Child;
            node1.Right = node2.Child.Right;
            node2.Child.Right = node1;
            node1.Right.Left = node1;
        }

        // increase degree of node2
        node2.Degree++;
        // clear node1 mark
        node1.IsMarked = false;
    }

    public void DecreaseKey(HeapNode<TData, TKey>? node, TKey? key)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        if (key == null)
        {
            throw new ArgumentNullException("key");
        }

        DecreaseKey(node, key, false);
    }

    protected virtual void DecreaseKey(HeapNode<TData, TKey> node, TKey key, bool deleteNode)
    {
        if (!deleteNode && key.CompareTo(node.Key) > 0)
        {
            throw new ArgumentException("must be less than node.Key", "key");
        }

        node.Key = key;
        var parent = node.Parent;
        if (parent != null && (deleteNode || key.CompareTo(parent.Key) < 0))
        {
            Cut(node, parent);
            CascadingCut(parent);
        }

        if (deleteNode || (Min != null && key.CompareTo(Min.Key) < 0))
        {
            Min = node;
        }
    }

    protected virtual void Cut(HeapNode<TData, TKey> node1, HeapNode<TData, TKey> node2)
    {
        // remove node1
        node1.Left.Right = node1.Right;
        node1.Right.Left = node1.Left;
        // decrement node 2 degree
        node2.Degree--;
        // reset node2 child
        if (node2.Degree == 0)
        {
            node2.Child = null;
        }
        else if (node2.Child != null && node2.Child.Equals(node1))
        {
            node2.Child = node1.Right;
        }

        if (Min == null)
        {
            return;
        }

        // add node1 to root list
        node1.Right = Min;
        node1.Left = Min.Left;
        Min.Left = node1;
        node1.Left.Right = node1;
        // clear node1 parent
        node1.Parent = null;
        // clear mark
        node1.IsMarked = false;
    }

    protected virtual void CascadingCut(HeapNode<TData, TKey> node)
    {
        // if this node has a parent and this node is marked,
        // cut the node from the parent, then cascade up the tree
        // with the parent. Unmarked nodes are getting marked.
        var parent = node.Parent;
        if (parent != null)
        {
            if (node.IsMarked)
            {
                Cut(node, parent);
                CascadingCut(parent);
            }
            else
            {
                node.IsMarked = true;
            }
        }
    }

    public static FibonacciHeap<TData, TKey>? Merge(
        FibonacciHeap<TData, TKey> heap1,
        FibonacciHeap<TData, TKey> heap2
    )
    {
        if (heap1 == null && heap2 == null)
        {
            return null;
        }

        var heap = new FibonacciHeap<TData, TKey>();
        if (heap1 == null)
        {
            heap.Min = heap2.Min;
            heap.Count = heap2.Count;
            return heap;
        }
        if (heap2 == null)
        {
            heap.Min = heap1.Min;
            heap.Count = heap1.Count;
            return heap;
        }

        heap.Min = heap1.Min;
        if (heap.Min != null)
        {
            if (heap2.Min != null)
            {
                heap.Min.Right.Left = heap2.Min.Left;
                heap2.Min.Left.Right = heap.Min.Right;
                heap.Min.Right = heap2.Min;
                heap2.Min.Left = heap.Min;
                if (heap2.Min.Key.CompareTo(heap1.Min!.Key) < 0)
                {
                    heap.Min = heap2.Min;
                }
            }
        }
        else
        {
            heap.Min = heap2.Min;
        }

        heap.Count = heap1.Count + heap2.Count;
        return heap;
    }

    public override string ToString()
    {
        if (Min == null)
        {
            return "empty";
        }

        return RenderToString(Min);
    }

    protected static string RenderToString(HeapNode<TData, TKey> node)
    {
        var sb = new StringBuilder();
        var current = node;
        var stringList = new List<string?>();
        stringList.Add(current.Key.ToString());
        if (current.Child != null)
        {
            stringList.Add("{" + RenderToString(current.Child) + "}");
        }

        while (!current.Right.Equals(node))
        {
            current = current.Right;
            stringList.Add(current.Key.ToString());
            if (current.Child != null)
            {
                stringList.Add("{" + RenderToString(current.Child) + "}");
            }
        }

        sb.Append(string.Join("-", stringList.ToArray()));

        return sb.ToString();
    }
}
