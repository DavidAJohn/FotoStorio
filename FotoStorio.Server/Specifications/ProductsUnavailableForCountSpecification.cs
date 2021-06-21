using FotoStorio.Server.Helpers;
using FotoStorio.Shared.Models;

namespace FotoStorio.Server.Specifications
{
    public class ProductsUnavailableForCountSpecification : BaseSpecification<Product>
    {
        public ProductsUnavailableForCountSpecification(ProductSpecificationParams productParams)
            : base(p =>
                (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
                (!productParams.BrandId.HasValue || p.BrandId == productParams.BrandId) &&
                (!productParams.CategoryId.HasValue || p.CategoryId == productParams.CategoryId) &&
                p.IsAvailable == false
            )
        {
        }
    }
}
