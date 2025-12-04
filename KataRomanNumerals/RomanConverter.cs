#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataRomanNumerals;

public static class RomanConverter
{
    private static int power;
    private static int accu;
    private static IList<Numeral> numerals = new List<Numeral>();

    public static string Convert(string input)
    {
        var convertedValue = string.Empty;
        power = 0;
        accu = 0;

        var conversion = SetConversion(input);
        convertedValue = EnumerateStringRightToLeft(input, conversion)
            .Aggregate(convertedValue, (current, part) => part + current);
        if (string.IsNullOrEmpty(convertedValue))
        {
            convertedValue = accu.ToString();
        }

        return convertedValue;
    }

    private static IEnumerable<string> EnumerateStringRightToLeft(
        string input,
        Func<string, string>? evaluateCall
    )
    {
        if (evaluateCall == null)
        {
            yield break;
        }

        for (var i = 0; i < input.Length; i++)
        {
            yield return evaluateCall(input[input.Length - i - 1].ToString());
        }
    }

    private static string ArabicToRoman(string input)
    {
        var romanLetter = string.Empty;
        switch (input)
        {
            case "1":
            case "2":
            case "3":
                romanLetter = new String(numerals[power].Letter[0], int.Parse(input));
                break;
            case "4":
                romanLetter = numerals[power].Letter + numerals[power].LeftBound;
                break;
            case "5":
                romanLetter = numerals[power].LeftBound;
                break;
            case "6":
            case "7":
            case "8":
                romanLetter = numerals[power].LeftBound;
                romanLetter += new String(numerals[power].Letter[0], int.Parse(input) - 5);
                break;
            case "9":
                romanLetter = numerals[power].Letter + numerals[power].RightBound;
                break;
        }

        if (power < 3)
        {
            power++;
        }

        return romanLetter;
    }

    private static string RomanToArabic(string input)
    {
        switch (input)
        {
            case "I":
                accu += (accu == 5 || accu >= 10) ? -1 : 1;
                break;
            case "V":
                accu += 5;
                break;
            case "X":
                accu += (accu == 50 || accu >= 100) ? -10 : 10;
                break;
            case "L":
                accu += 50;
                break;
            case "C":
                accu += (accu == 500 || accu >= 1000) ? -100 : 100;
                break;
            case "D":
                accu += 500;
                break;
            case "M":
                accu += 1000;
                break;
        }

        return string.Empty;
    }

    private static Func<string, string>? SetConversion(string input)
    {
        const string ARABIC_NUMERALS = "0123456789";
        const string ROMAN_NUMERALS = "IVXLCDM";

        // convert arabic to roman
        if (ContainAnyChar(input, ARABIC_NUMERALS))
        {
            numerals.Clear();
            numerals.Add(new Numeral("I", "V", "X"));
            numerals.Add(new Numeral("X", "L", "C"));
            numerals.Add(new Numeral("C", "D", "M"));
            numerals.Add(new Numeral("M", "M", "M"));
            return ArabicToRoman;
        }

        // convert roman to arabic
        if (ContainAnyChar(input, ROMAN_NUMERALS))
        {
            numerals.Clear();
            return RomanToArabic;
        }

        return null;
    }

    private static bool ContainAnyChar(string input, string characters)
    {
        return characters.Any(input.Contains);
    }
}
