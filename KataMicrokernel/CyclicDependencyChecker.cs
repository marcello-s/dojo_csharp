#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Reflection;

namespace KataMicrokernel;

public class CyclicDependencyChecker
{
    private readonly Bindings bindings;
    private readonly Stack<Type> resolvedTypes;

    public CyclicDependencyChecker(Bindings bindings)
    {
        if (bindings == null)
        {
            throw new ArgumentNullException("bindings");
        }

        this.bindings = bindings;
        resolvedTypes = new Stack<Type>();
    }

    public void CheckType(Type type)
    {
        var implementationTypes = bindings.GetImplementationTypes(type);
        if (!implementationTypes.Any())
        {
            return;
        }

        Resolve(implementationTypes);
    }

    private void Resolve(IEnumerable<Type> implementationTypes)
    {
        foreach (var implementationType in implementationTypes)
        {
            if (resolvedTypes.Contains(implementationType))
            {
                throw new ArgumentException(
                    string.Format(
                        "The type '{0}' is already contained in the dependency tree",
                        implementationType.FullName
                    )
                );
            }

            resolvedTypes.Push(implementationType);
            ResolveType(implementationType);
            resolvedTypes.Pop();
        }
    }

    private void ResolveType(Type implementationType)
    {
        var constructor = GetConstructorWithMostArguments(implementationType);
        if (constructor?.GetParameters().Length == 0)
        {
            return;
        }

        foreach (var parameterInfo in constructor!.GetParameters())
        {
            if (parameterInfo.IsOptional)
            {
                continue;
            }

            var parameterType = parameterInfo.ParameterType;
            var genericArgumentType = GetFirstGenericArgumentType(parameterType);

            CheckType(genericArgumentType ?? parameterType);
        }
    }

    private static ConstructorInfo? GetConstructorWithMostArguments(Type implementationType)
    {
        return implementationType
            .GetConstructors()
            .OrderBy(c => c.GetParameters().Length)
            .LastOrDefault();
    }

    private static Type? GetFirstGenericArgumentType(Type parameterType)
    {
        Type? genericArgumentType = null;
        if (parameterType.IsGenericType)
        {
            var genericArguments = parameterType.GetGenericArguments();
            genericArgumentType = genericArguments.First();
        }

        return genericArgumentType;
    }
}
