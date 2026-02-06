#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Navigation;

public class NavigationItem(object Item) : INavigatable
{
    private WeakReference itemReference = new WeakReference(Item);
    private INavigatable previous = null!;
    private INavigatable next = null!;
    private bool isActive = true;

    public INavigatable Previous
    {
        get { return previous; }
        set { previous = value; }
    }

    public INavigatable Next
    {
        get { return next; }
        set { next = value; }
    }

    public object? Target
    {
        get
        {
            if (itemReference == null)
            {
                return null;
            }

            return itemReference.Target;
        }
    }

    public bool IsActive
    {
        get
        {
            if (itemReference == null)
            {
                return false;
            }

            return isActive && itemReference.IsAlive;
        }
        set { isActive = value; }
    }
}
