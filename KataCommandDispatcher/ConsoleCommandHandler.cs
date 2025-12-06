#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public class ConsoleCommandHandler : ICommandHandler<ConsoleCommand>
{
    public ICommandResult Execute(ICommand command)
    {
        var consoleCommand = (ConsoleCommand)command;
        Console.WriteLine(consoleCommand.Text);
        return new CommandResult();
    }
}
