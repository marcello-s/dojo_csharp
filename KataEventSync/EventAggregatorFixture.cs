#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataEventSync
{
    [TestFixture]
    public class EventAggregatorFixture
    {
        IEventAggregator events = null!;
        Recipient? recipient = null!;

        [SetUp]
        public void Setup()
        {
            events = new EventAggregator();
            recipient = new Recipient();
        }

        [Test]
        public void InstanceTest()
        {
            Assert.That(events, Is.InstanceOf<EventAggregator>());
        }

        [Test]
        public void SubscribeTest()
        {
            events.Subscribe(recipient);
            // subscribe again
            events.Subscribe(recipient);
        }

        [Test]
        public void UnsubscribeTest()
        {
            events.Unsubscribe(recipient);
        }

        [Test]
        public void SubscribeUnsubscribeTest()
        {
            events.Subscribe(recipient);
            events.Unsubscribe(recipient);
            // unsubscribe again
            events.Unsubscribe(recipient);
        }

        [Test]
        public void PublishTest()
        {
            events.Subscribe(recipient);
            events.Publish(100);
            Assert.That(recipient?.Message, Is.EqualTo(100));
            events.Unsubscribe(recipient);
            events.Publish(200);
            Assert.That(recipient?.Message, Is.EqualTo(100));
        }

        [Test]
        public void BroadcastTest()
        {
            var newRecipient = new Recipient();
            events.Subscribe(recipient);
            events.Subscribe(newRecipient);
            events.Publish(100);
            Assert.That(recipient?.Message, Is.EqualTo(100));
            Assert.That(newRecipient.Message, Is.EqualTo(100));
            events.Unsubscribe(recipient);
            events.Publish(200);
            Assert.That(recipient?.Message, Is.EqualTo(100));
            Assert.That(newRecipient.Message, Is.EqualTo(200));
            events.Unsubscribe(newRecipient);
        }

        [Test]
        public void NullReferenceTest()
        {
            var newRecipient = new Recipient();
            events.Subscribe(recipient);
            events.Subscribe(newRecipient);
            events.Publish(100);
            Assert.That(recipient?.Message, Is.EqualTo(100));
            Assert.That(newRecipient.Message, Is.EqualTo(100));
            recipient = null;
            GC.Collect();
            events.Publish(200);
            Assert.That(newRecipient.Message, Is.EqualTo(200));
            events.Unsubscribe(recipient);
            events.Unsubscribe(newRecipient);
        }

        [Test]
        public void FaultyRecipientTest()
        {
            var faultyRecipient = new FaultyRecipient();
            events.Subscribe(faultyRecipient);
            events.Subscribe(recipient);
            try
            {
                events.Publish(100);
            }
            catch (Exception ex)
            {
                Assert.That(ex, Is.InstanceOf<System.Reflection.TargetInvocationException>());
                Assert.That(ex.InnerException, Is.InstanceOf<NotImplementedException>());
            }

            Assert.That(recipient?.Message, Is.EqualTo(100));
        }

        internal class Recipient : IHandle<int>, IHandle<string>
        {
            public int Message { get; private set; }

            public void Handle(int message)
            {
                Message = message;
            }

            public void Handle(string message)
            {
                throw new NotImplementedException();
            }
        }

        internal class FaultyRecipient : IHandle<int>
        {
            public void Handle(int message)
            {
                throw new NotImplementedException();
            }
        }
    }
}
