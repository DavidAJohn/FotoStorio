using System.Collections.Generic;
using FotoStorio.Shared.Entities;

namespace FotoStorio.Shared.DTOs
{
    public class PaymentIntentCreateDTO
    {
        public List<BasketItem> Items { get; set; }
        public string PaymentIntentId { get; set; }
    }
}