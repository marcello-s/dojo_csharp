#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public interface ICommand
{
    void InvokeResultHandler(ICommandResult result);
    void InvokeExceptionHandler(Exception exception);
}
