#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace ViewModelLib.Navigation;

public interface IScreenNavigator
{
    INavigatable? Back();
    INavigatable? Forward();
    void NavigateTo(object Target);
    INavigatable Current { get; }
    void Add(INavigatable screen);
    void Remove(INavigatable screen);
}
