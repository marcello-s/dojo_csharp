#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataHeap;

[TestFixture]
public class TrieNodeTests
{
    private TrieNode<int> trieNode = null!;

    [SetUp]
    public void Setup()
    {
        trieNode = new TrieNode<int>('A');
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(trieNode, Is.InstanceOf<TrieNode<int>>());
    }

    [Test]
    public void Key_ReturnsA()
    {
        Assert.That(trieNode.Key, Is.EqualTo('A'));
    }

    [Test]
    public void AddChildNode_WithKey_ThenChildNodesContainsNode()
    {
        const char key = 'B';
        Assert.That(trieNode.GetChildNode(key), Is.Null);
        var node = trieNode.AddChildNode(key);
        Assert.That(trieNode.GetChildNode(key), Is.EqualTo(node));
    }

    [Test]
    public void AddChildNode_WithSameKey_ThenReturnSameNode()
    {
        var node = trieNode.AddChildNode('B');
        Assert.That(trieNode.AddChildNode('B'), Is.EqualTo(node));
    }

    [Test]
    public void ContainsChildNode_WithNoChildNodes_ThenReturnFalse()
    {
        Assert.That(trieNode.ContainsChildNode('B'), Is.False);
    }

    [Test]
    public void ContainsChildNode_WithOneNode_ThenReturnTrue()
    {
        const char key = 'B';
        trieNode.AddChildNode(key);
        Assert.That(trieNode.ContainsChildNode(key), Is.True);
    }

    [Test]
    public void ContainsChildNode_WithOtherNode_ThenReturnFalse()
    {
        trieNode.AddChildNode('B');
        Assert.That(trieNode.ContainsChildNode('C'), Is.False);
    }

    [Test]
    public void GetChildNode_WithNoChildNodes_ThenReturnNull()
    {
        Assert.That(trieNode.GetChildNode('B'), Is.Null);
    }

    [Test]
    public void GetChildNode_WithOneNode_ThenReturnNode()
    {
        const char key = 'B';
        trieNode.AddChildNode(key);
        Assert.That(trieNode.GetChildNode(key), Is.InstanceOf<TrieNode<int>>());
    }

    [Test]
    public void GetChildNode_WithOtherNode_ThenReturnNull()
    {
        trieNode.AddChildNode('B');
        Assert.That(trieNode.GetChildNode('C'), Is.Null);
    }
}
