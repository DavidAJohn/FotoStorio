using System.Threading.Tasks;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;

namespace FotoStorio.Server.Contracts
{
    public interface IPaymentService
    {
        Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO);
    }
}