#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataBootstrapper;

internal static class IoC
{
    internal static Func<Type, string, object?> GetInstance = (type, key) => null;
    internal static Func<Type, IEnumerable<object>?> GetAllInstances = type => null;
    internal static Action<object> BuildUp = instance => { };
    internal static Action<Type, Type> RegisterService = (ti, ts) => { };
    internal static Action<Type, Type> RegisterServiceSingleton = (ti, ts) => { };

    internal static T? Get<T>()
    {
        return (T?)GetInstance(typeof(T?), null!);
    }

    internal static T? Get<T>(string key)
    {
        return (T?)GetInstance(typeof(T?), key);
    }

    internal static void IocRegisterService<TInterface, TService>()
    {
        RegisterService(typeof(TInterface), typeof(TService));
    }

    internal static void IocRegisterServiceSingleton<TInterface, TService>()
    {
        RegisterServiceSingleton(typeof(TInterface), typeof(TService));
    }
}
