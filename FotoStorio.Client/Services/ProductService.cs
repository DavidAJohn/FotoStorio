using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FotoStorio.Client.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models;

namespace FotoStorio.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClient;

        public ProductService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                var product = await client.GetFromJsonAsync<ProductDTO>($"products/{id}");

                return product;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message);
            }
        }

        public async Task<List<ProductDTO>> GetProductsAsync()
        {
            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                var products = await client.GetFromJsonAsync<List<ProductDTO>>("products?pageSize=20&brandId=2&sort=priceDesc");

                return products;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }

        public async Task<List<ProductDTO>> GetProductsByBrandAsync(int brandId)
        {
            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                var products = await client.GetFromJsonAsync<List<ProductDTO>>($"products?brandId={brandId}");

                return products;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }

        public async Task<List<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                var products = await client.GetFromJsonAsync<List<ProductDTO>>($"products?categoryId={categoryId}");

                return products;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }

        public async Task<List<Brand>> GetProductBrandsAsync()
        {
            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                var brands = await client.GetFromJsonAsync<List<Brand>>("brands");

                return brands;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }

        public async Task<List<Category>> GetProductCategoriesAsync()
        {
            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                var categories = await client.GetFromJsonAsync<List<Category>>("categories");

                return categories;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }
    }
}