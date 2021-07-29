using System;
using System.Collections.Generic;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Shared.DTOs
{
    public class OrderDetailsDTO
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public Address SendToAddress { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
}