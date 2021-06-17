using System.Collections.Generic;
using System.Threading.Tasks;
using FotoStorio.Shared.Models;

namespace FotoStorio.Client.Contracts
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
    }
}