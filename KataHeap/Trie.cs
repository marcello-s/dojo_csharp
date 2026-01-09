#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataHeap;

public class Trie<T>
{
    private readonly TrieNode<T> root;

    public Trie()
    {
        root = new TrieNode<T>('\0');
    }

    public void Add(string? word, T? value)
    {
        if (word == null)
        {
            throw new ArgumentNullException("word");
        }

        var node = root;
        foreach (var c in word)
        {
            var newNode = node.AddChildNode(c);
            if (newNode != null)
            {
                node = newNode;
            }
        }

        node.Value = value;
    }

    public T? GetValue(string word)
    {
        if (word == null)
        {
            throw new ArgumentNullException(nameof(word));
        }

        var node = root;
        foreach (var c in word)
        {
            if (node != null)
            {
                if (!node.ContainsChildNode(c))
                {
                    return default;
                }

                var child = node.GetChildNode(c);
                node = child;
            }
        }

        // if the value equals default(T) we can return
        // a list of all leaves for this prefix
        // from child nodes -> IEnumerable<T>
        return node != null ? node.Value : default;
    }

    public IEnumerable<T?> GetPartialValue(string word)
    {
        if (word == null)
        {
            throw new ArgumentNullException(nameof(word));
        }

        var node = root;
        foreach (var c in word)
        {
            if (node != null)
            {
                if (!node.ContainsChildNode(c))
                {
                    return [default];
                }

                var child = node.GetChildNode(c);
                node = child;
            }
        }

        var results = new List<T?>();
        if (EqualityComparer<T>.Default.Equals(node!.Value, default))
        {
            TraverseDepthFirst(
                node,
                n =>
                {
                    if (!EqualityComparer<T>.Default.Equals(n.Value, default))
                    {
                        results.Add(n.Value);
                    }
                }
            );
        }
        else
        {
            results.Add(node.Value);
        }

        return results;
    }

    private static void TraverseDepthFirst(TrieNode<T>? node, Action<TrieNode<T>> action)
    {
        if (node == null)
        {
            return;
        }

        action(node);
        if (node.Children == null)
        {
            return;
        }

        foreach (var child in node.Children)
        {
            TraverseDepthFirst(child, action);
        }
    }
}
