#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModelLib;

namespace WpfApp.Views;

/// <summary>
/// Interaction logic for TareProcessingDetailsView.xaml
/// </summary>
public partial class TareProcessingDetailsView : UserControl
{
    public TareProcessingDetailsView()
    {
        InitializeComponent();
    }

    private void TareDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TareDetails.SelectedItem == null)
        {
            IdColumn.IsReadOnly = true;
        }
        else
        {
            IdColumn.IsReadOnly = !TareDetails.SelectedItem.Equals(
                CollectionView.NewItemPlaceholder
            );
        }
    }

    private void TareDetails_PreparingCellForEdit(
        object sender,
        DataGridPreparingCellForEditEventArgs e
    )
    {
        // hook up virtual keyboard/numpad
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        var conductor = IoC.GetInstance<IShell>() as ScreenConductor;
        var vm = conductor?.ActiveScreen;
        if (vm != null)
        {
            var binding = new Binding("GetTareDetails");
            binding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(TareDetails, DataGrid.ItemsSourceProperty, binding);
        }
    }
}
