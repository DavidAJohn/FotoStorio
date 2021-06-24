using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FotoStorio.Shared.Models
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
        
        [Required]
        public bool IsAvailable { get; set; } = true;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        public string GetFormattedPrice()
        {
            return Price.ToString("0.00");
        }
    }
}
