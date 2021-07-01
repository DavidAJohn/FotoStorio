using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models;

namespace FotoStorio.Client.Contracts
{
    public interface IProductService
    {
        Task<PagedList<ProductDTO>> GetProductsAsync(ProductParameters productParameters);
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<List<ProductDTO>> GetProductsByBrandAsync(int brandId);
        Task<List<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Brand>> GetProductBrandsAsync();
        Task<List<Category>> GetProductCategoriesAsync();
    }
}