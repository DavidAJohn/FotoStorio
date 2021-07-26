using FotoStorio.Server.Contracts;
using FotoStorio.Shared.Models.Orders;

namespace FotoStorio.Server.Data
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}