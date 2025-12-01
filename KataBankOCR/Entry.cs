#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion
        
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KataBankOCR
{
    class Entry
    {
        private const string ERR = "ERR";
        private const string ILL = "ILL";
        private const string AMB = "AMB";

        private readonly IList<Digit> _digits = new List<Digit>();
        public IEnumerable<Digit> Digits { get { return _digits; } }

        public string Number { get; set; }
        public string State { get; set; }

        public void AddDigit(Digit digit)
        {
            _digits.Add(digit);
            ComputeNumber();
            if (Number.Length >= 9)
            {
                ComputeState();
                if (State.Equals(ERR) || State.Equals(ILL))
                {
                    AutoCorrect();
                }
            }
        }

        public override string ToString()
        {
            return Number + " " + State;
        }

        private void ComputeState()
        {
            State = IsChecked(Number) ? string.Empty : ERR;
            State = Number.Contains("?") ? ILL : State;
        }

        private void ComputeState(IEnumerable<string> entryCandidates)
        {
            if (entryCandidates != null && entryCandidates.Any())
            {
                State = AMB + "[" + string.Join(",", entryCandidates.ToArray()) + "]";
            }
            else
            {
                ComputeState();
            }
        }

        private void ComputeNumber()
        {
            var text = new StringBuilder();
            foreach (var digit in Digits)
            {
                text.Append(digit.Number);
            }

            Number = text.ToString();
        }

        public static bool IsChecked(string number)
        {
            if (number.Contains("?"))
            {
                return false;
            }

            var sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (int)char.GetNumericValue(number, 8 - i) * (i + 1);
            }

            return sum % 11 == 0;
        }

        private void AutoCorrect()
        {
            /*
             * 1. for each digit get a list of candidate digits, add/remove a stroke
             *   - check each candidate digit for correct checksum and get candidate entries
             * 2. for one single candidate entry -> accept, exit
             * 3. for a list of candidate entries -> list AMB values, exit
             * 
             */

            var entryCandidates = new List<string>();
            var digitIndex = 0;
            foreach (var digit in Digits)
            {
                // digit candidates
                var digitCandidates = new List<string>();
                var digitInput = digit.Text;
                for (int i = 0; i < 7; i++)
                {
                    var changedInput = Digit.SetOrRemoveStroke(digitInput, i);
                    var digitCandidate = Digit.GetNumber(changedInput.GetHashCode());
                    if (!digitCandidate.Equals("?"))
                    {
                        digitCandidates.Add(digitCandidate);
                    }
                }

                // check for correct checksum
                foreach (var digitCandidate in digitCandidates)
                {
                    var entryCandidate = Number.Substring(0, digitIndex)
                        + digitCandidate
                        + Number.Substring(digitIndex + 1, Number.Length - digitIndex - 1);
                    if (IsChecked(entryCandidate))
                    {
                        entryCandidates.Add(entryCandidate);
                    }
                }

                digitIndex++;
            }

            // accept single entry
            if (entryCandidates.Count() == 1)
            {
                Number = entryCandidates.First();
                ComputeState();
            }

            // list AMB values
            if (entryCandidates.Count() > 1)
            {
                entryCandidates.Sort();
                ComputeState(entryCandidates);
            }

        }
    }
}
