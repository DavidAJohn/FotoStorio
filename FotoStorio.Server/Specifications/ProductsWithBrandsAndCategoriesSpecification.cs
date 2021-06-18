using FotoStorio.Shared.Models;

namespace FotoStorio.Server.Specifications
{
    public class ProductsWithBrandsAndCategoriesSpecification : BaseSpecification<Product>
    {
        public ProductsWithBrandsAndCategoriesSpecification()
        {
            AddInclude(p => p.Brand);
            AddInclude(p => p.Category);
        }

        public ProductsWithBrandsAndCategoriesSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Brand);
            AddInclude(p => p.Category);
        }
    }
}