#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

public class Rule5 : Rule<User>
{
    private bool isFredOnline,
        isMikeOnline;

    protected override object? DoEvaluate(User message)
    {
        if (message.Name.Equals("Fred"))
        {
            isFredOnline = message.IsOnline;
        }

        if (message.Name.Equals("Mike"))
        {
            isMikeOnline = message.IsOnline;
        }

        if (isFredOnline && isMikeOnline)
        {
            return "$rule5";
        }

        return null;
    }
}
