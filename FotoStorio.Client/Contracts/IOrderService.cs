using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Shared.DTOs;

namespace FotoStorio.Client.Contracts
{
    public interface IOrderService
    {
        Task<OrderDetailsDTO> CreateOrderAsync(OrderCreateDTO order);
        Task<List<OrderDetailsDTO>> GetOrdersForUserAsync();
        Task<OrderDetailsDTO> GetOrderByIdAsync(int orderId);
    }
}