#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

using System.ComponentModel.Composition;

namespace ViewModelLib.Navigation;

[Export(typeof(IScreenNavigator))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class ScreenNavigator : IScreenNavigator
{
    private INavigatable current = null!;
    private IList<INavigatable> navigatableItems = null!;

    public INavigatable? Back()
    {
        if (Current == null)
        {
            return Current;
        }

        INavigatable previousActiveItem = Current.Previous;
        while (previousActiveItem != null && !previousActiveItem.IsActive)
        {
            previousActiveItem = previousActiveItem.Previous;
        }

        if (previousActiveItem != null)
        {
            current = previousActiveItem;
        }

        return previousActiveItem;
    }

    public INavigatable? Forward()
    {
        if (Current == null)
        {
            return Current;
        }

        INavigatable nextActiveItem = Current.Next;
        while (nextActiveItem != null && !nextActiveItem.IsActive)
        {
            nextActiveItem = nextActiveItem.Next;
        }

        if (nextActiveItem != null)
        {
            current = nextActiveItem;
        }

        return nextActiveItem;
    }

    public void NavigateTo(object target)
    {
        if (navigatableItems == null)
        {
            return;
        }

        var item = navigatableItems.SingleOrDefault((x) => x.Target == target);
        if (item != null)
        {
            current = item;
        }
    }

    public INavigatable Current
    {
        get { return current; }
    }

    public void Add(INavigatable screen)
    {
        if (navigatableItems == null)
        {
            navigatableItems = new List<INavigatable>();
        }

        if (!navigatableItems.Contains<INavigatable>(screen))
        {
            navigatableItems.Add(screen);
            if (Current == null)
            {
                current = screen;
            }
        }
    }

    public void Remove(INavigatable screen)
    {
        if (navigatableItems == null)
        {
            return;
        }

        if (navigatableItems.Contains<INavigatable>(screen))
        {
            navigatableItems.Remove(screen);
            if (navigatableItems.Count == 0)
            {
                current = null!;
            }
        }
    }
}
