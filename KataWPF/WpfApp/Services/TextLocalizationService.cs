#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Linq.Expressions;
using ViewModelLib.Services;

namespace WpfApp.Services;

[Export(typeof(ITextLocalizationService))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class TextLocalizationService : LocalizationService, ITextLocalizationService
{
    public string TranslateString(string key)
    {
        return MapTitleForMode(key);
    }

    public override string TranslateString<T>(Expression<Func<T>> property, string key)
    {
        return TranslateString(base.TranslateString(property, key));
    }

    private static string MapTitleForMode(string titleKey)
    {
        return titleKey switch
        {
            "Title_Tare" => "TARE MODE",
            "Title_WeighSolid" => "WEIGH SOLID MODE",
            "Title_Dilute" => "DILUTE MODE",
            _ => string.Empty,
        };
    }
}
