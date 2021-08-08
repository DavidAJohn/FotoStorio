using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Shared.DTOs
{
    public class OrderCreateDTO
    {
        [Required]
        public List<BasketItem> Items { get; set; }
        
        [Required]
        public Address SendToAddress { get; set; }
        
        [Required]
        public string PaymentIntentId { get; set; }
    }
}