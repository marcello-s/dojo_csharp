#region license and copyright
/*
The MIT License, Copyright (c) 2011-2025 Marcel Schneider
for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataHeap;

[TestFixture]
public class SuffixTreeNodeTests
{
    private SuffixTree suffixTree = null!;

    [SetUp]
    public void Setup()
    {
        suffixTree = new SuffixTree();
    }

    [Test]
    public void Add_abc_Added()
    {
        AddString("abc");
    }

    [Test]
    public void Add_abcabxabcd_Added()
    {
        AddString("abcabxabcd");
    }

    [Test]
    public void Add_bananas_Added()
    {
        AddString("bananas");
    }

    private void AddString(string data)
    {
        foreach (var c in data)
        {
            suffixTree.Add(c);
        }
    }
}
