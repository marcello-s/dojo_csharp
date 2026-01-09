#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataCommandDispatcher;

public interface IDependencyResolver
{
    IEnumerable<object>? GetAll<T>();
}
