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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppShell.Controls;

/// <summary>
/// Interaction logic for BusyIndicatorControl.xaml
/// </summary>
public partial class BusyIndicatorControl : UserControl
{
    public BusyIndicatorControl()
    {
        InitializeComponent();
    }

    private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Storyboard storyBoard = (Storyboard)FindResource("RotateBusy");
        if (Visibility == System.Windows.Visibility.Visible)
        {
            storyBoard.Begin(this, true);
            ((Storyboard)FindResource("FadeInScreen")).Begin(this, true);
        }

        if (Visibility == Visibility.Hidden || Visibility == Visibility.Collapsed)
        {
            ((Storyboard)FindResource("FadeOutScreen")).Begin(this, true);
            storyBoard.Stop(this);
        }
    }
}
