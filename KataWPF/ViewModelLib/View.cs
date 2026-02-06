#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Windows;
using System.Windows.Controls;

namespace ViewModelLib;

public static class View
{
    public static DependencyProperty ModelProperty = DependencyProperty.RegisterAttached(
        "Model",
        typeof(object),
        typeof(View),
        new PropertyMetadata(ModelChanged)
    );

    public static void SetModel(DependencyObject d, object value)
    {
        d.SetValue(ModelProperty, value);
    }

    public static object GetModel(DependencyObject d)
    {
        return d.GetValue(ModelProperty);
    }

    public static void ModelChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null || e.NewValue == e.OldValue)
        {
            return;
        }

        var view = ViewLocator.Locate(e.NewValue);
        if (e.NewValue is ViewModelBase viewModel)
        {
            ViewModelBinder.CustomFindControl = viewModel.FindControl;
            ViewModelBinder.Bind(e.NewValue, view);
            ((ContentControl)sender).Content = view;
        }
    }
}
