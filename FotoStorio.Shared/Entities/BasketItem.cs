using FotoStorio.Shared.Models;

namespace FotoStorio.Shared.Entities
{
    public class BasketItem
    {
        public int Quantity { get; set; }
        public Product Product { get; set; }

        public decimal Total
        {
            get
            {
                var currentPrice = Product.SalePrice != 0 && Product.SalePrice < Product.Price ? Product.SalePrice : Product.Price;
                return currentPrice * Quantity;
            }
        }
    }
}