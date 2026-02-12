#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;
using System.Linq.Expressions;

namespace ViewModelLib.Services;

[Export(typeof(ILocalizationService))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class LocalizationService : ILocalizationService
{
    public virtual string TranslateString<T>(Expression<Func<T>> property, string key)
    {
        var lambda = (LambdaExpression)property;

        MemberExpression memberExpression;
        if (lambda.Body is UnaryExpression)
        {
            var unaryExpression = (UnaryExpression)lambda.Body;
            memberExpression = (MemberExpression)unaryExpression.Operand;
        }
        else
        {
            memberExpression = (MemberExpression)lambda.Body;
        }

        return memberExpression.Member.Name + "_" + key;
    }
}
