#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataHeap;

public class ListNode<T>(T key)
{
    public ListNode<T>? Next { get; set; }

    public T Key { get; init; } = key;
}
