using System.IO;
using System.Threading.Tasks;
using FotoStorio.Server.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = FotoStorio.Shared.Models.Orders.Order;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _config;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
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

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            string WhSecret = _config["Stripe:WhSecret"];

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);

            PaymentIntent intent; // Stripe class
            Order order; // application class from FotoStorio.Shared.Models.Orders.Order

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded: {Id}", intent.Id);

                    order = await _paymentService.UpdateOrderPaymentStatus(intent.Id, OrderStatus.PaymentReceived);
                    _logger.LogInformation("Order updated to payment succeeded: {Id}", order.Id);
                    break;

                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: {Id}", intent.Id);

                    order = await _paymentService.UpdateOrderPaymentStatus(intent.Id, OrderStatus.PaymentFailed);
                    _logger.LogInformation("Order updated to payment failed: {Id}", order.Id);
                    break;
            }

            return new EmptyResult(); // confirms to Stripe that response has been received
        }
    }
}