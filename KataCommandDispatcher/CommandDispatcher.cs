#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly ICommandQueue commandQueue;
    private readonly Task dispatcherTask;
    private readonly CancellationTokenSource cancelToken;

    public CommandDispatcher(ICommandQueue? commandQueue)
    {
        if (commandQueue == null)
        {
            throw new ArgumentNullException("commandQueue");
        }

        this.commandQueue = commandQueue;
        cancelToken = new CancellationTokenSource();
        dispatcherTask = new Task(Dispatch, cancelToken.Token);
        dispatcherTask.Start();
    }

    private void Dispatch()
    {
        try
        {
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    var commandAndHandlers = commandQueue.Dequeue(cancelToken.Token);
                    ExecuteCommandHandler(commandAndHandlers);
                }
                catch (OperationCanceledException opex)
                {
                    Console.WriteLine("command queue: '{0}'", opex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("dispatcher continuous execution");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void ExecuteCommandHandler(CommandBindings commandAndHandlers)
    {
        foreach (var handlerInstance in commandAndHandlers.HandlerInstances)
        {
            // catch errors of the handler and return to command owner
            try
            {
                var returnValue = handlerInstance.Execute(commandAndHandlers.Command);
                var result = returnValue;
                if (result == null || !result.Success)
                {
                    Console.WriteLine(
                        "command execution unknown or unsuccessful '{0}'[{1}] for command '{2}'[{3}]",
                        handlerInstance.GetType().Name,
                        handlerInstance.GetHashCode(),
                        commandAndHandlers.Command.GetType().Name,
                        commandAndHandlers.Command.GetHashCode()
                    );
                    result = new CommandResult(false);
                }

                commandAndHandlers.Command.InvokeResultHandler(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    string.Format(
                        "command handler execution failed on instance '{0}'[{1}] for command '{2}'[{3}]",
                        handlerInstance.GetType().Name,
                        handlerInstance.GetHashCode(),
                        commandAndHandlers.Command.GetType().Name,
                        commandAndHandlers.Command.GetHashCode()
                    )
                );
                commandAndHandlers.Command.InvokeExceptionHandler(ex);
            }
        }
    }

    public void Shutdown()
    {
        if (cancelToken.IsCancellationRequested)
        {
            return;
        }

        cancelToken.Cancel();
        dispatcherTask.Wait();
        cancelToken.Dispose();
        dispatcherTask.Dispose();
    }
}
