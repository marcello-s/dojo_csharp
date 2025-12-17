#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCompiler;

public struct TextLocation
{
    private readonly int _column;

    public int Column
    {
        get { return _column; }
    }

    public TextLocation(int column)
    {
        if (column < 0)
        {
            throw new ArgumentException("must be >= 0", "column");
        }

        _column = column;
    }
}
