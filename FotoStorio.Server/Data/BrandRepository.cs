using FotoStorio.Server.Contracts;
using FotoStorio.Shared.Models;

namespace FotoStorio.Server.Data
{
    public class BrandRepository : RepositoryBase<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
