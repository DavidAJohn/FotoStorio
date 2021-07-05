using System;
using System.Collections.Generic;

namespace FotoStorio.Shared.Entities
{
    public class Basket
    {
        public List<BasketItem> BasketItems { get; set; } = new();

        public Decimal BasketTotal
        {
            get
            {
                decimal total = (decimal)0.0;

                foreach (var item in BasketItems)
                {
                    total += item.Total;
                }

                return total;
            }
        }
        public DateTime LastAccessed { get; set; }
        public int TimeToLiveInSeconds { get; set; } = 300; // 5 mins
    }
}