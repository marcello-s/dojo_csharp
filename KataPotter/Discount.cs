#region license and copyright
/*
 * The MIT License, Copyright (c) 2011-2025 Marcel Schneider
 * for details see License.txt
 */
#endregion

namespace KataPotter
{
    public class Discount(string name)
    {
        public ShoppingBasket Basket { get; set; } = new ShoppingBasket();
        public string Name { get; set; } = name;
        public decimal Percentage { get; set; }

        private static decimal GetPercentageForDistinctItems(int numberOfItems, decimal percentage)
        {
            return numberOfItems switch
            {
                2 => percentage,
                3 => 2 * percentage,
                4 => 4 * percentage,
                5 => 5 * percentage,
                _ => 0,
            };
        }

        public decimal Calculate()
        {
            var discounts = RunCandidates();
            return discounts != null && discounts.Any() ? discounts.Max() : 0;
        }

        private IEnumerable<decimal> RunCandidates()
        {
            var basket = Basket.Clone() as ShoppingBasket;
            var distinctCount = basket?.Items.Distinct().Count();
            var items = basket?.Items.OrderBy(item => item.Name).ToList();
            var discounts = new List<decimal>();

            if (distinctCount == null || items == null)
            {
                return discounts;
            }

            // run all distinct count permutations
            for (int i = 2; i < distinctCount + 1; i++)
            {
                for (int j = 0; j < items?.Count / i; j++)
                {
                    // create sets of distinct count
                    var setList = CreateSets(items!, i, j);

                    // calculate discount total for sets
                    var discountTotal = (
                        from set in setList
                        let percentage = GetPercentageForDistinctItems(set.Count(), Percentage)
                        select set.Sum(item => item.Price * percentage)
                    ).Sum();
                    discounts.Add(discountTotal);
                }
            }

            return discounts;
        }

        /*
         * input: 1,1,2,2,3,3,4,5
         * output:
         * 1,2/1,2/3,4/3,5
         * 1,2,3/1,2,3/4,5
         * 1,2,3,4/1,2,3,5
         * 1,2,3,4,5/1,2,3
         *
         * input: 1,1,1,1,1,2,2,2,2,2,3,3,3,3,4,4,4,4,4,5,5,5,5
         * output:
         * 1,2,3,4,5/1,2,3,4,5/1,2,3,4,5/1,2,3,4,5/1,2,4
         * 1,2,3,4,5/1,2,3,4,5/1,2,3,4,5/1,2,3,4/1,2,4,5
         *
         * a,b,c
         * a,c,b
         * b,a,c
         * b,c,a
         * c,a,b
         * c,b,a
         *
         */

        private static IEnumerable<IEnumerable<Item>> CreateSets(
            IEnumerable<Item> input,
            int numberOfItems,
            int setCount
        )
        {
            var sets = new List<IEnumerable<Item>>();
            var items = input.ToList();
            while (items.Count > 0)
            {
                var takes = new List<Item>();
                for (int i = 0; i < numberOfItems; i++)
                {
                    var take = items.FirstOrDefault(item => !takes.Contains(item));
                    if (take != null)
                    {
                        takes.Add(take);
                        items.Remove(take);
                    }
                }

                sets.Add(takes);
                //foreach (var take in takes)
                //    Console.Write(take.Name + ",");
                //Console.WriteLine();

                // recurse 1 time to build sets with a lower set count
                if (sets.Count() == setCount && setCount != int.MaxValue)
                {
                    sets.AddRange(CreateSets(items, numberOfItems - 1, int.MaxValue));
                    break;
                }
            }

            return sets;
        }
    }
}
