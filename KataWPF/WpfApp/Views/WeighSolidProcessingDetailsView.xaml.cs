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
/// Interaction logic for WeighSolidProcessingDetailsView.xaml
/// </summary>
public partial class WeighSolidProcessingDetailsView : UserControl
{
    public WeighSolidProcessingDetailsView()
    {
        InitializeComponent();
    }

    private void WeighSolidDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (WeighSolidDetails.SelectedItem == null)
        {
            IdColumn.IsReadOnly = true;
        }
        else
        {
            IdColumn.IsReadOnly = !WeighSolidDetails.SelectedItem.Equals(
                CollectionView.NewItemPlaceholder
            );
        }
    }

    private void WeighSolidDetails_PreparingCellForEdit(
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
            var binding = new Binding("GetWeighSolidDetails");
            binding.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(WeighSolidDetails, DataGrid.ItemsSourceProperty, binding);
        }
    }
}
