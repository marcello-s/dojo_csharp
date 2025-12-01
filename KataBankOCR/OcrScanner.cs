#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion
        
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KataBankOCR
{
    class OcrScanner
    {
        public IEnumerable<Entry> Scan(string path, string marker, int numberOfEntries)
        {
            var entries = new List<Entry>();
            using (var reader = new StreamReader(path))
            {
                // read until marker found
                string line;
                do
                {
                    line = reader.ReadLine();
                }
                while (line != null && (!reader.EndOfStream && !line.StartsWith(marker)));

                // scan the number of entries
                for (int i = 0; i < numberOfEntries; i++)
                {
                    entries.Add(ParseEntry(reader));
                }
            }

            return entries;
        }

        private Entry ParseEntry(StreamReader reader)
        {
            var lines = new StringBuilder();
            var lineCount = 0;
            while (!reader.EndOfStream && lineCount < 3)
            {
                lines.Append(reader.ReadLine());
                lineCount++;
            }
            var entry = new Entry();
            for (int i = 0; i < 9; i++)
            {
                entry.AddDigit(new Digit(lines.ToString(), i).Parse());
            }

            return entry;
        }
    }
}
