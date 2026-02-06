#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.Windows;

namespace ViewModelLib;

public static class ViewLocator
{
    public static UIElement Locate(object viewModel)
    {
        var viewTypeName = viewModel
            .GetType()
            ?.AssemblyQualifiedName?.Replace("Model", string.Empty);
        if (viewTypeName == null)
        {
            throw new InvalidOperationException("Cannot locate view for null view model");
        }

        var viewType = Type.GetType(viewTypeName);

        if (viewType == null)
        {
            // go find the view
            throw new InvalidOperationException(
                $"Cannot locate view for view model type {viewModel.GetType().FullName}"
            );
        }

        return (UIElement)Activator.CreateInstance(viewType)!;
    }
}
