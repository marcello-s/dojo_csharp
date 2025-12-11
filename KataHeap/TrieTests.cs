#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataHeap;

[TestFixture]
public class TrieTests
{
    private Trie<int> trie = null!;

    [SetUp]
    public void Setup()
    {
        trie = new Trie<int>();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(trie, Is.InstanceOf<Trie<int>>());
    }

    [Test]
    public void Add_WithWordIsNull_ThenThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => trie.Add(null, default));
    }

    [Test]
    public void Add_WithWordAndValue_ThenValueIsStored()
    {
        trie.Add("cat", 0);
    }

    [Test]
    public void Add_WithTwoWordsAndValues_ThenValuesAreStored()
    {
        trie.Add("cat", 0);
        trie.Add("cap", 1);
    }

    [Test]
    public void GetValue_WithTwoWords_ThenReturnCorrectValue()
    {
        trie.Add("cat", 1);
        trie.Add("cap", 2);
        Assert.That(trie.GetValue("cat"), Is.EqualTo(1));
        Assert.That(trie.GetValue("cap"), Is.EqualTo(2));
    }

    [TestCase("cup")]
    [TestCase("car")]
    [TestCase("cats")]
    [TestCase("ca")]
    public void GetValue_WithWordNotContained_ThenReturnDefault(string word)
    {
        trie.Add("cat", 1);
        trie.Add("cap", 2);
        Assert.That(trie.GetValue(word), Is.EqualTo(0));
    }

    [Test]
    public void GetPartialValue_WithTwoWords_ThenReturnThemBoth()
    {
        trie.Add("cat", 1);
        trie.Add("cap", 2);
        var results = trie.GetPartialValue("ca");
        Assert.That(results, Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public void GetPartialValue_WithThreeWords_ThenReturnTwoMatching()
    {
        trie.Add("car", 1);
        trie.Add("cup", 2);
        trie.Add("cupboard", 3);
        var results = trie.GetPartialValue("cu");
        Assert.That(results, Is.EqualTo(new[] { 2, 3 }));
    }

    [Test]
    public void GetPartialValue_WithThreeWords_ThenReturnSingleMatch()
    {
        trie.Add("car", 1);
        trie.Add("cup", 2);
        trie.Add("cupboard", 3);
        var results = trie.GetPartialValue("cup");
        Assert.That(results, Is.EqualTo(new[] { 2 }));
    }

    [Test]
    public void TrieOfIndices_WithRepeatingWords_ThenReturnCorrectIndices()
    {
        var trie = new Trie<IList<int>>();
        const string text = "A test is a test is a test";
        AddingTextToTrie(trie, text);

        var idx_A = trie.GetValue("A");
        Assert.That(idx_A, Is.EqualTo(new[] { 0 }));
        var idx_test = trie.GetValue("test");
        Assert.That(idx_test, Is.EqualTo(new[] { 2, 12, 22 }));
        var idx_is = trie.GetValue("is");
        Assert.That(idx_is, Is.EqualTo(new[] { 7, 17 }));
        var idx_a = trie.GetValue("a");
        Assert.That(idx_a, Is.EqualTo(new[] { 10, 20 }));
    }

    private static void AddingTextToTrie(Trie<IList<int>> trie, string text)
    {
        // -- breaking text on white space --
        var word = string.Empty;
        var wordIndex = 0;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (!Char.IsWhiteSpace(c))
            {
                word += c;
                continue;
            }

            if (!string.IsNullOrEmpty(word))
            {
                AddWordindex(trie, word, wordIndex);
            }

            word = string.Empty;
            wordIndex = i + 1; // point to potential begin of word
        }

        // flushing word buffer
        if (!string.IsNullOrEmpty(word))
        {
            AddWordindex(trie, word, wordIndex);
        }
        // -- breaking text on white space --
    }

    private static void AddWordindex(Trie<IList<int>> trie, string word, int wordIndex)
    {
        var indices = trie.GetValue(word);
        if (indices != null)
        {
            indices.Add(wordIndex);
        }
        else
        {
            trie.Add(word, new List<int> { wordIndex });
        }
    }
}
