#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Diagnostics;
using NUnit.Framework;

namespace KataHeap;

[TestFixture]
public class BinaryTreeTests
{
    private BinaryTree<int> binaryTree = null!;

    [SetUp]
    public void Setup()
    {
        binaryTree = new BinaryTree<int>();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(binaryTree, Is.InstanceOf<BinaryTree<int>>());
    }

    [Test]
    public void Add_WithNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => binaryTree.Add(null));
    }

    [Test]
    public void Add_WithNode_CorrectCount()
    {
        binaryTree.Add(new BinaryTreeNode<int>(100));
        Assert.That(binaryTree.Count, Is.EqualTo(1));
    }

    [Test]
    public void Add_With3Nodes_NodesAreAtCorrectPlaces()
    {
        binaryTree.Add(new BinaryTreeNode<int>(100));
        binaryTree.Add(new BinaryTreeNode<int>(99));
        binaryTree.Add(new BinaryTreeNode<int>(101));
        Assert.That(binaryTree.Count, Is.EqualTo(3));
    }

    [Test]
    public void Add_With7Nodes_NodesAreAtCorrectPlaces()
    {
        binaryTree.Add(new BinaryTreeNode<int>(4));
        binaryTree.Add(new BinaryTreeNode<int>(2));
        binaryTree.Add(new BinaryTreeNode<int>(6));
        binaryTree.Add(new BinaryTreeNode<int>(1));
        binaryTree.Add(new BinaryTreeNode<int>(3));
        binaryTree.Add(new BinaryTreeNode<int>(5));
        binaryTree.Add(new BinaryTreeNode<int>(7));
        Assert.That(binaryTree.Count, Is.EqualTo(7));
    }

    [Test]
    public void Add_With7Nodes_DifferentOrderAtCorrectPlaces()
    {
        binaryTree.Add(new BinaryTreeNode<int>(4));
        binaryTree.Add(new BinaryTreeNode<int>(3));
        binaryTree.Add(new BinaryTreeNode<int>(5));
        binaryTree.Add(new BinaryTreeNode<int>(2));
        binaryTree.Add(new BinaryTreeNode<int>(1));
        binaryTree.Add(new BinaryTreeNode<int>(6));
        binaryTree.Add(new BinaryTreeNode<int>(7));
        Assert.That(binaryTree.Count, Is.EqualTo(7));
    }

    [Test]
    public void Remove_WithNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => binaryTree.Remove(null));
    }

    [Test]
    public void Remove_WithBlankTree_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => binaryTree.Remove(new BinaryTreeNode<int>(99)));
    }

    [Test]
    public void Remove_WithNodeNotContained_ThrowsException()
    {
        binaryTree.Add(new BinaryTreeNode<int>(100));
        Assert.That(binaryTree.Count, Is.EqualTo(1));

        Assert.Throws<ArgumentException>(() => binaryTree.Remove(new BinaryTreeNode<int>(99)));
    }

    [Test]
    public void Remove_With1Node_NodeRemoved()
    {
        var node = new BinaryTreeNode<int>(99);
        binaryTree.Add(node);
        Assert.That(binaryTree.Count, Is.EqualTo(1));
        binaryTree.Remove(node);
        Assert.That(binaryTree.Count, Is.EqualTo(0));
    }

    [Test]
    public void Remove_With3Nodes_NodeRemoved()
    {
        var node = new BinaryTreeNode<int>(99);
        binaryTree.Add(new BinaryTreeNode<int>(100));
        binaryTree.Add(node);
        binaryTree.Add(new BinaryTreeNode<int>(101));
        Assert.That(binaryTree.Count, Is.EqualTo(3));
        binaryTree.Remove(node);
        Assert.That(binaryTree.Count, Is.EqualTo(2));
    }

    private static void AddNodes(BinaryTree<int> tree)
    {
        tree.Add(new BinaryTreeNode<int>(4));
        tree.Add(new BinaryTreeNode<int>(2));
        tree.Add(new BinaryTreeNode<int>(6));
        tree.Add(new BinaryTreeNode<int>(1));
        tree.Add(new BinaryTreeNode<int>(3));
        tree.Add(new BinaryTreeNode<int>(5));
        tree.Add(new BinaryTreeNode<int>(7));
    }

    [Test]
    public void Find_WithKey1_ReturnCorrectNode()
    {
        AddNodes(binaryTree);
        var node = binaryTree.Find(1);
        Assert.That(node!.Key, Is.EqualTo(1));
    }

    [Test]
    public void Find_WithKey7_ReturnCorrectNode()
    {
        AddNodes(binaryTree);
        var node = binaryTree.Find(7);
        Assert.That(node!.Key, Is.EqualTo(7));
    }

    [Test]
    public void Find_WithKey8_ReturnNull()
    {
        AddNodes(binaryTree);
        Assert.That(binaryTree.Find(8), Is.Null);
    }

    [Test]
    public void Contains_WithFuncKeyEq7_True()
    {
        AddNodes(binaryTree);
        Assert.That(binaryTree.Contains(n => n.Key == 7), Is.True);
    }

    [Test]
    public void Contains_WithFuncKeyEq5_True()
    {
        AddNodes(binaryTree);
        Assert.That(binaryTree.Contains(n => n.Key == 5), Is.True);
    }

    [Test]
    public void Contains_WithFuncKeyEq2_True()
    {
        AddNodes(binaryTree);
        Assert.That(binaryTree.Contains(n => n.Key == 2), Is.True);
    }

    [Test]
    public void Contains_WithFuncReturnsFalse_False()
    {
        AddNodes(binaryTree);
        Assert.That(binaryTree.Contains(n => n.Key == 0), Is.False);
    }

    private class AnnotatedBinaryTreeNode<T> : BinaryTreeNode<T>
    {
        public string Annotation { get; private set; }

        public AnnotatedBinaryTreeNode(T key, string annotation)
            : base(key)
        {
            Annotation = annotation;
        }
    }

    [Test]
    public void Add_AnnotatedNode_CorrectCount()
    {
        binaryTree.Add(new AnnotatedBinaryTreeNode<int>(0, "my annotation"));
        Assert.That(binaryTree.Count, Is.EqualTo(1));
    }

    [Test]
    public void Balance_WithOrderedNodes_BalancedTree()
    {
        binaryTree.Add(new BinaryTreeNode<int>(1));
        binaryTree.Add(new BinaryTreeNode<int>(2));
        binaryTree.Add(new BinaryTreeNode<int>(3));
        binaryTree.Add(new BinaryTreeNode<int>(4));
        binaryTree.Add(new BinaryTreeNode<int>(5));
        binaryTree.Add(new BinaryTreeNode<int>(6));
        binaryTree.Add(new BinaryTreeNode<int>(7));

        binaryTree.Balance();

        var node = binaryTree.Find(7);
        Assert.That(node!.Key, Is.EqualTo(7));
    }

    [Test]
    public void Balance_With10e6orderedNodes_BalanceTree()
    {
        const int NumberOfItems = (int)1e6;
        const int SpecificItem = NumberOfItems - 2;
        var stopWatch = new Stopwatch();

        Console.WriteLine("inserting {0} items in quasi random order..", NumberOfItems);
        stopWatch.Start();
        const int iMax = 1000;
        const int jMax = NumberOfItems / iMax;
        for (var j = 0; j < jMax; ++j)
        {
            for (var i = 0; i < iMax; ++i)
            {
                binaryTree.Add(new BinaryTreeNode<int>(j + i * jMax));
            }
        }
        stopWatch.Stop();
        Console.WriteLine("time [ms]: {0}", stopWatch.Elapsed.TotalMilliseconds);

        Console.WriteLine("balancing..");
        stopWatch.Reset();
        stopWatch.Start();
        binaryTree.Balance();
        stopWatch.Stop();
        Console.WriteLine("time [ms]: {0}", stopWatch.Elapsed.TotalMilliseconds);
        Console.WriteLine("item count: {0}", binaryTree.Count);

        Console.WriteLine("searching item {0}..", SpecificItem);
        stopWatch.Reset();
        stopWatch.Start();
        var node = binaryTree.Find(SpecificItem);
        stopWatch.Stop();
        Console.WriteLine("time [ms]: {0}", stopWatch.Elapsed.TotalMilliseconds);
        Assert.That(node!.Key, Is.EqualTo(SpecificItem));
    }

    [Test]
    public void LoadAndBalance_With10e7orderedNodes_BalanceTree()
    {
        const int NumberOfItems = (int)1e7;
        const int SpecificItem = NumberOfItems - 2;
        var stopWatch = new Stopwatch();

        Console.WriteLine("load and balance {0} ordered items..", NumberOfItems);
        var list = new BinaryTreeNode<int>[NumberOfItems];
        for (var i = 0; i < NumberOfItems; ++i)
        {
            list[i] = new BinaryTreeNode<int>(i);
        }
        Console.WriteLine("balancing..");
        stopWatch.Start();
        binaryTree.LoadAndBalance(list);
        stopWatch.Stop();
        Console.WriteLine("time [ms]: {0}", stopWatch.Elapsed.TotalMilliseconds);
        Console.WriteLine("item count: {0}", binaryTree.Count);

        Console.WriteLine("searching item {0}..", SpecificItem);
        stopWatch.Reset();
        stopWatch.Start();
        var node = binaryTree.Find(SpecificItem);
        stopWatch.Stop();
        Console.WriteLine("time [ms]: {0}", stopWatch.Elapsed.TotalMilliseconds);
        Assert.That(node!.Key, Is.EqualTo(SpecificItem));
    }
}
