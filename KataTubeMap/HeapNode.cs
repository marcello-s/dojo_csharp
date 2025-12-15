#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataTubeMap;

public class HeapNode<TData, TKey>
{
    public HeapNode<TData, TKey> Left { get; set; }
    public HeapNode<TData, TKey> Right { get; set; }
    public HeapNode<TData, TKey>? Parent { get; set; }
    public HeapNode<TData, TKey>? Child { get; set; }

    public TData Data { get; private set; }
    public TKey Key { get; set; }
    public int Degree { get; set; }
    public bool IsMarked { get; set; }

    public HeapNode(TData data, TKey key)
    {
        Data = data;
        Key = key;
        Right = this;
        Left = this;
    }
}
