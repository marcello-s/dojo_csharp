#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventSync;

interface IHandle { }

interface IHandle<T> : IHandle
{
    void Handle(T message);
}
