#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Collections;
using System.Reflection;

namespace KataMicrokernel;

public class Microkernel
{
    private readonly Bindings bindings;
    private readonly Scope globalSingletonScope;
    private Binding binding;
    private bool hasBinding;

    public Microkernel()
    {
        bindings = new Bindings();
        globalSingletonScope = new Scope();
    }

    public Microkernel Bind<TAbstraction, TImplementation>()
    {
        binding = bindings.Add(typeof(TAbstraction), typeof(TImplementation));
        hasBinding = true;
        return this;
    }

    public void InGlobalSingletonScope()
    {
        if (!hasBinding)
        {
            return;
        }

        var newBinding = new Binding(binding)
        {
            ScopeHashCode = globalSingletonScope.GetHashCode(),
        };
        bindings.UpdateBinding(binding, newBinding);
        binding = newBinding;
    }

    public IEnumerable<T>? GetInstances<T>()
    {
        return GetInstances<T>(typeof(T))?.Cast<T>();
    }

    private IEnumerable? GetInstances<T>(Type type)
    {
        var cyclicDepedencyChecker = new CyclicDependencyChecker(bindings);
        cyclicDepedencyChecker.CheckType(type);

        var implementationTypes = bindings.GetImplementationTypes(type);
        return !implementationTypes.Any() ? null : Resolve<T>(implementationTypes);
    }

    private IEnumerable Resolve<T>(IEnumerable<Type> implementationTypes)
    {
        var instances = implementationTypes.Select(ResolveType<T>);
        return instances;
    }

    private object? ResolveType<T>(Type implementationType)
    {
        var constructor = GetConstructorWithMostArguments(implementationType);
        if (constructor?.GetParameters().Length == 0)
        {
            return CreateOrLookupInstanceInScope(
                bindings,
                globalSingletonScope,
                implementationType
            );
        }

        var args = new List<object>(constructor!.GetParameters().Length);
        foreach (var parameterInfo in constructor.GetParameters())
        {
            if (parameterInfo.IsOptional)
            {
                if (parameterInfo.DefaultValue is not null)
                {
                    args.Add(parameterInfo.DefaultValue);
                }

                continue;
            }

            var parameterType = parameterInfo.ParameterType;
            var genericArgumentType = GetFirstGenericArgumentType(parameterType);

            var parameterInstances = CreateParameterInstances<T>(
                genericArgumentType,
                parameterType
            );

            ThrowErrorWhenParameterInstancesAreNull(parameterInstances, parameterType);
            var castInstances = parameterInstances!.Cast<object>();

            if (parameterType.IsGenericType)
            {
                var list = CreateGenericList(genericArgumentType!);
                AddItemsToList(castInstances, list);
                args.Add(list);
                continue;
            }

            args.Add(castInstances.Count() == 1 ? castInstances.First() : castInstances);
        }

        return CreateOrLookupInstanceInScope(
            bindings,
            globalSingletonScope,
            implementationType,
            args.ToArray()
        );
    }

    private static ConstructorInfo? GetConstructorWithMostArguments(Type implementationType)
    {
        return implementationType
            .GetConstructors()
            .OrderBy(c => c.GetParameters().Length)
            .LastOrDefault();
    }

    private static object? CreateOrLookupInstanceInScope(
        Bindings bindings,
        Scope scope,
        Type type,
        params object[] args
    )
    {
        var binding = bindings.GetBindingForImplementationType(type);
        if (scope.GetHashCode() == binding.ScopeHashCode)
        {
            var instance = Scope.Find(scope, type);
            if (instance != null)
            {
                return instance;
            }

            instance = CreateInstance(type, args);
            if (instance is not null)
            {
                scope.Add(type, instance);
            }
            return instance;
        }

        return CreateInstance(type, args);
    }

    private static object? CreateInstance(Type type, params object[] args)
    {
        return Activator.CreateInstance(type, args);
    }

    private static Type? GetFirstGenericArgumentType(Type parameterType)
    {
        Type? genericArgumentType = null;
        if (parameterType.IsGenericType)
        {
            // TODO should probably constrain to IEnumerable<T>
            var genericArguments = parameterType.GetGenericArguments();
            genericArgumentType = genericArguments.First();
        }

        return genericArgumentType;
    }

    private IEnumerable? CreateParameterInstances<T>(Type? genericArgumentType, Type parameterType)
    {
        var parameterInstances =
            genericArgumentType != null
                ? GetInstances<T>(genericArgumentType)
                : GetInstances<T>(parameterType);

        return parameterInstances;
    }

    private static void ThrowErrorWhenParameterInstancesAreNull(
        IEnumerable? parameterInstances,
        Type parameterType
    )
    {
        if (parameterInstances == null)
        {
            throw new InvalidOperationException(
                string.Format(
                    "Not able to resolve implementation for type '{0}'",
                    parameterType.FullName
                )
            );
        }
    }

    private static IList CreateGenericList(Type type)
    {
        var listType = typeof(List<>).MakeGenericType(type);
        return (IList)Activator.CreateInstance(listType)!;
    }

    private static void AddItemsToList(IEnumerable<object> items, IList list)
    {
        foreach (var item in items)
        {
            list.Add(item);
        }
    }
}
