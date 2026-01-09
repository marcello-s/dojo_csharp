#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public class CommandBindings
{
    public ICommand Command { get; protected set; }
    public IEnumerable<ICommandHandler> HandlerInstances { get; protected set; }

    public CommandBindings(ICommand command, IEnumerable<ICommandHandler> handlerInstances)
    {
        if (command == null)
        {
            throw new ArgumentNullException("command");
        }

        if (handlerInstances == null)
        {
            throw new ArgumentNullException("handlerInstances");
        }

        Command = command;
        HandlerInstances = handlerInstances;
    }
}
