using FotoStorio.Server.Contracts;
using FotoStorio.Shared.Models;

namespace FotoStorio.Server.Data
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
