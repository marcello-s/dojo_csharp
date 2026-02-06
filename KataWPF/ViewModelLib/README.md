# ViewModelLib

A library to glue views and viewmodels by naming convention.

type-ViewModel
type-View

Views and ViewModels will be matched by their namespace type name.

In the your UI WPF app organize assets as following:

/ViewModels
    NavigationViewModel.cs
    ShellViewModel.cs    
/Views
    NavigationView.xaml
    NavigationView.xaml.cs
    ShellView.xaml
    ShellView.xaml.cs

