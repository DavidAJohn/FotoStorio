﻿using FotoStorio.Server.Helpers;
using FotoStorio.Shared.Models;

namespace FotoStorio.Server.Specifications
{
    public class ProductsUnavailableSpecification : BaseSpecification<Product>
    {
        public ProductsUnavailableSpecification(ProductSpecificationParams productParams) 
            : base(p =>
                (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
                (!productParams.BrandId.HasValue || p.BrandId == productParams.BrandId) &&
                (!productParams.CategoryId.HasValue || p.CategoryId == productParams.CategoryId) &&
                p.IsAvailable == false
            )
        {
            // Includes
            AddInclude(p => p.Brand);
            AddInclude(p => p.Category);

            // Paging
            ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

            // Sorting
            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "nameAsc":
                        ApplyOrderBy(p => p.Name);
                        break;
                    case "nameDesc":
                        ApplyOrderByDescending(p => p.Name);
                        break;
                    case "brandAsc":
                        ApplyOrderBy(p => p.Brand.Name);
                        break;
                    case "brandDesc":
                        ApplyOrderByDescending(p => p.Brand.Name);
                        break;
                    case "priceAsc":
                        ApplyOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        ApplyOrderByDescending(p => p.Price);
                        break;
                    case "idAsc":
                        ApplyOrderBy(p => p.Id);
                        break;
                    case "idDesc":
                        ApplyOrderByDescending(p => p.Id);
                        break;
                    default:
                        ApplyOrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                ApplyOrderBy(p => p.Name);
            }
        }
    }
}
