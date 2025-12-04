#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataEventSync;

[TestFixture]
public class UserRulesEngineFixture
{
    private IEventAggregator events = null!;
    private RulesEngine<User> engine = null!;

    [SetUp]
    public void Setup()
    {
        events = new EventAggregator();
        engine = new RulesEngine<User>(events);
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(engine, Is.InstanceOf<RulesEngine<User>>());
    }

    [Test]
    public void Rule4Test()
    {
        var recipient = new Recipient();
        events.Subscribe(recipient);
        var rule = new Rule4();
        engine.AddRule(rule);
        events.Publish(new User("Fred"));
        events.Publish(new User("Mike"));
        Assert.That(recipient.Message, Is.EqualTo("$rule4"));
        engine.RemoveRule(rule);
        events.Unsubscribe(recipient);
    }

    [Test]
    public void Rule5Test()
    {
        var recipient = new Recipient();
        events.Subscribe(recipient);
        var rule = new Rule5();
        engine.AddRule(rule);
        events.Publish(new User("Fred", true));
        Assert.That(recipient.Message, Is.Null);
        events.Publish(new User("Fred"));
        Assert.That(recipient.Message, Is.Null);
        events.Publish(new User("Mike", true));
        Assert.That(recipient.Message, Is.Null);
        events.Publish(new User("Fred", true));
        Assert.That(recipient.Message, Is.EqualTo("$rule5"));
        engine.RemoveRule(rule);
        events.Unsubscribe(recipient);
    }

    internal class Recipient : IHandle<string>
    {
        public string? Message { get; private set; }

        public void Handle(string message)
        {
            Message = message;
        }
    }
}
