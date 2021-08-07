using System.Collections.Generic;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Shared.DTOs
{
    public class OrderCreateDTO
    {
        public List<BasketItem> Items { get; set; }
        public Address SendToAddress { get; set; }
        public string PaymentIntentId { get; set; }
    }
}