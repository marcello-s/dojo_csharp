#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataEventSync;

[TestFixture]
public class RulesEngineFixture
{
    private IEventAggregator events = null!;
    private RulesEngine<string> engine = null!;

    [SetUp]
    public void Setup()
    {
        events = new EventAggregator();
        engine = new RulesEngine<string>(events);
    }

    [Test]
    public void InstanceTest()
    {
        Assert.That(engine, Is.InstanceOf<RulesEngine<string>>());
    }

    [Test]
    public void FaultyRule1Test()
    {
        var recipient = new Recipient();
        events.Subscribe(recipient);
        var rule = new FaultyRule1();
        engine.AddRule(rule);
        events.Publish("something");
        Assert.That(rule.Error, Is.InstanceOf<NotImplementedException>());
    }

    [Test]
    public void FaultyRule2Test()
    {
        var recipient = new Recipient();
        events.Subscribe(recipient);
        var rule = new FaultyRule2();
        engine.AddRule(rule);
        events.Publish("something");
        Assert.That(recipient.Message, Is.EqualTo("always"));
        engine.RemoveRule(rule);
        events.Unsubscribe(recipient);
    }

    [Test]
    public void Rule1Test()
    {
        var recipient = new Recipient();
        events.Subscribe(recipient);
        var rule = new Rule1();
        engine.AddRule(rule);
        events.Publish("$event1");
        events.Publish("$event3");
        events.Publish("$event2");
        Assert.That(recipient.Message, Is.EqualTo("$rule1"));
        engine.RemoveRule(rule);
        events.Unsubscribe(recipient);
    }

    [Test]
    public void Rule2Test1()
    {
        var recipient = new Recipient();
        events.Subscribe(recipient);
        var rule = new Rule2();
        engine.AddRule(rule);
        events.Publish("$event4");
        Assert.That(recipient.Message, Is.EqualTo("$rule2"));
        engine.RemoveRule(rule);
        events.Unsubscribe(recipient);
    }

    [Test]
    public void Rule2Test2()
    {
        var recipient = new Recipient();
        events.Subscribe(recipient);
        var rule = new Rule2();
        engine.AddRule(rule);
        events.Publish("$event5");
        Assert.That(recipient.Message, Is.EqualTo("$rule2"));
        engine.RemoveRule(rule);
        events.Unsubscribe(recipient);
    }

    [Test]
    public void Rule3Test()
    {
        var recipient = new Recipient();
        events.Subscribe(recipient);
        var rule1 = new Rule1();
        var rule2 = new Rule2();
        var rule3 = new Rule3();
        engine.AddRule(rule1);
        engine.AddRule(rule2);
        engine.AddRule(rule3);
        events.Publish("$event1");
        events.Publish("$event3");
        events.Publish("$event2");
        events.Publish("$event4");
        Assert.That(recipient.Message, Is.EqualTo("$rule3"));
        engine.RemoveRule(rule1);
        engine.RemoveRule(rule2);
        engine.RemoveRule(rule3);
        events.Unsubscribe(recipient);
    }

    internal class FaultyRule1 : Rule<string>
    {
        protected override object? DoEvaluate(string message)
        {
            return base.DoEvaluate(message);
        }
    }

    internal class FaultyRule2 : Rule<string>
    {
        protected override object? DoEvaluate(string message)
        {
            return "always";
        }
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
