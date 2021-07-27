using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Shared.DTOs
{
    public class OrderDTO
    {
        public string BasketId { get; set; }
        public Address SendToAddress { get; set; }
    }
}