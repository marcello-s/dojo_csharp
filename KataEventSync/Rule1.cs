#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

public class Rule1 : Rule<string>
{
    bool event1,
        event2,
        event3;

    protected override object? DoEvaluate(string message)
    {
        event1 = event1 || message.Equals("$event1");
        event2 = event2 || message.Equals("$event2");
        event3 = event3 || message.Equals("$event3");
        if (event1 && event2 && event3)
        {
            event1 = event2 = event3 = false;
            return "$rule1";
        }

        return null;
    }
}
