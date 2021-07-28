using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Contracts
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, OrderCreateDTO order, Address sendToAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
    }
}