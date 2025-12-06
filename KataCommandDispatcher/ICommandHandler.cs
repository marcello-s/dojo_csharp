#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public interface ICommandHandler
{
    ICommandResult? Execute(ICommand command);
}

public interface ICommandHandler<in T> : ICommandHandler
    where T : ICommand { }
