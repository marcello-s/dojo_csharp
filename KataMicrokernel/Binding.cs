#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataMicrokernel;

public struct Binding
{
    public Type AbstractType { get; private set; }
    public Type ImplementationType { get; private set; }
    public int ScopeHashCode { get; set; }

    internal Binding(Type abstractType, Type implementationType)
        : this()
    {
        AbstractType = abstractType;
        ImplementationType = implementationType;
    }

    internal Binding(Binding binding)
        : this()
    {
        AbstractType = binding.AbstractType;
        ImplementationType = binding.ImplementationType;
        ScopeHashCode = binding.ScopeHashCode;
    }
}
