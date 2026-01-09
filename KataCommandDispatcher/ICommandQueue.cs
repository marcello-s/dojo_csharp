#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public interface ICommandQueue
{
    void Enqueue<TCommand>(TCommand? command)
        where TCommand : class, ICommand;
    CommandBindings Dequeue(CancellationToken token);
}
