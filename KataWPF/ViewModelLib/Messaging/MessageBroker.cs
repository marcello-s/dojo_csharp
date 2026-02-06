#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;

namespace ViewModelLib.Messaging;

[Export(typeof(IMessageBroker))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class MessageBroker : IMessageBroker
{
    private Dictionary<Type, List<WeakActionAndToken>> recipientsAction = null!;

    public virtual void Register<T>(object recipient, Action<T> action)
    {
        Register(recipient, null, action);
    }

    public virtual void Unregister<T>(object recipient)
    {
        Unregister<T>(recipient, null);
    }

    public virtual void Unregister(object recipient)
    {
        UnregisterFromList(recipient, recipientsAction);
    }

    public virtual void Unregister<T>(object recipient, Action<T>? action)
    {
        UnregisterFromList(recipient, action, recipientsAction);
    }

    public virtual void Send<T>(T message)
    {
        SendToTargetOrType(message, null, null);
    }

    public virtual void Send<TMessage, TTarget>(TMessage message)
    {
        SendToTargetOrType(message, typeof(TTarget), null);
    }

    private void Register<T>(object recipient, object? token, Action<T> action)
    {
        var messageType = typeof(T);
        Dictionary<Type, List<WeakActionAndToken>> recipients;

        if (recipientsAction == null)
        {
            recipientsAction = new Dictionary<Type, List<WeakActionAndToken>>();
        }

        recipients = recipientsAction;

        List<WeakActionAndToken> actionList;
        if (!recipients.ContainsKey(messageType))
        {
            actionList = new List<WeakActionAndToken>();
            recipients.Add(messageType, actionList);
        }
        else
        {
            actionList = recipients[messageType];
        }

        var weakAction = new WeakActionGeneric<T>(recipient, action);
        var item = new WeakActionAndToken { Action = weakAction, Token = token };
        actionList.Add(item);

        Cleanup();
    }

    private void SendToTargetOrType<T>(T message, Type? messageTargetType, object? token)
    {
        var messageType = typeof(T);

        if (recipientsAction != null)
        {
            if (recipientsAction.ContainsKey(messageType))
            {
                var actionList = recipientsAction[messageType];
                SendToList(message, actionList, messageTargetType, token);
            }
        }

        Cleanup();
    }

    private static void SendToList<T>(
        T message,
        IEnumerable<WeakActionAndToken> actionList,
        Type? messageTargetType,
        object? token
    )
    {
        if (actionList != null)
        {
            var actionListCopy = actionList.Take(actionList.Count()).ToList();
            foreach (var item in actionListCopy)
            {
                var executeAction = item.Action as IExecuteWithObject;
                if (
                    executeAction != null
                    && item.Action.IsAlive
                    && item.Action.Target != null
                    && (
                        messageTargetType == null
                        || item.Action.Target.GetType() == messageTargetType
                        || Implements(item.Action.Target.GetType(), messageTargetType)
                    )
                    && (
                        (item.Token == null && token == null)
                        || item.Token != null && item.Token.Equals(token)
                    )
                )
                {
                    executeAction.ExecuteWithObject(message);
                }
            }
        }
    }

    private static void UnregisterFromList(
        object recipient,
        Dictionary<Type, List<WeakActionAndToken>> actionList
    )
    {
        if (recipient == null || actionList == null || actionList.Count == 0)
        {
            return;
        }

        lock (actionList)
        {
            foreach (var messageType in actionList.Keys)
            {
                foreach (var item in actionList[messageType])
                {
                    var weakAction = item.Action;

                    if (weakAction != null && recipient == weakAction.Target)
                    {
                        weakAction.MarkForDeletion();
                    }
                }
            }
        }
    }

    private static void UnregisterFromList<T>(
        object recipient,
        Action<T>? action,
        Dictionary<Type, List<WeakActionAndToken>> actionList
    )
    {
        var messageType = typeof(T);

        if (
            recipient == null
            || actionList == null
            || actionList.Count == 0
            || !actionList.ContainsKey(messageType)
        )
        {
            return;
        }

        lock (actionList)
        {
            foreach (var item in actionList[messageType])
            {
                var weakActionCasted = item.Action as WeakActionGeneric<T>;

                if (
                    weakActionCasted != null
                    && recipient == weakActionCasted.Target
                    && (action == null || action == weakActionCasted.Action)
                )
                {
                    item.Action.MarkForDeletion();
                }
            }
        }
    }

    private static bool Implements(Type instanceType, Type interfaceType)
    {
        if (instanceType == null || interfaceType == null)
        {
            return false;
        }

        var interfaces = instanceType.GetInterfaces();
        foreach (var currentInterface in interfaces)
        {
            if (currentInterface == interfaceType)
            {
                return true;
            }
        }

        return false;
    }

    private void Cleanup()
    {
        CleanupList(recipientsAction);
    }

    private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> actionLists)
    {
        if (actionLists == null)
        {
            return;
        }

        var listsToRemove = new List<Type>();
        foreach (var list in actionLists)
        {
            var recipientsToRemove = new List<WeakActionAndToken>();
            foreach (var item in list.Value)
            {
                if (item.Action == null || !item.Action.IsAlive)
                {
                    recipientsToRemove.Add(item);
                }
            }

            foreach (var recipient in recipientsToRemove)
            {
                list.Value.Remove(recipient);
            }

            if (list.Value.Count == 0)
            {
                listsToRemove.Add(list.Key);
            }
        }

        foreach (var key in listsToRemove)
        {
            actionLists.Remove(key);
        }
    }

    private struct WeakActionAndToken
    {
        public WeakAction Action;
        public object? Token;
    }
}
