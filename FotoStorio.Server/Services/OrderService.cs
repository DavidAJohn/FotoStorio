using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FotoStorio.Server.Contracts;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Services
{
    public class OrderService : IOrderService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        public OrderService(IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, Order order, Address sendToAddress)
        {
            // get items (crucially, with price from db) from the products repository
            var items = new List<OrderItem>();

            var basket = order.OrderItems;

            foreach (var item in basket)
            {
                var productItem = await _productRepository.GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.ImageUrl);

                // is the product on offer or at regular price?
                var currentPrice = productItem.SalePrice != 0 && productItem.SalePrice < productItem.Price ? productItem.SalePrice : productItem.Price;
                
                var orderItem = new OrderItem(itemOrdered, currentPrice, item.Quantity);
                items.Add(orderItem);
            }

            // calculate the subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            // create the order and save changes
            var orderToCreate = new Order(items, buyerEmail, sendToAddress, subtotal, "");
            var newOrder = await _orderRepository.Create(orderToCreate);
            
            // return the new order
            if (newOrder != null) 
            {
                return newOrder;
            }

            return null;
        }

        public Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            throw new System.NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            throw new System.NotImplementedException();
        }
    }
}