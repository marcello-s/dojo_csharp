#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion
                
namespace KataBankOCR
{
    class Digit
    {
        private readonly string _input;
        private readonly int _index;

        public Digit(string input, int index)
        {
            _input = input;
            _index = index;
        }

        public string Text { get; private set; }
        public string Number { get; private set; }

        public Digit Parse()
        {
            Text = DigitText(_input, _index);
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
            switch (hash)
            {
                case 485846108:
                    return "0";
                case 1865122982:
                    return "1";
                case -1445372811:
                    return "2";
                case 900428925:
                    return "3";
                case -1441827709:
                    return "4";
                case -1610386307:
                    return "5";
                case 338779185:
                    return "6";
                case 1824294056:
                    return "7";
                case 485841969:
                    return "8";
                case -1463323523:
                    return "9";
                default:
                    return "?";
            }
        }

        public static string SetOrRemoveStroke(string input, int index)
        {
            switch (index)
            {
                case 0:
                    return ReplaceChar(input, 1, true);
                case 1:
                    return ReplaceChar(input, 3, false);
                case 2:
                    return ReplaceChar(input, 4, true);
                case 3:
                    return ReplaceChar(input, 5, false);
                case 4:
                    return ReplaceChar(input, 6, false);
                case 5:
                    return ReplaceChar(input, 7, true);
                case 6:
                    return ReplaceChar(input, 8, false);
                default:
                    return null;
            }
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
}
