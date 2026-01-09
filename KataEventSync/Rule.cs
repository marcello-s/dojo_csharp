#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

public abstract class Rule<T>
{
    public Exception? Error { get; private set; }

    public virtual object? Evaluate(T message)
    {
        // TODO run through exception filter to notify owner of faulty implementation
        object? returnValue = null;
        try
        {
            returnValue = DoEvaluate(message);
        }
        catch (Exception ex)
        {
            Error = ex;
        }

        return returnValue;
    }

    protected virtual object? DoEvaluate(T message)
    {
        throw new NotImplementedException();
    }
}
