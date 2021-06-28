using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models;

namespace FotoStorio.Client.Contracts
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetProductsAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task<List<ProductDTO>> GetProductsByBrandAsync(int brandId);
        Task<List<ProductDTO>> GetProductsByCategoryAsync(int categoryId);
        Task<List<Brand>> GetProductBrandsAsync();
        Task<List<Category>> GetProductCategoriesAsync();
    }
}