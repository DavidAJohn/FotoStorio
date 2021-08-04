using System.Threading.Tasks;
using FotoStorio.Server.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FotoStorio.Server.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PaymentIntentResult>> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO)
        {
            var result = await _paymentService.CreateOrUpdatePaymentIntent(paymentIntentCreateDTO);

            if (result == null) return BadRequest();

            return Ok(result);
        }
    }
}