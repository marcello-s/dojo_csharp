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
/// Interaction logic for TouchButton.xaml
/// </summary>
public partial class TouchButton : UserControl
{
    public TouchButton()
    {
        InitializeComponent();
    }

    public static DependencyProperty ButtonTextProperty = DependencyProperty.Register(
        "ButtonText",
        typeof(string),
        typeof(TouchButton)
    );
    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    public static DependencyProperty IconControlProperty = DependencyProperty.Register(
        "IconControl",
        typeof(UserControl),
        typeof(TouchButton)
    );
    public UserControl IconControl
    {
        get { return (UserControl)GetValue(IconControlProperty); }
        set { SetValue(IconControlProperty, value); }
    }
}
