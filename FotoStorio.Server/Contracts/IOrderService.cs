using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Contracts
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, Basket basket, Address sendToAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
    }
}