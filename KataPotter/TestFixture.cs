#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

using NUnit.Framework;

namespace KataPotter
{
    [TestFixture]
    public class TestFixture
    {
        private ShoppingBasket basket = null!;
        private IList<Discount> discounts = null!;
        private GrandTotal grandTotal = null!;

        [SetUp]
        public void SetUp()
        {
            basket = new ShoppingBasket();
            discounts = new List<Discount>();
            grandTotal = new GrandTotal(basket, discounts);
        }

        [Test]
        public void TestItemCompare()
        {
            var item1 = new Item("book1", 8);
            var item2 = new Item("book1", 8);
            Assert.That(item1.Equals(item2), Is.True);
        }

        [Test]
        public void TestItemTotal()
        {
            basket.AddItem(new Item("book1", 8 ));
            basket.AddItem(new Item("book2", 8 ));
            grandTotal.Calculate();
            Assert.That(16, Is.EqualTo(grandTotal.ItemTotal));
        }

        [Test]
        public void TestNoDiscount2Items()
        {
            basket.AddItem(new Item("book1", 8 ));
            basket.AddItem(new Item("book1",8 ));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 2, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(8 * 2, Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void TestNoDiscount3Items()
        {
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 3, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(8 * 3, Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void TestDiscount2Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 2, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(8 * 2 * 0.95M, Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void TestDiscount3Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book3", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 3, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(8 * 3 * 0.9M, Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void TestDiscount4Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book4", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 4, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(8 * 4 * 0.8M, Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void TestDiscount5Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book4", 8));
            basket.AddItem(new Item("book5", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 5, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(8 * 5 * 0.75M, Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void Test2Discounts3Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 3, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(8 + (8 * 2 * 0.95M), Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void Test2Discounts4Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 4, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(2 * (8 * 2 * 0.95M), Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void Test2Discounts6Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book4", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 6, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That((8 * 4 * 0.8M) + (8 * 2 * 0.95M), Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void Test1Discounts6Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book4", 8));
            basket.AddItem(new Item("book5", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 6, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(8 + (8 * 5 * 0.75M), Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void TestEdge2Discounts8Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book4", 8));
            basket.AddItem(new Item("book5", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 8, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(2 * (8 * 4 * 0.8M), Is.EqualTo(grandTotal.Total));
        }

        [Test]
        public void TestEdge5Discounts23Items()
        {
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book1", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book2", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book3", 8));
            basket.AddItem(new Item("book4", 8));
            basket.AddItem(new Item("book4", 8));
            basket.AddItem(new Item("book4", 8));
            basket.AddItem(new Item("book4", 8));
            basket.AddItem(new Item("book4", 8));
            basket.AddItem(new Item("book5", 8));
            basket.AddItem(new Item("book5", 8));
            basket.AddItem(new Item("book5", 8));
            basket.AddItem(new Item("book5", 8));
            var discount = new Discount("5% per unique item");
            discount.Percentage = 0.05M;
            discount.Basket = basket;
            discounts.Add(discount);
            grandTotal.Calculate();
            Assert.That(8 * 23, Is.EqualTo(grandTotal.ItemTotal));
            Assert.That(3 * (8 * 5 * 0.75M) + 2 * (8 * 4 * 0.8M), Is.EqualTo(grandTotal.Total));
        }
    }
}
