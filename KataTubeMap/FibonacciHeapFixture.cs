#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataTubeMap;

[TestFixture]
public class FibonacciHeapFixture
{
    private FibonacciHeap<string, int> heap = null!;

    [SetUp]
    public void Setup()
    {
        heap = new FibonacciHeap<string, int>();
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(heap, Is.InstanceOf<FibonacciHeap<string, int>>());
    }

    [Test]
    public void InsertTest()
    {
        Assert.That(heap.Min, Is.Null);
        var node = heap.Insert("test", 10);
        Assert.That(node.Key, Is.EqualTo(10));
        Assert.That(heap.Min, Is.EqualTo(node));
        node = heap.Insert("test", 9);
        Assert.That(node.Key, Is.EqualTo(9));
        Assert.That(heap.Min, Is.EqualTo(node));
        node = heap.Insert("test", 11);
        Assert.That(node.Key, Is.EqualTo(11));
        Assert.That(heap.Min, Is.Not.EqualTo(node));
    }

    [Test]
    public void InsertNullTest()
    {
        heap.Insert(null, 10);
    }

    [Test]
    public void MergeTest()
    {
        this.heap.Insert("test", 10);
        this.heap.Insert("test", 9);
        this.heap.Insert("test", 11);
        var heap2 = new FibonacciHeap<string, int>();
        heap2.Insert("test", 8);
        heap2.Insert("test", 15);
        heap2.Insert("test", 12);
        var heap = FibonacciHeap<string, int>.Merge(this.heap, heap2);
        Assert.That(heap?.Min?.Key, Is.EqualTo(8));
    }

    [Test]
    public void DecreaseKeyTest()
    {
        var node10 = heap.Insert("test", 10);
        heap.Insert("test", 9);
        var node11 = heap.Insert("test", 11);
        heap.Insert("test", 8);
        var node15 = heap.Insert("test", 15);
        heap.Insert("test", 12);
        var min = heap.ExtractMin();

        // for the asserts the structure of the heap
        // is rendered to string

        //Console.WriteLine(_heap.ToString());
        Assert.That(heap.ToString(), Is.EqualTo("9-{10-11-{15}}-12"));
        heap.DecreaseKey(node11, 8);
        //Console.WriteLine(_heap.ToString());
        Assert.That(heap.ToString(), Is.EqualTo("8-{15}-9-{10}-12"));
        Assert.That(node11.Key, Is.EqualTo(8));
        heap.DecreaseKey(node10, 9);
        //Console.WriteLine(_heap.ToString());
        Assert.That(heap.ToString(), Is.EqualTo("8-{15}-9-{9}-12"));
        Assert.That(node10.Key, Is.EqualTo(9));
        heap.DecreaseKey(node15, 7);
        //Console.WriteLine(_heap.ToString());
        Assert.That(heap.ToString(), Is.EqualTo("7-8-9-{9}-12"));
        Assert.That(node15.Key, Is.EqualTo(7));
    }

    [Test]
    public void RemoveTest()
    {
        heap.Insert("test", 10);
        heap.Insert("test", 9);
        var node = heap.Insert("test", 11);
        heap.Insert("test", 8);
        heap.Insert("test", 15);
        heap.Insert("test", 12);
        var removed = heap.Remove(node);
        var current = heap.Min!;
        Assert.That(current, Is.Not.EqualTo(removed));
        while (!current.Right.Equals(heap.Min))
        {
            current = current.Right;
            Assert.That(current, Is.Not.EqualTo(removed));
        }
    }

    [Test]
    public void ExtraxtMinTest()
    {
        heap.Insert("test", 10);
        heap.Insert("test", 9);
        heap.Insert("test", 11);
        heap.Insert("test", 8);
        heap.Insert("test", 15);
        heap.Insert("test", 12);

        // for the asserts the structure of the heap
        // is rendered to string

        //Console.WriteLine(_heap.ToString());
        Assert.That(heap.ToString(), Is.EqualTo("8-9-10-11-15-12"));
        var min = heap.ExtractMin()!;
        //Console.WriteLine(_heap.ToString());
        Assert.That(heap.ToString(), Is.EqualTo("9-{10-11-{15}}-12"));
        Assert.That(min.Key, Is.EqualTo(8));
        Assert.That(heap?.Min?.Key, Is.EqualTo(9));

        var minValues = new[] { 9, 10, 11, 12, 15 };
        var heapStructures = new[] { "10-{12-11-{15}}", "11-{15}-12", "12-{15}", "15", "empty" };
        var index = 0;
        while (heap?.Count > 0)
        {
            min = heap.ExtractMin();
            //Console.Write("min:{0}\t", min.Key);
            Assert.That(min?.Key, Is.EqualTo(minValues[index]));
            //Console.WriteLine(_heap.ToString());
            Assert.That(heap.ToString(), Is.EqualTo(heapStructures[index]));
            index++;
        }
    }
}
