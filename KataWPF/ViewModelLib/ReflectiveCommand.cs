#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;

namespace ViewModelLib;

public class ReflectiveCommand : ICommand
{
    private readonly PropertyInfo? canExecute;
    private readonly MethodInfo execute;
    private readonly object model;

    public ReflectiveCommand(object model, MethodInfo execute, PropertyInfo? canExecute)
    {
        this.model = model;
        this.execute = execute;
        this.canExecute = canExecute;

        var notifier = this.model as INotifyPropertyChanged;
        if (notifier != null && this.canExecute != null)
        {
            notifier.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == this.canExecute.Name)
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            };
        }
    }

    public bool CanExecute(object? parameter)
    {
        if (canExecute != null)
        {
            var value = canExecute.GetValue(model, null);
            if (value is bool boolValue)
            {
                return boolValue;
            }

            return true;
        }

        return true;
    }

    public event EventHandler? CanExecuteChanged = delegate { };

    /*
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
     */

    public void Execute(object? parameter)
    {
        var returnValue = execute.Invoke(model, null);
        if (returnValue != null)
        {
            HandleReturnValue(returnValue);
        }
    }

    private static void HandleReturnValue(object returnValue)
    {
        if (returnValue is IResult)
        {
            returnValue = new[] { returnValue as IResult };
        }

        if (returnValue is IEnumerable<IResult>)
        {
            if (returnValue is IEnumerable<IResult> value)
            {
                new ResultEnumerator(value).Enumerate();
            }
        }
    }
}
