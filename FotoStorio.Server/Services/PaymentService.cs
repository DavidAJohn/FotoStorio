using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Server.Contracts;
using FotoStorio.Server.Specifications;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models.Orders;
using Microsoft.Extensions.Configuration;
using Stripe;
using Order = FotoStorio.Shared.Models.Orders.Order;

namespace FotoStorio.Server.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(IConfiguration config, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _config = config;
            _productRepository = productRepository;
        }

        public async Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(PaymentIntentCreateDTO paymentIntentCreateDTO)
        {
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var basket = paymentIntentCreateDTO.Items;

            if (basket == null) return null;

            // calculate basket total
            var basketTotal = (long)0m;

            foreach (var item in basket)
            {
                // get product details from the database
                var productItem = await _productRepository.GetByIdAsync(item.Product.Id);

                // is the product on offer or at regular price?
                var currentPrice = productItem.SalePrice != 0 && productItem.SalePrice < productItem.Price ? productItem.SalePrice : productItem.Price;
                
                // calculate the subtotal per item
                var itemTotal = item.Quantity * (currentPrice * 100);

                // add that to the overall total
                basketTotal += (long)itemTotal;
            }

            // create PaymentIntentService instance
            var service = new PaymentIntentService();

            // create new payment intent by sending basket amount and currency to Stripe
            PaymentIntent intent;

            var intentResult = new PaymentIntentResult { };

            if (string.IsNullOrEmpty(paymentIntentCreateDTO.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = basketTotal,
                    Currency = "gbp",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                // await json response from Stripe containing payment intent id and client_secret
                intent = await service.CreateAsync(options);

                if (intent != null)
                {
                    intentResult.ClientSecret = intent.ClientSecret;
                    intentResult.PaymentIntentId = intent.Id;
                }
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = basketTotal
                };

                // await json response from Stripe containing updated payment intent id and client_secret
                intent = await service.UpdateAsync(paymentIntentCreateDTO.PaymentIntentId, options);

                if (intent != null)
                {
                    intentResult.ClientSecret = intent.ClientSecret;
                    intentResult.PaymentIntentId = intent.Id;
                }
            }

            if (intentResult != null)
            {
                return intentResult;
            }

            return null;
        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _orderRepository.GetEntityWithSpecification(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentReceived;
            await _orderRepository.Save();

            return order;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _orderRepository.GetEntityWithSpecification(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentFailed;
            await _orderRepository.Save();

            return order;
        }
    }
}