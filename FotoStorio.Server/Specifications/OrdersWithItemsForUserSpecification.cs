using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Specifications
{
    public class OrdersWithItemsForUserSpecification : BaseSpecification<Order>
    {
        public OrdersWithItemsForUserSpecification(string email)
            : base(o => o.BuyerEmail == email)
        {
            AddInclude(o => o.OrderItems);
            ApplyOrderByDescending(o => o.OrderDate);
        }

        public OrdersWithItemsForUserSpecification(int id, string email) 
            : base(o => o.Id == id && o.BuyerEmail == email)
        {
            AddInclude(o => o.OrderItems);
        }
    }
}