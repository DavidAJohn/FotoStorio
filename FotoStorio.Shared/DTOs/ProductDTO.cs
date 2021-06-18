namespace FotoStorio.Shared.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int BrandId { get; set; }

        public string Brand { get; set; }

        public int CategoryId { get; set; }

        public string Category { get; set; }
        
        public bool IsAvailable { get; set; }
    }
}