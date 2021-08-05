using FotoStorio.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FotoStorio.Client.Contracts
{
    public interface IPaymentService
    {
        Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(Basket basket);
    }
}
