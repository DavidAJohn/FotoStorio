using FotoStorio.Server.Contracts;
using FotoStorio.Server.Specifications;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Services
{
    public class OrderService : IOrderService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;

        public OrderService(IProductRepository productRepository, IOrderRepository orderRepository, IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, OrderCreateDTO order)
        {
            // get items (crucially, with price from db) from the products repository
            var items = new List<OrderItem>();

            var basket = order.Items;

            foreach (var item in basket)
            {
                var productItem = await _productRepository.GetByIdAsync(item.Product.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.ImageUrl);

                // is the product on offer or at regular price?
                var currentPrice = productItem.SalePrice != 0 && productItem.SalePrice < productItem.Price ? productItem.SalePrice : productItem.Price;
                
                var orderItem = new OrderItem(itemOrdered, currentPrice, item.Quantity);
                items.Add(orderItem);
            }

            // calculate the total
            var total = items.Sum(item => item.Price * item.Quantity);

            // check for an existing order
            var spec = new OrderByPaymentIntentIdSpecification(order.PaymentIntentId);
            var existingOrder = await _orderRepository.GetEntityWithSpecification(spec);

            if (existingOrder != null)
            {
                await _orderRepository.Delete(existingOrder);

                var pi = new PaymentIntentCreateDTO {
                    Items = order.Items,
                    PaymentIntentId = order.PaymentIntentId
                };

                await _paymentService.CreateOrUpdatePaymentIntent(pi);
            }

            // create the order and save changes
            var orderToCreate = new Order(items, buyerEmail, order.SendToAddress, total, order.PaymentIntentId);
            var newOrder = await _orderRepository.Create(orderToCreate);
            
            // return the new order
            if (newOrder != null) 
            {
                return newOrder;
            }

            return null;
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsForUserSpecification(id, buyerEmail);
            return await _orderRepository.GetEntityWithSpecification(spec);
        }

        public async Task<IEnumerable<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsForUserSpecification(buyerEmail);
            return await _orderRepository.ListWithSpecificationAsync(spec);
        }
    }
}