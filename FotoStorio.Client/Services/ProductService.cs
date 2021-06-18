using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FotoStorio.Client.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models;
using Microsoft.Extensions.Logging;

namespace FotoStorio.Client.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IHttpClientFactory httpClient, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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
                _logger.LogError(ex.StatusCode + " " + ex.Message);
                throw new HttpRequestException(ex.Message);
            }
        }

        public async Task<List<ProductDTO>> GetProductsAsync()
        {
            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                var products = await client.GetFromJsonAsync<List<ProductDTO>>("products");

                return products;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex.StatusCode + " " + ex.Message);
                throw new HttpRequestException(ex.Message);
            }
        }
    }
}