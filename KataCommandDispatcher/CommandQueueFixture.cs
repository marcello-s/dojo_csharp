#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataCommandDispatcher;

[TestFixture]
public class CommandQueueFixture
{
    private IDependencyResolver dependencyResolver = null!;
    private ICommandQueue commandQueue = null!;

    [SetUp]
    public void Setup()
    {
        dependencyResolver = new MockResolver();
        commandQueue = new CommandQueue<ICommand>(dependencyResolver);
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(commandQueue, Is.InstanceOf<CommandQueue<ICommand>>());
    }

    [Test]
    public void DependencyResolverNullTest()
    {
        Assert.Throws<ArgumentNullException>(() => new CommandQueue<ICommand>(null));
    }

    [Test]
    public void EnqueueNullTest()
    {
        ConsoleCommand? command = null;

        Assert.Throws<ArgumentNullException>(() => commandQueue.Enqueue(command));
    }

    [Test]
    public void EnqueueDequeueTest()
    {
        var myCommand = new ConsoleCommand("test command");
        commandQueue.Enqueue(myCommand);
        var cancelToken = new CancellationTokenSource();
        var commandAndHandlersItem = commandQueue.Dequeue(cancelToken.Token);
        Assert.That(commandAndHandlersItem.Command, Is.EqualTo(myCommand));
        Assert.That(commandAndHandlersItem.HandlerInstances.Count(), Is.EqualTo(1));
    }

    /*
    [Test]
    public void NoHandlerFoundTest()
    {
        Assert.Throws<ArgumentException>(() => commandQueue.Enqueue(new TestCommand()));
    }
    */

    [Test]
    public void EmptyResolverTest()
    {
        var q = new CommandQueue<ICommand>(new EmptyResolver());

        Assert.Throws<InvalidOperationException>(() => q.Enqueue(new TestCommand()));
    }

    [Test]
    public void NullResolverTest()
    {
        var q = new CommandQueue<ICommand>(new NullResolver());

        Assert.Throws<InvalidOperationException>(() => q.Enqueue(new TestCommand()));
    }

    [Test]
    public void BadResolverTest()
    {
        var q = new CommandQueue<ICommand>(new BadResolver());

        Assert.Throws<NotImplementedException>(() => q.Enqueue(new TestCommand()));
    }

    private class MockResolver : IDependencyResolver
    {
        public IEnumerable<object> GetAll<T>()
        {
            return new List<object>() { new ConsoleCommandHandler() };
        }
    }

    private class EmptyResolver : IDependencyResolver
    {
        public IEnumerable<object> GetAll<T>()
        {
            return new List<object>();
        }
    }

    private class NullResolver : IDependencyResolver
    {
        public IEnumerable<object>? GetAll<T>()
        {
            return null;
        }
    }

    private class BadResolver : IDependencyResolver
    {
        public IEnumerable<object> GetAll<T>()
        {
            throw new NotImplementedException();
        }
    }

    private class TestCommand : Command { }
}
