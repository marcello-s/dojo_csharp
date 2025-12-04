#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataBankOCR;

public class Digit(string input, int index)
{
    private readonly string input = input;
    private readonly int index = index;

    public string? Text { get; private set; }
    public string? Number { get; private set; }

    public Digit Parse()
    {
        Text = DigitText(input, index);
        Number = GetNumber(Text.GetHashCode());
        return this;
    }

    public static string DigitText(string input, int index)
    {
        const int LENGTH = 3;
        return input.Substring(index * LENGTH, LENGTH)
            + input.Substring(9 * LENGTH + index * LENGTH, LENGTH)
            + input.Substring(18 * LENGTH + index * LENGTH, LENGTH);
    }

    public static string GetNumber(int hash)
    {
        return hash switch
        {
            485846108 => "0",
            1865122982 => "1",
            -1445372811 => "2",
            900428925 => "3",
            -1441827709 => "4",
            -1610386307 => "5",
            338779185 => "6",
            1824294056 => "7",
            485841969 => "8",
            -1463323523 => "9",
            _ => "?",
        };
    }

    public static string? SetOrRemoveStroke(string? input, int index)
    {
        if (input is null)
        {
            return null;
        }

        return index switch
        {
            0 => ReplaceChar(input, 1, true),
            1 => ReplaceChar(input, 3, false),
            2 => ReplaceChar(input, 4, true),
            3 => ReplaceChar(input, 5, false),
            4 => ReplaceChar(input, 6, false),
            5 => ReplaceChar(input, 7, true),
            6 => ReplaceChar(input, 8, false),
            _ => null,
        };
    }

    private static string ReplaceChar(string input, int index, bool isUnderscore)
    {
        const char UNDERSCORE = '_';
        const char PIPE = '|';
        const char SPACE = ' ';

        var beforeIndex = input.Substring(0, index);
        char atIndex;
        if (isUnderscore)
        {
            atIndex = input[index].Equals(UNDERSCORE) ? SPACE : UNDERSCORE;
        }
        else
        {
            atIndex = input[index].Equals(PIPE) ? SPACE : PIPE;
        }

        var afterIndex = input.Substring(index + 1, input.Length - index - 1);
        return beforeIndex + atIndex + afterIndex;
    }
}
