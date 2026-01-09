#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataMicrokernel;

public class Scope
{
    private readonly Scope? parentScope;
    private readonly IDictionary<Type, object> instanceCache;

    public Scope(Scope? parentScope = null)
    {
        this.parentScope = parentScope;
        instanceCache = new Dictionary<Type, object>();
    }

    public void Add(Type? type, object? instance)
    {
        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        if (instance == null)
        {
            throw new ArgumentNullException("instance");
        }

        if (!instanceCache.ContainsKey(type))
        {
            instanceCache.Add(type, instance);
        }
    }

    public static object? Find(Scope? scope, Type type)
    {
        if (scope == null)
        {
            return null;
        }

        if (type == null)
        {
            throw new ArgumentNullException("type");
        }

        return scope.instanceCache.ContainsKey(type)
            ? scope.instanceCache[type]
            : Find(scope.parentScope, type);
    }
}
