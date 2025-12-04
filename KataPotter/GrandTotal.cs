#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataPotter;

public class GrandTotal(ShoppingBasket basket, IEnumerable<Discount> discounts)
{
    public decimal ItemTotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal Total { get; set; }

    public void Calculate()
    {
        CalculateItemTotal();
        CalculateDiscountTotal();
        Total = ItemTotal - DiscountTotal;
    }

    private void CalculateItemTotal()
    {
        ItemTotal = basket.Items.Sum(item => item.Price);
    }

    private void CalculateDiscountTotal()
    {
        foreach (var discount in discounts)
        {
            if (basket.Clone() is not ShoppingBasket basketClone)
            {
                continue;
            }

            discount.Basket = basketClone;
            DiscountTotal += discount.Calculate();
        }
    }
}
