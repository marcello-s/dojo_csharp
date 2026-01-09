#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataMicrokernel;

public class Bindings
{
    private readonly HashSet<Binding> bindingsSet;

    public Bindings()
    {
        bindingsSet = new HashSet<Binding>();
    }

    public Binding Add(Type? abstractType, Type? implementationType)
    {
        if (abstractType == null)
        {
            throw new ArgumentNullException("abstractType");
        }

        if (implementationType == null)
        {
            throw new ArgumentNullException("implementationType");
        }

        if (abstractType == implementationType)
        {
            throw new ArgumentException("abstractType cannot be same as implementationType");
        }

        if (implementationType.IsInterface)
        {
            throw new ArgumentException("implementationType cannot be an interface type");
        }

        if (implementationType.IsAbstract)
        {
            throw new ArgumentException("implementationType cannot be an abstract type");
        }

        if (!abstractType.IsAssignableFrom(implementationType))
        {
            throw new ArgumentException(
                string.Format(
                    "type '{0}' cannot be assigned to type '{1}' ",
                    implementationType.FullName,
                    abstractType.FullName
                )
            );
        }

        if (
            bindingsSet.Any(b =>
                b.AbstractType == abstractType && b.ImplementationType == implementationType
            )
        )
        {
            return bindingsSet.First(b =>
                b.AbstractType == abstractType && b.ImplementationType == implementationType
            );
        }

        var binding = new Binding(abstractType, implementationType);
        bindingsSet.Add(binding);
        return binding;
    }

    public IEnumerable<Type> GetImplementationTypes(Type abstractType)
    {
        return bindingsSet
            .Where(b => b.AbstractType == abstractType)
            .Select(b => b.ImplementationType)
            .ToList();
    }

    public Binding GetBindingForImplementationType(Type implementationType)
    {
        return bindingsSet.FirstOrDefault(b => b.ImplementationType == implementationType);
    }

    public void UpdateBinding(Binding oldBinding, Binding newBinding)
    {
        bindingsSet.RemoveWhere(b => b.Equals(oldBinding));
        bindingsSet.Add(newBinding);
    }
}
