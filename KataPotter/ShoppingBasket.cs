#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2026 Marcel Schneider
 * for details see License.txt
 */
#endregion
namespace KataPotter;

public class ShoppingBasket : ICloneable
{
    private readonly IList<Item> items = new List<Item>();
    public IEnumerable<Item> Items => items;

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    public object Clone()
    {
        var clone = new ShoppingBasket();
        foreach (var item in Items)
        {
            clone.AddItem(new Item(item.Name, item.Price));
        }

        return clone;
    }
}
