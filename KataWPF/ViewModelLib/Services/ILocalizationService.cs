#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Linq.Expressions;

namespace ViewModelLib.Services;

public interface ILocalizationService
{
    string TranslateString(string key);
    string TranslateString<T>(Expression<Func<T>> property, string key);
}
