#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataEventAggregator;

[TestFixture]
public class EventAggregatorTests
{
    private IEventAggregator eventAggregator = null!;

    [SetUp]
    public void Setup()
    {
        eventAggregator = new EventAggregator();
    }

    [Test]
    public void CanCreate_Instance()
    {
        Assert.That(eventAggregator, Is.InstanceOf<EventAggregator>());
    }

    [Test]
    public void PublishSubscribe_WithInt_MessageDelivered()
    {
        var intEvent = eventAggregator.GetEvent<int>();

        var message = 0;
        var intObserver = new IntObserver();
        var unsubscriber1 = intEvent.Subscribe(intObserver);
        var unsubscriber2 = intEvent.Subscribe(m => message = m, ThreadOption.UiThread);

        eventAggregator.Publish(100);
        Assert.That(message, Is.EqualTo(100));
        eventAggregator.Publish(200);
        Assert.That(message, Is.EqualTo(200));

        unsubscriber1.Dispose();
        unsubscriber2.Dispose();
    }

    [Test]
    public void Subscribe_WithDisposeTwice_DoesNotThrowException()
    {
        var intEvent = eventAggregator.GetEvent<int>();
        var unsubscriber = intEvent.Subscribe(new IntObserver());
        unsubscriber.Dispose();
        unsubscriber.Dispose();
    }

    [Test]
    public void PublishSubscriber_WithSubscriberThrows_ThrowsException()
    {
        var stringEvent = eventAggregator.GetEvent<string>();
        stringEvent.Subscribe(s =>
        {
            throw new NotSupportedException();
        });

        Assert.Throws<NotSupportedException>(() => eventAggregator.Publish("message"));
    }

    [Test]
    public void PublishSubscriber_WithTwoSubscriber_OneThrows_ThrowsException_Message_Delivered()
    {
        var stringEvent = eventAggregator.GetEvent<string>();
        string message = string.Empty;
        Exception? error = null;

        var unsubcriber1 = stringEvent.Subscribe(s =>
        {
            throw new NotSupportedException();
        });
        var unsubcriber2 = stringEvent.Subscribe(m => message = m);

        try
        {
            eventAggregator.Publish("message");
        }
        catch (Exception ex)
        {
            error = ex;
        }

        Assert.That(string.IsNullOrEmpty(message), Is.False);
        Assert.That(error, Is.InstanceOf<NotSupportedException>());

        unsubcriber1.Dispose();
        unsubcriber2.Dispose();
    }

    [Test]
    public void Subscribe_WithSubscriberNull_ThrowsException()
    {
        var intEvent = eventAggregator.GetEvent<int>();
        IntObserver? nullObserver = null;

        Assert.Throws<ArgumentNullException>(() => intEvent.Subscribe(nullObserver!));
    }

    [Test]
    public void Subscribe_WithActionNull_ThrowsException()
    {
        var intEvent = eventAggregator.GetEvent<int>();
        Action<int>? nullAction = null;

        Assert.Throws<ArgumentNullException>(() => intEvent.Subscribe(nullAction!));
    }

    [Test]
    public void PublishSubcribe_WithNull_NullSend()
    {
        var stringEvent = eventAggregator.GetEvent<string?>();
        const string InitialMessage = "initial_message";
        var message = InitialMessage;
        var unsubscriber = stringEvent.Subscribe(m => message = m);
        eventAggregator.Publish<string?>(null);
        Assert.That(string.IsNullOrEmpty(message), Is.True);
        unsubscriber.Dispose();
    }

    [Test]
    public void PublishSubscribe_WithCollectedObserver_ReferencesArePrunedCorrectly()
    {
        var intEvent = eventAggregator.GetEvent<int>();
        var observer1 = new IntObserver();
        var observer2 = new IntObserver();
        var unsubscriber1 = intEvent.Subscribe(observer1);
        var unsubscriber2 = intEvent.Subscribe(observer2);
        eventAggregator.Publish(100);
        Assert.That(observer1.Message, Is.EqualTo(100));
        Assert.That(observer2.Message, Is.EqualTo(100));

        observer1 = null;
        unsubscriber1 = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();

        eventAggregator.Publish(101);
        Assert.That(observer2.Message, Is.EqualTo(101));

        unsubscriber2.Dispose();
    }

    /*
    [Test]
    public void PublishSubscribe_WithCollectedUnsubscriber_ReferencesArePrunedCorrectly()
    {
        var intEvent = eventAggregator.GetEvent<int>();
        var message1 = 0;
        var message2 = 0;
        var unsubscriber1 = intEvent.Subscribe(m => message1 = m);
        var unsubscriber2 = intEvent.Subscribe(m => message2 = m);
        eventAggregator.Publish(100);
        Assert.That(message1, Is.EqualTo(100));
        Assert.That(message2, Is.EqualTo(100));

        unsubscriber1 = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        // garbage collector works differently in dotnet core -> test is disabled

        eventAggregator.Publish(101);
        Assert.That(message1, Is.EqualTo(100));
        Assert.That(message2, Is.EqualTo(101));

        unsubscriber2.Dispose();
    }
    */

    [TestCase(100000), Explicit]
    public void PublishSubscribe_WithObservers_ObserveMemoryConsumption(int numberOfObjects)
    {
        var intEvent = eventAggregator.GetEvent<int>();
        var observerList = new List<IntObserver>();
        var unsubscriberList = new List<IDisposable>();

        Console.Out.WriteLine("creating {0} objects..", numberOfObjects);
        for (var i = 0; i < numberOfObjects; i++)
        {
            var observer = new IntObserver();
            var unsubscriber = intEvent.Subscribe(observer);
            observerList.Add(observer);
            unsubscriberList.Add(unsubscriber);
        }

        Console.Out.WriteLine("publishing 1 message..");
        eventAggregator.Publish(100);

        Console.Out.WriteLine("asserting message has been delivered..");
        for (var i = 0; i < numberOfObjects; i++)
        {
            Assert.That(observerList[i].Message, Is.EqualTo(100));
        }

        Console.Out.WriteLine("memory consumption: {0} bytes", Environment.WorkingSet);

        var numberOfObjectsToRemove = numberOfObjects / 2;
        Console.Out.WriteLine("removing {0} objects..", numberOfObjectsToRemove);
        for (var i = 0; i < numberOfObjectsToRemove; i++)
        {
            observerList.RemoveAt(i);
            unsubscriberList.RemoveAt(i);
        }

        Console.Out.WriteLine("collecting garbage..");
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.Out.WriteLine("publishing 1 message..");
        eventAggregator.Publish(101);

        Console.Out.WriteLine("asserting message has been delivered..");
        for (var i = 0; i < numberOfObjectsToRemove; i++)
        {
            Assert.That(observerList[i].Message, Is.EqualTo(101));
        }

        Console.Out.WriteLine("memory consumption: {0} bytes", Environment.WorkingSet);

        Console.Out.WriteLine("unsubscribing..");
        unsubscriberList.ForEach(u => u.Dispose());
    }

    private class IntObserver : IObserver<int>
    {
        public int Message { get; private set; }

        public void OnNext(int value)
        {
            Message = value;
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
