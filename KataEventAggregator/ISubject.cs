#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataEventAggregator;

/// <summary>
/// Internal ISubject definition to combine with IObserver
/// </summary>
/// <typeparam name="T">The data type</typeparam>
internal interface ISubject<in T> : IObserver<T> { }
