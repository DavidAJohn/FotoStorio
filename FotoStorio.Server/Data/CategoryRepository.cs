using FotoStorio.Server.Contracts;
using FotoStorio.Shared.Models;

namespace FotoStorio.Server.Data
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext repositoryContext) : base(repositoryContext)
        {
        }
    }
}
