#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public abstract class Command : ICommand
{
    public void InvokeResultHandler(ICommandResult result)
    {
        if (ResultHandler != null)
        {
            ResultHandler(result);
        }
    }

    public void InvokeExceptionHandler(Exception exception)
    {
        if (ExceptionHandler != null)
        {
            ExceptionHandler(exception);
        }
    }

    public Action<ICommandResult> ResultHandler = result => { };
    public Action<Exception> ExceptionHandler = ex => { };
}
