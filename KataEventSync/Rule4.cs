#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

public class Rule4 : Rule<User>
{
    private bool isNameFred,
        isNameMike;

    protected override object? DoEvaluate(User message)
    {
        isNameFred = isNameFred || message.Name.Equals("Fred");
        isNameMike = isNameMike || message.Name.Equals("Mike");
        if (isNameMike && isNameMike)
        {
            isNameFred = isNameMike = false;
            return "$rule4";
        }

        return null;
    }
}
