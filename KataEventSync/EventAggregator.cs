#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Reflection;

namespace KataEventSync;

public class EventAggregator : IEventAggregator
{
    public static Action<Action> DefaultPublicationThreadMarshaler = action => action();
    public Action<Action> PublicationThreadMarshaler { get; set; }
    private readonly List<Handler> handlers = new List<Handler>();

    public EventAggregator()
    {
        PublicationThreadMarshaler = DefaultPublicationThreadMarshaler;
    }

    public virtual void Subscribe(object? instance)
    {
        if (instance == null)
        {
            return;
        }

        lock (handlers)
        {
            if (handlers.Any(h => h.IsMatch(instance)))
            {
                return;
            }

            handlers.Add(new Handler(instance));
        }
    }

    public virtual void Unsubscribe(object? instance)
    {
        if (instance == null)
        {
            return;
        }

        lock (handlers)
        {
            var registeredInstance = handlers.FirstOrDefault(h => h.IsMatch(instance));
            if (registeredInstance != null)
            {
                handlers.Remove(registeredInstance);
            }
        }
    }

    public virtual void Publish(object message)
    {
        Publish(message, PublicationThreadMarshaler);
    }

    public virtual void Publish(object message, Action<Action> marshal)
    {
        lock (handlers)
        {
            marshal(() =>
            {
                var messageType = message.GetType();
                var deadHandlers = handlers.Where(h => !h.Handle(messageType, message)).ToList();
                if (deadHandlers.Any())
                {
                    foreach (var handler in deadHandlers)
                    {
                        handlers.Remove(handler);
                    }
                }
            });
        }
    }

    protected class Handler
    {
        private readonly WeakReference reference;
        private readonly Dictionary<Type, MethodInfo> supportedHandlers =
            new Dictionary<Type, MethodInfo>();

        public Handler(object handler)
        {
            reference = new WeakReference(handler);

            var interfaces = handler
                .GetType()
                .GetInterfaces()
                .Where(h => typeof(IHandle).IsAssignableFrom(h) && h.IsGenericType);
            foreach (var @interface in interfaces)
            {
                var type = @interface.GetGenericArguments()[0];
                var handleMethod = @interface.GetMethod("Handle");
                if (handleMethod != null)
                {
                    supportedHandlers[type] = handleMethod;
                }
            }
        }

        public bool IsMatch(object instance)
        {
            return reference.Target == instance;
        }

        public bool Handle(Type messageType, object message)
        {
            var target = reference.Target;
            if (target == null)
            {
                return false;
            }

            TargetInvocationException? lastException = null;
            foreach (
                var pair in supportedHandlers.Where(pair => pair.Key.IsAssignableFrom(messageType))
            )
            {
                try
                {
                    pair.Value.Invoke(target, new[] { message });
                }
                catch (TargetInvocationException ex)
                {
                    lastException = ex;
                }

                return true;
            }

            if (lastException != null)
            {
                throw lastException;
            }

            return true;
        }
    }
}
