#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using ViewModelLib.Services;

namespace WpfApp.Services;

public interface ITextLocalizationService : ILocalizationService
{
    public string TranslateString(string key);
}
