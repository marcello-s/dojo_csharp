#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Runtime.CompilerServices;
using NUnit.Framework;
using ViewModelLib.Messaging;

namespace ViewModelLib.Test.Messaging;

[TestFixture]
public class MessageBrokerTest
{
    private IMessageBroker messageBroker = null!;

    [SetUp]
    public void Setup()
    {
        messageBroker = new MessageBroker();
    }

    public string MyContent { get; set; } = string.Empty;

    [Test]
    public void RegisterRecipientMessage()
    {
        const string content = "asdf";
        MyContent = string.Empty;
        messageBroker.Register<TestMessage>(this, m => MyContent = m.Content);
        Assert.That(string.IsNullOrEmpty(MyContent), Is.True);
        messageBroker.Send(new TestMessage { Content = content });
        Assert.That(MyContent.Equals(content), Is.True);
    }

    [Test]
    public void RegisterRecipientInstanceAction()
    {
        const string content = "qwer";
        var recipient = new TestRecipient();
        messageBroker.Register<string>(recipient, recipient.ReceiveMessage);
        Assert.That(string.IsNullOrEmpty(recipient.ContentString), Is.True);
        messageBroker.Send(content);
        Assert.That(recipient.ContentString.Equals(content), Is.True);
    }

    [Test]
    public void RegisterMultipleRecipients()
    {
        const string content = "yxcv";
        var recipient1 = new TestRecipient();
        var recipient2 = new TestRecipient();
        messageBroker.Register<string>(recipient1, recipient1.ReceiveMessage);
        messageBroker.Register<string>(recipient2, recipient2.ReceiveMessage);
        Assert.That(string.IsNullOrEmpty(recipient1.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient2.ContentString), Is.True);
        messageBroker.Send(content);
        Assert.That(recipient1.ContentString.Equals(content), Is.True);
        Assert.That(recipient2.ContentString.Equals(content), Is.True);
    }

    [Test]
    public void UnregisterRecipient()
    {
        const string content = "asdf";
        MyContent = string.Empty;
        messageBroker.Register<TestMessage>(this, m => MyContent = m.Content);
        messageBroker.Unregister(this);
        Assert.That(string.IsNullOrEmpty(MyContent), Is.True);
        messageBroker.Send(new TestMessage { Content = content });
        Assert.That(string.IsNullOrEmpty(MyContent), Is.True);
    }

    [Test]
    public void UnregisterRecipientMessage()
    {
        const string content = "asdf";
        MyContent = string.Empty;
        messageBroker.Register<TestMessage>(this, m => MyContent = m.Content);
        messageBroker.Unregister<TestMessage>(this);
        Assert.That(string.IsNullOrEmpty(MyContent), Is.True);
        messageBroker.Send(new TestMessage { Content = content });
        Assert.That(string.IsNullOrEmpty(MyContent), Is.True);
    }

    [Test]
    public void UnregisterMultipleRecipients()
    {
        const string content1 = "yxcv";
        const string content2 = "uiop";
        var recipient1 = new TestRecipient();
        var recipient2 = new TestRecipient();
        messageBroker.Register<string>(recipient1, recipient1.ReceiveMessage);
        messageBroker.Register<string>(recipient2, recipient2.ReceiveMessage);
        Assert.That(string.IsNullOrEmpty(recipient1.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient2.ContentString), Is.True);
        messageBroker.Send(content1);
        Assert.That(recipient1.ContentString.Equals(content1), Is.True);
        Assert.That(recipient2.ContentString.Equals(content1), Is.True);
        messageBroker.Unregister<string>(recipient2);
        messageBroker.Send(content2);
        Assert.That(recipient1.ContentString.Equals(content2), Is.True);
        Assert.That(recipient2.ContentString.Equals(content2), Is.False);
    }

    public class TestMessage
    {
        public string Content { get; set; } = string.Empty;
    }

    private class TestRecipient
    {
        public string ContentString { get; private set; } = string.Empty;

        public void ReceiveMessage(string message)
        {
            ContentString = message;
        }
    }
}
