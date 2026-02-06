#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion


using NUnit.Framework;
using ViewModelLib.Messaging;

namespace ViewModelLib.Test.Messaging;

[TestFixture]
public class MessageBrokerWithTypeTest
{
    private IMessageBroker messageBroker = null!;

    [SetUp]
    public void Setup()
    {
        messageBroker = new MessageBroker();
    }

    [Test]
    public void SendToTypeTest()
    {
        const string content1 = "abcd";
        const string content2 = "efgh";
        var recipient11 = new TestRecipient1();
        var recipient12 = new TestRecipient1();
        var recipient21 = new TestRecipient2();
        var recipient22 = new TestRecipient2();
        messageBroker.Register<string>(recipient11, recipient11.ReceiveContent);
        messageBroker.Register<string>(recipient12, recipient12.ReceiveContent);
        messageBroker.Register<string>(recipient21, recipient21.ReceiveContent);
        messageBroker.Register<string>(recipient22, recipient22.ReceiveContent);
        Assert.That(string.IsNullOrEmpty(recipient11.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient12.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient21.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient22.ContentString), Is.True);
        messageBroker.Send<string, TestRecipient1>(content1);
        Assert.That(recipient11.ContentString.Equals(content1), Is.True);
        Assert.That(recipient12.ContentString.Equals(content1), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient21.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient22.ContentString), Is.True);
        messageBroker.Send<string, TestRecipient2>(content2);
        Assert.That(recipient11.ContentString.Equals(content1), Is.True);
        Assert.That(recipient12.ContentString.Equals(content1), Is.True);
        Assert.That(recipient21.ContentString.Equals(content2), Is.True);
        Assert.That(recipient22.ContentString.Equals(content2), Is.True);
    }

    [Test]
    public void SendToInterfaceTest()
    {
        const string content1 = "abcd";
        var recipient11 = new TestRecipient1();
        var recipient12 = new TestRecipient1();
        var recipient21 = new TestRecipient2();
        var recipient22 = new TestRecipient2();
        var recipient31 = new TestRecipient3();
        var recipient32 = new TestRecipient3();
        messageBroker.Register<string>(recipient11, recipient11.ReceiveContent);
        messageBroker.Register<string>(recipient12, recipient12.ReceiveContent);
        messageBroker.Register<string>(recipient21, recipient21.ReceiveString);
        messageBroker.Register<string>(recipient22, recipient22.ReceiveString);
        messageBroker.Register<string>(recipient31, recipient31.ReceiveString);
        messageBroker.Register<string>(recipient32, recipient32.ReceiveString);
        Assert.That(string.IsNullOrEmpty(recipient11.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient12.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient21.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient22.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient31.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient32.ContentString), Is.True);
        messageBroker.Send<string, ITestRecipient>(content1);
        Assert.That(string.IsNullOrEmpty(recipient11.ContentString), Is.True);
        Assert.That(string.IsNullOrEmpty(recipient12.ContentString), Is.True);
        Assert.That(recipient21.ContentString.Equals(content1), Is.True);
        Assert.That(recipient22.ContentString.Equals(content1), Is.True);
        Assert.That(recipient31.ContentString.Equals(content1), Is.True);
        Assert.That(recipient32.ContentString.Equals(content1), Is.True);
    }

    private interface ITestRecipient
    {
        void ReceiveString(string message);
    }

    private class TestRecipient1
    {
        public string ContentString { get; private set; } = string.Empty;

        public void ReceiveContent(string message)
        {
            ContentString = message;
        }
    }

    private class TestRecipient2 : ITestRecipient
    {
        public string ContentString { get; private set; } = string.Empty;

        public void ReceiveContent(string message)
        {
            ContentString = message;
        }

        public void ReceiveString(string message)
        {
            ContentString = message;
        }
    }

    private class TestRecipient3 : ITestRecipient
    {
        public string ContentString { get; private set; } = string.Empty;

        public void ReceiveString(string message)
        {
            ContentString = message;
        }
    }
}
