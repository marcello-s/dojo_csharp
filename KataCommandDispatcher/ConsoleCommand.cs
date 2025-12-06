#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public class ConsoleCommand : Command
{
    public string Text { get; protected set; }

    public ConsoleCommand(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException("text");
        }

        Text = text;
    }
}
