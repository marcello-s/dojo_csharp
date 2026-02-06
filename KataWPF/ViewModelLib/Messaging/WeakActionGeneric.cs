#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Messaging;

public class WeakActionGeneric<T>(object target, Action<T> action)
    : WeakAction(target, null),
        IExecuteWithObject
{
    private readonly Action<T> action = action;

    public new Action<T> Action
    {
        get { return action; }
    }

    public new void Execute()
    {
        if (action != null && IsAlive)
        {
            action(default!);
        }
    }

    public void Execute(T parameter)
    {
        if (action != null && IsAlive)
        {
            action(parameter);
        }
    }

    public void ExecuteWithObject(object? parameter)
    {
        if (parameter != null)
        {
            var parameterCasted = (T)parameter;
            Execute(parameterCasted);
        }
    }
}
