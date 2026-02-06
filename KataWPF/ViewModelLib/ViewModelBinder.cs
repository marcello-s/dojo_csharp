#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;

namespace ViewModelLib;

public static class ViewModelBinder
{
    private static readonly BooleanToVisibilityConverter _booleanToVisibilityConverter =
        new BooleanToVisibilityConverter();

    private static Dictionary<Type, HashSet<DependencyProperty>> propertyRegistry = new Dictionary<
        Type,
        HashSet<DependencyProperty>
    >
    {
        {
            typeof(FrameworkElement),
            new HashSet<DependencyProperty> { FrameworkElement.IsEnabledProperty }
        },
        {
            typeof(TextBox),
            new HashSet<DependencyProperty> { TextBox.TextProperty }
        },
        {
            typeof(ContentControl),
            new HashSet<DependencyProperty> { View.ModelProperty }
        },
        {
            typeof(ItemsControl),
            new HashSet<DependencyProperty> { ItemsControl.ItemsSourceProperty }
        },
        {
            typeof(TextBlock),
            new HashSet<DependencyProperty> { TextBlock.TextProperty }
        },
        {
            typeof(Border),
            new HashSet<DependencyProperty> { Border.VisibilityProperty }
        },
        {
            typeof(UserControl),
            new HashSet<DependencyProperty> { UserControl.VisibilityProperty }
        },
        {
            typeof(ButtonBase),
            new HashSet<DependencyProperty> { ButtonBase.VisibilityProperty }
        },
        {
            typeof(ToggleButton),
            new HashSet<DependencyProperty> { ToggleButton.IsCheckedProperty }
        },
        {
            typeof(RangeBase),
            new HashSet<DependencyProperty> { RangeBase.ValueProperty }
        },
        {
            typeof(DataGrid),
            new HashSet<DependencyProperty> { DataGrid.ItemsSourceProperty }
        },
    };

    public static Dictionary<Type, HashSet<DependencyProperty>> PropertyRegistry
    {
        get { return propertyRegistry; }
    }

    private static DataTemplate GetDefaultTemplate()
    {
        string template =
            "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' "
            + "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' "
            + "xmlns:vm='clr-namespace:Tecan.ViewModelLib;assembly=Tecan.ViewModel'> "
            + "<ContentControl vm:View.Model=\"{Binding}\" />"
            + "</DataTemplate>";
        XmlReader xmlReader = XmlReader.Create(template);

        return (DataTemplate)XamlReader.Load(xmlReader);
    }

    public static void Bind(object viewModel, DependencyObject view)
    {
        var element = view as FrameworkElement;
        if (element == null)
        {
            return;
        }

        var viewType = viewModel.GetType();
        var properties = viewType.GetProperties();
        var methods = viewType.GetMethods();

        BindProperties(element, properties);
        BindCommands(viewModel, element, methods, properties);

        element.DataContext = viewModel;
    }

    public delegate DependencyObject FindControlDelegate(
        FrameworkElement view,
        PropertyInfo property
    );

    private static FindControlDelegate customFindControl = null!;

    public static FindControlDelegate CustomFindControl
    {
        get { return customFindControl; }
        set { customFindControl = value; }
    }

    private static void BindProperties(FrameworkElement view, IEnumerable<PropertyInfo> properties)
    {
        foreach (var property in properties)
        {
            var foundControl = view.FindName(property.Name) as DependencyObject;
            if (foundControl == null)
            {
                if (customFindControl != null)
                {
                    foundControl = customFindControl.Invoke(view, property);
                    if (foundControl == null)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            DependencyProperty boundProperty = null!;
            var bindingType = foundControl.GetType();

            if (foundControl is ButtonBase)
                bindingType = typeof(ButtonBase);

            if (foundControl is ToggleButton)
                bindingType = typeof(ToggleButton);

            //System.Diagnostics.Trace.WriteLine("binding type: " + bindingType.Name + " for: " + property.Name);

            if (
                !propertyRegistry.TryGetValue(
                    bindingType,
                    out HashSet<DependencyProperty>? propertySet
                )
            )
            {
                System.Diagnostics.Trace.WriteLine("continue property binding");
                continue;
            }
            else
            {
                var propertyName = property.Name.Replace(
                    ((FrameworkElement)foundControl).Name,
                    string.Empty
                );

                if (!string.IsNullOrEmpty(propertyName))
                {
                    if (propertySet.Any((p) => p.Name.Equals(propertyName)))
                    {
                        //System.Diagnostics.Trace.WriteLine(propertyName + " registered");
                        boundProperty = propertySet.First((p) => p.Name.Equals(propertyName));
                    }
                }
                else
                {
                    boundProperty = propertySet.First();
                }
            }

            if (((FrameworkElement)foundControl).GetBindingExpression(boundProperty) != null)
            {
                continue;
            }

            var binding = new Binding(property.Name)
            {
                Mode = property.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                ValidatesOnDataErrors = Attribute
                    .GetCustomAttributes(property, typeof(ValidationAttribute), true)
                    .Any(),
            };

            System.Diagnostics.Trace.WriteLine(
                "boundProperty: " + boundProperty.Name + " for: " + property.Name
            );

            if (
                boundProperty == UIElement.VisibilityProperty
                && typeof(bool).IsAssignableFrom(property.PropertyType)
            )
            {
                binding.Converter = _booleanToVisibilityConverter;
            }
            else if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
            {
                binding.StringFormat = "{0:yyyy-MM-dd}";
            }

            BindingOperations.SetBinding(foundControl, boundProperty, binding);

            if (foundControl is TextBox textBox && boundProperty == TextBox.TextProperty)
            {
                textBox.TextChanged += delegate
                {
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                };

                continue;
            }

            if (
                foundControl is ItemsControl itemsControl
                && string.IsNullOrEmpty(itemsControl.DisplayMemberPath)
                && itemsControl.ItemTemplate == null
            )
            {
                itemsControl.ItemTemplate = GetDefaultTemplate();
                continue;
            }
        }
    }

    private static void BindCommands(
        object viewModel,
        FrameworkElement view,
        IEnumerable<MethodInfo> methods,
        IEnumerable<PropertyInfo> properties
    )
    {
        foreach (var method in methods)
        {
            var foundControl = view.FindName(method.Name);
            if (foundControl == null)
            {
                continue;
            }

            var bindingControl = foundControl;
            var element = foundControl as UserControl;
            if (element != null)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(element))
                {
                    if (child is ButtonBase)
                    {
                        bindingControl = child;
                        break;
                    }
                }
            }

            var foundProperty = properties.FirstOrDefault(x => x.Name == "Can" + method.Name);
            var command = new ReflectiveCommand(viewModel, method, foundProperty);
            TrySetCommand(bindingControl, command);
        }
    }

    private static void TrySetCommand(object control, ICommand command)
    {
        var succeedIn = TrySetCommandBinding<ButtonBase>(
            control,
            ButtonBase.CommandProperty,
            command
        );
    }

    private static bool TrySetCommandBinding<T>(
        object control,
        DependencyProperty property,
        ICommand command
    )
        where T : DependencyObject
    {
        if (control is not T commandSource)
        {
            return false;
        }

        BindingOperations.SetBinding(commandSource, property, new Binding { Source = command });

        return true;
    }
}
