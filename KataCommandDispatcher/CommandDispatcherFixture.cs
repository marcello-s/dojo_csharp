#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataCommandDispatcher;

[TestFixture]
public class CommandDispatcherFixture
{
    private IDependencyResolver dependencyResolver = null!;
    private ICommandQueue commandQueue = null!;
    private ICommandDispatcher dispatcher = null!;

    [SetUp]
    public void Setup()
    {
        dependencyResolver = new MockResolver();
        commandQueue = new CommandQueue<ICommand>(dependencyResolver);
        dispatcher = new CommandDispatcher(commandQueue);
    }

    [TearDown]
    public void Teardown()
    {
        try
        {
            ((CommandDispatcher)dispatcher).Shutdown();
        }
        catch (AggregateException ex)
        {
            Console.WriteLine(ex);
        }
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(commandQueue, Is.InstanceOf<CommandQueue<ICommand>>());
        Assert.That(dispatcher, Is.InstanceOf<CommandDispatcher>());
        Thread.Sleep(50);
    }

    [Test]
    public void QueueNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new CommandDispatcher(null));
    }

    [Test]
    public void EnqueueCommandTest()
    {
        var command = new ConsoleCommand("console command test");
        var resultSuccess = false;
        Exception? resultException = null;
        command.ResultHandler = result => resultSuccess = result.Success;
        command.ExceptionHandler = ex => resultException = ex;
        commandQueue.Enqueue(command);
        Thread.Sleep(50);
        Assert.That(resultSuccess, Is.True);
        Assert.That(resultException, Is.Null);
    }

    [Test]
    public void BadCommandHandlerTest()
    {
        var q = new CommandQueue<ICommand>(new BadCommandHandlerResolver());
        var d = new CommandDispatcher(q);
        var command = new TestCommand();
        var resultSuccess = true;
        Exception? resultException = null;
        command.ResultHandler = result => resultSuccess = result.Success;
        command.ExceptionHandler = ex => resultException = ex;
        q.Enqueue(command);
        Thread.Sleep(50);
        d.Shutdown();
        Assert.That(resultSuccess, Is.True);
        Assert.That(resultException, Is.InstanceOf<NotImplementedException>());
    }

    [Test]
    public void NullResultTest()
    {
        var q = new CommandQueue<ICommand>(new NullResultCommandHandlerResolver());
        var d = new CommandDispatcher(q);
        var command = new TestCommand();
        var resultSuccess = true;
        Exception? resultException = null;
        command.ResultHandler = result => resultSuccess = result.Success;
        command.ExceptionHandler = ex => resultException = ex;
        q.Enqueue(command);
        Thread.Sleep(50);
        d.Shutdown();
        Assert.That(resultSuccess, Is.False);
        Assert.That(resultException, Is.Null);
    }

    private class MockResolver : IDependencyResolver
    {
        public IEnumerable<object> GetAll<T>()
        {
            return new List<object>() { new ConsoleCommandHandler() };
        }
    }

    private class TestCommand : Command { }

    private class BadCommandHandler : ICommandHandler<TestCommand>
    {
        public ICommandResult Execute(ICommand command)
        {
            throw new NotImplementedException();
        }
    }

    private class BadCommandHandlerResolver : IDependencyResolver
    {
        public IEnumerable<object> GetAll<T>()
        {
            return new List<object>() { new BadCommandHandler() };
        }
    }

    private class NullResultCommandHandler : ICommandHandler<TestCommand>
    {
        public ICommandResult? Execute(ICommand command)
        {
            return null;
        }
    }

    private class NullResultCommandHandlerResolver : IDependencyResolver
    {
        public IEnumerable<object> GetAll<T>()
        {
            return new List<object>() { new NullResultCommandHandler() };
        }
    }
}
