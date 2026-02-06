#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

namespace ViewModelLib;

public class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
{
    public ViewModelBase()
    {
        FindControl = CustomFindControl!;
    }

    public event PropertyChangedEventHandler? PropertyChanged = delegate { };

    public virtual string Error
    {
        get
        {
            var context = new ValidationContext(this, null, null);
            var results = new List<ValidationResult>();
            return !Validator.TryValidateObject(this, context, results)
                ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                : string.Empty;
        }
    }

    public virtual string this[string columnName]
    {
        get
        {
            var context = new ValidationContext(this, null, null) { MemberName = columnName };

            var results = new List<ValidationResult>();
            var value = GetType().GetProperty(columnName)?.GetValue(this, null);
            return !Validator.TryValidateProperty(value, context, results)
                ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                : string.Empty;
        }
    }

    public void NotifyOfPropertyChange(string propertyName)
    {
        if (PropertyChanged != null)
        {
            Execute.OnUIThread(() =>
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName))
            );
        }
    }

    public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
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

        NotifyOfPropertyChange(memberExpression.Member.Name);
    }

    public ViewModelBinder.FindControlDelegate FindControl = null!;

    public virtual DependencyObject? CustomFindControl(FrameworkElement view, PropertyInfo property)
    {
        return null;
    }
}
