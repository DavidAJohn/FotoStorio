using System.ComponentModel.DataAnnotations;

namespace FotoStorio.Shared.DTOs
{
    public class ProductUpdateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public bool IsAvailable { get; set; }
    }
}