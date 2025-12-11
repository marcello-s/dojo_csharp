#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataHeap;

[TestFixture]
public class LinkedListTests
{
    private LinkedList<int> linkedList = null!;

    [SetUp]
    public void Setup()
    {
        linkedList = new LinkedList<int>();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(linkedList, Is.InstanceOf<LinkedList<int>>());
    }

    [Test]
    public void Add_WithNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => linkedList.Add(null));
    }

    [Test]
    public void Add_WithNode_CorrectCount()
    {
        linkedList.Add(new ListNode<int>(100));
        Assert.That(linkedList.Count, Is.EqualTo(1));
    }

    [Test]
    public void Add_With2Nodes_CorrectCount()
    {
        linkedList.Add(new ListNode<int>(100));
        linkedList.Add(new ListNode<int>(101));
        Assert.That(linkedList.Count, Is.EqualTo(2));
    }

    [Test]
    public void Add_With100Nodes_CorrectCount()
    {
        for (var i = 0; i < 100; ++i)
        {
            linkedList.Add(new ListNode<int>(i));
        }
        Assert.That(linkedList.Count, Is.EqualTo(100));
    }

    [Test]
    public void Add_With10e6Nodes_CorrectCount()
    {
        const int NumberOfItems = (int)10e6;

        for (var i = 0; i < NumberOfItems; ++i)
        {
            linkedList.Add(new ListNode<int>(i));
        }
        Assert.That(linkedList.Count, Is.EqualTo(NumberOfItems));
    }

    [Test]
    public void InsertAt_WithPositionNodeNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            linkedList.InsertAt(null, new ListNode<int>(100))
        );
    }

    [Test]
    public void InsertAt_WithNodeNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            linkedList.InsertAt(new ListNode<int>(100), null)
        );
    }

    [Test]
    public void InsertAt_WithNodeNotInList_ThrowsException()
    {
        var node1 = new ListNode<int>(100);
        var node2 = new ListNode<int>(120);
        linkedList.Add(node1);

        Assert.Throws<ArgumentException>(() => linkedList.InsertAt(node2, new ListNode<int>(110)));
    }

    [Test]
    public void InsertAt_With1Node_InsertedInCenter()
    {
        var node1 = new ListNode<int>(100);
        var node2 = new ListNode<int>(120);
        linkedList.Add(node1);
        linkedList.Add(node2);
        linkedList.InsertAt(node1, new ListNode<int>(110));
        Assert.That(linkedList.Count, Is.EqualTo(3));
    }

    [Test]
    public void InsertAt_With1Node_InsertedAtTail()
    {
        var node1 = new ListNode<int>(100);
        var node2 = new ListNode<int>(110);
        linkedList.Add(node1);
        linkedList.Add(node2);
        linkedList.InsertAt(node2, new ListNode<int>(120));
        Assert.That(linkedList.Count, Is.EqualTo(3));
    }

    [Test]
    public void Remove_WithNodeNull_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => linkedList.Remove(null));
    }

    [Test]
    public void Remove_WithNodeNotInList_ThrowsException()
    {
        var node = new ListNode<int>(100);

        Assert.Throws<ArgumentException>(() => linkedList.Remove(node));
    }

    [Test]
    public void Remove_WithRootNodeOnly_CorrectCount()
    {
        var node = new ListNode<int>(100);
        linkedList.Add(node);
        linkedList.Remove(node);
        Assert.That(linkedList.Count, Is.Zero);
    }

    [Test]
    public void Remove_WithRootNode_CorrectCount()
    {
        var node1 = new ListNode<int>(100);
        var node2 = new ListNode<int>(110);
        linkedList.Add(node1);
        linkedList.Add(node2);
        linkedList.Remove(node1);
        Assert.That(linkedList.Count, Is.EqualTo(1));
    }

    [Test]
    public void Remove_WithTailNode_CorrectCount()
    {
        var node1 = new ListNode<int>(100);
        var node2 = new ListNode<int>(110);
        linkedList.Add(node1);
        linkedList.Add(node2);
        linkedList.Remove(node2);
        Assert.That(linkedList.Count, Is.EqualTo(1));
    }

    [Test]
    public void Find_WithNull_ThrowsException()
    {
        var linkedList = new LinkedList<string>();

        Assert.Throws<ArgumentNullException>(() => linkedList.Find(null!));
    }

    [Test]
    public void Find_WithKeyNotInList_ReturnEmptyList()
    {
        linkedList.Add(new ListNode<int>(100));
        linkedList.Add(new ListNode<int>(110));
        linkedList.Add(new ListNode<int>(120));
        var result = linkedList.Find(500);
        Assert.That(result, Is.InstanceOf<IEnumerable<ListNode<int>>>());
        Assert.That(result.Count(), Is.Zero);
    }

    [Test]
    public void Find_WithKeyInList_ReturnListWith1Item()
    {
        linkedList.Add(new ListNode<int>(100));
        linkedList.Add(new ListNode<int>(110));
        linkedList.Add(new ListNode<int>(120));
        var result = linkedList.Find(110);
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public void Find_WithKey_ReturnListWith5Items()
    {
        for (var j = 0; j < 5; j++)
        {
            for (var i = 0; i < 10; i++)
            {
                linkedList.Add(new ListNode<int>(i));
            }
        }
        var result = linkedList.Find(4);
        Assert.That(result.Count(), Is.EqualTo(5));
    }
}
