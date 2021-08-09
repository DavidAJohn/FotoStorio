using System.Threading.Tasks;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Contracts
{
    public interface IPaymentService
    {
        Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO);
        Task<Order> UpdateOrderPaymentStatus(string paymentIntentId, OrderStatus status);
    }
}