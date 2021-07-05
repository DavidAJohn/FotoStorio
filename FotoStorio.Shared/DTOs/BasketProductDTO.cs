namespace FotoStorio.Shared.DTOs
{
    public class BasketProductDTO
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public decimal SalePrice { get; set; }
    }
}