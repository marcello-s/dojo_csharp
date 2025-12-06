#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Collections.Concurrent;

namespace KataCommandDispatcher;

public class CommandQueue<T> : ICommandQueue
    where T : class, ICommand
{
    private readonly IDependencyResolver dependencyResolver;
    private readonly BlockingCollection<CommandBindings> commandQueue;

    public CommandQueue(IDependencyResolver? dependencyResolver)
    {
        if (dependencyResolver == null)
        {
            throw new ArgumentNullException("dependencyResolver");
        }

        this.dependencyResolver = dependencyResolver;
        commandQueue = new BlockingCollection<CommandBindings>(
            new ConcurrentQueue<CommandBindings>()
        );
    }

    public void Enqueue<TCommand>(TCommand? command)
        where TCommand : class, ICommand
    {
        if (command == null)
        {
            throw new ArgumentNullException("command");
        }

        // resolve handlers and add to queue
        var handlerInstances = dependencyResolver.GetAll<ICommandHandler<TCommand>>();
        if (handlerInstances == null || !handlerInstances.Any())
        {
            throw new InvalidOperationException(
                string.Format("no handlers found for command of type '{0}'", command.GetType().Name)
            );
        }

        var handlers = handlerInstances.Cast<ICommandHandler>().ToList();
        commandQueue.Add(new CommandBindings(command, handlers));
    }

    public CommandBindings Dequeue(CancellationToken token)
    {
        return commandQueue.Take(token);
    }
}
