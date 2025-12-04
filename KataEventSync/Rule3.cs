#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

public class Rule3 : Rule<string>
{
    private bool rule1,
        rule2;

    protected override object? DoEvaluate(string message)
    {
        rule1 = rule1 || message.Equals("$rule1");
        rule2 = rule2 || message.Equals("$rule2");
        if (rule1 && rule2)
        {
            rule1 = rule2 = false;
            return "$rule3";
        }

        return null;
    }
}
