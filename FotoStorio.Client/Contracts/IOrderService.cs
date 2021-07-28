using System.Threading.Tasks;
using FotoStorio.Shared.DTOs;

namespace FotoStorio.Client.Contracts
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrderAsync(OrderCreateDTO order);
    }
}