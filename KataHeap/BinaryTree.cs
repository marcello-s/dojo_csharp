#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataHeap;

public class BinaryTree<T>
{
    private BinaryTreeNode<T>? root;

    public int Count { get; private set; }

    public void Add(BinaryTreeNode<T>? node)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        if (root == null)
        {
            root = node;
            Count++;
            return;
        }

        InsertSorted(root, node);
        Count++;
    }

    private static void InsertSorted(BinaryTreeNode<T>? node, BinaryTreeNode<T> newNode)
    {
        while (true)
        {
            if (node == null)
            {
                return;
            }

            if (Comparer<T>.Default.Compare(node.Key, newNode.Key) > 0 && node.Left == null)
            {
                node.Left = newNode;
                return;
            }
            if (Comparer<T>.Default.Compare(node.Key, newNode.Key) < 0 && node.Right == null)
            {
                node.Right = newNode;
                return;
            }

            if (Comparer<T>.Default.Compare(node.Key, newNode.Key) > 0 && node.Left != null)
            {
                node = node.Left;
                continue;
            }
            if (Comparer<T>.Default.Compare(node.Key, newNode.Key) < 0 && node.Right != null)
            {
                node = node.Right;
                continue;
            }

            Console.WriteLine("node.key={0}, newNode.key={1}", node.Key, newNode.Key);
            node = null;
        }
    }

    public void Remove(BinaryTreeNode<T>? node)
    {
        if (node == null)
        {
            throw new ArgumentNullException("node");
        }

        if (!Contains(node))
        {
            throw new ArgumentException("Tree does not contain node.", "node");
        }

        if (node.Left != null && node.Right != null)
        {
            throw new ArgumentException("Node cannot be removed unambiguously.", "node");
        }

        if (root == node)
        {
            root = null;
            Count--;
            return;
        }

        var parent = TraverseBreadthFirst(root, n => n.Left == node || n.Right == node);
        var childNode = node.Left ?? node.Right;

        if (parent != null && parent.Left == node)
        {
            parent.Left = childNode;
        }
        if (parent != null && parent.Right == node)
        {
            parent.Right = childNode;
        }

        Count--;
    }

    public BinaryTreeNode<T>? Find(T key)
    {
        return Find(root, key);
    }

    private static BinaryTreeNode<T>? Find(BinaryTreeNode<T>? node, T key)
    {
        if (node == null)
        {
            return null;
        }

        if (Comparer<T>.Default.Compare(node.Key, key) == 0)
        {
            return node;
        }
        if (Comparer<T>.Default.Compare(node.Key, key) > 0)
        {
            return Find(node.Left, key);
        }

        return Find(node.Right, key);
    }

    private static BinaryTreeNode<T>? TraverseBreadthFirst(
        BinaryTreeNode<T>? node,
        Func<BinaryTreeNode<T>, bool> func
    )
    {
        if (node == null)
        {
            return null;
        }
        if (func(node))
        {
            return node;
        }

        var left = TraverseBreadthFirst(node.Left, func);
        if (left != null)
        {
            return left;
        }
        var right = TraverseBreadthFirst(node.Right, func);
        if (right != null)
        {
            return right;
        }

        return null;
    }

    private bool Contains(BinaryTreeNode<T> node)
    {
        var containedNode = TraverseBreadthFirst(root, n => n == node);
        return containedNode != null;
    }

    private static BinaryTreeNode<T>? TraverseDepthFirst(
        BinaryTreeNode<T>? node,
        Func<BinaryTreeNode<T>, bool> func
    )
    {
        if (node == null)
        {
            return null;
        }

        var left = TraverseDepthFirst(node.Left, func);
        if (left != null)
        {
            return left;
        }
        var right = TraverseDepthFirst(node.Right, func);
        if (right != null)
        {
            return right;
        }

        return func(node) ? node : null;
    }

    public bool Contains(Func<BinaryTreeNode<T>, bool> func)
    {
        var containedNode = TraverseDepthFirst(root, func);
        return containedNode != null;
    }

    private static IEnumerable<BinaryTreeNode<T>>? TraverseBreadthFirst(
        BinaryTreeNode<T>? node,
        ref BinaryTreeNode<T>[] list,
        ref int index
    )
    {
        if (node == null)
        {
            return null;
        }

        list[index] = node;
        index++;

        var leftList = TraverseBreadthFirst(node.Left, ref list, ref index);
        if (leftList != null)
        {
            return leftList;
        }
        var rightList = TraverseBreadthFirst(node.Right, ref list, ref index);
        if (rightList != null)
        {
            return rightList;
        }

        return null;
    }

    public void Clear()
    {
        root = null;
        Count = 0;
    }

    public void LoadAndBalance(BinaryTreeNode<T>[] list)
    {
        Clear();
        Balance(list);
    }

    public void Balance()
    {
        var list = new BinaryTreeNode<T>[Count];
        var index = 0;

        TraverseBreadthFirst(root, ref list, ref index);

        Clear();
        Balance(list);
    }

    private void Balance(BinaryTreeNode<T>[] list)
    {
        if (list.Length == 0)
        {
            return;
        }

        var lists = SplitList(list);

        var mid = Pop(ref lists[0]);
        Add(new BinaryTreeNode<T>(mid.Key));

        Balance(lists[0]);
        Balance(lists[1]);
    }

    private static BinaryTreeNode<T>[][] SplitList(BinaryTreeNode<T>[] list)
    {
        var half = (int)Math.Ceiling(list.Length / 2.0);

        var a = new BinaryTreeNode<T>[half];
        var b = new BinaryTreeNode<T>[list.Length - half];

        Array.Copy(list, 0, a, 0, a.Length);
        Array.Copy(list, half, b, 0, b.Length);

        return new[] { a, b };
    }

    private static BinaryTreeNode<T> Pop(ref BinaryTreeNode<T>[] list)
    {
        var last = list[list.Length - 1];
        var a = new BinaryTreeNode<T>[list.Length - 1];

        Array.Copy(list, 0, a, 0, list.Length - 1);
        list = a;

        return last;
    }
}
