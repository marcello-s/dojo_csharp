#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataDataCompression;

class Tree
{
    private readonly int bufferSize;
    private readonly int upperMatchLength;
    private readonly int None;
    private readonly int[] parent;
    private readonly int[] leftChild;
    private readonly int[] rightChild;

    public Tree(int bufferSize, int upperMatchLength)
    {
        // check buffersize > 0

        this.bufferSize = bufferSize;
        this.upperMatchLength = upperMatchLength;
        None = this.bufferSize;
        parent = new int[this.bufferSize + 1];
        leftChild = new int[this.bufferSize + 1];
        rightChild = new int[this.bufferSize + 257];
    }

    public void Init()
    {
        for (var i = bufferSize + 1; i <= bufferSize + 256; i++)
        {
            rightChild[i] = None;
        }

        for (var i = 0; i < bufferSize; i++)
        {
            parent[i] = None;
        }
    }

    public void InsertNode(int r) { }

    public void DeleteNode(int p) { }
}
