#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataHeap;

public class TrieNode<T>(char key)
{
    private IList<TrieNode<T>>? childNodes;
    public char Key { get; init; } = key;
    public T? Value { get; set; }

    public IEnumerable<TrieNode<T>>? Children
    {
        get { return childNodes; }
    }

    public TrieNode<T>? AddChildNode(char key)
    {
        if (childNodes == null)
        {
            childNodes = new List<TrieNode<T>>();
        }

        if (ContainsChildNode(key))
        {
            return GetChildNode(key);
        }

        var newNode = new TrieNode<T>(key);
        childNodes.Add(newNode);
        return newNode;
    }

    public bool ContainsChildNode(char key)
    {
        return childNodes != null && childNodes.Any(n => n.Key.Equals(key));
    }

    public TrieNode<T>? GetChildNode(char key)
    {
        return childNodes == null ? null : childNodes.FirstOrDefault(n => n.Key.Equals(key));
    }
}
