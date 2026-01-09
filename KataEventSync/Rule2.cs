#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

public class Rule2 : Rule<string>
{
    private bool event4,
        event5;

    protected override object? DoEvaluate(string message)
    {
        event4 = event4 || message.Equals("$event4");
        event5 = event5 || message.Equals("$event5");
        if (event4 || event5)
        {
            event4 = event5 = false;
            return "$rule2";
        }

        return null;
    }
}
