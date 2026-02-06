#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Messaging;

public class WeakAction(object target, Action? action)
{
    private WeakReference? reference = new WeakReference(target);

    public Action? Action
    {
        get { return action; }
    }

    public bool IsAlive
    {
        get
        {
            if (reference == null)
            {
                return false;
            }

            return reference.IsAlive;
        }
    }

    public object? Target
    {
        get
        {
            if (reference == null)
            {
                return null;
            }

            return reference.Target;
        }
    }

    public void Execute()
    {
        if (action != null && IsAlive)
        {
            action();
        }
    }

    public void MarkForDeletion()
    {
        reference = null;
    }
}
