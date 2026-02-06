#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppShell.Controls;

/// <summary>
/// Interaction logic for ToggleTouchButton.xaml
/// </summary>
public partial class ToggleTouchButton : UserControl
{
    public ToggleTouchButton()
    {
        InitializeComponent();
    }

    public static DependencyProperty ButtonTextProperty = DependencyProperty.Register(
        "ButtonText",
        typeof(string),
        typeof(ToggleTouchButton)
    );
    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    public static DependencyProperty IconControlProperty = DependencyProperty.Register(
        "IconControl",
        typeof(UserControl),
        typeof(ToggleTouchButton)
    );

    public UserControl IconControl
    {
        get { return (UserControl)GetValue(IconControlProperty); }
        set { SetValue(IconControlProperty, value); }
    }

    public static DependencyProperty IsCheckedProperty = DependencyProperty.Register(
        "IsChecked",
        typeof(bool),
        typeof(ToggleTouchButton)
    );

    public bool IsChecked
    {
        get { return (bool)GetValue(IsCheckedProperty); }
        set { SetValue(IsCheckedProperty, value); }
    }
}
