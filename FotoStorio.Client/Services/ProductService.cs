using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FotoStorio.Client.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using FotoStorio.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;

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

        public async Task<PagedList<ProductDTO>> GetProductsAsync(ProductParameters productParams)
        {
            try
            {
                var request = new HttpRequestMessage();

                if (productParams != null) 
                {
                    var queryStringParams = new Dictionary<string, string>
                    {
                        ["pageIndex"] = productParams.PageIndex < 1  ? "1" : productParams.PageIndex.ToString(),
                    };

                    // conditionally add a page size param (number of items to return)
                    if (productParams.PageSize != 0)
                    {
                        queryStringParams.Add("pageSize", productParams.PageSize.ToString());
                    };

                    // conditionally add a search term
                    if (!String.IsNullOrEmpty(productParams.Search))
                    {
                        queryStringParams.Add("search", productParams.Search.ToString());
                    };

                    // conditionally add a categoryId param
                    if (productParams.CategoryId != 0)
                    {
                        queryStringParams.Add("categoryId", productParams.CategoryId.ToString());
                    };

                    // conditionally add a brandId param
                    if (productParams.BrandId != 0)
                    {
                        queryStringParams.Add("brandId", productParams.BrandId.ToString());
                    };

                    // conditionally add a sort param
                    if (!String.IsNullOrEmpty(productParams.SortBy))
                    {
                        queryStringParams.Add("sort", productParams.SortBy.ToString());
                    };

                    request = new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("products", queryStringParams));
                }
                else
                {
                    request = new HttpRequestMessage(HttpMethod.Get, "products");
                }

                var client = _httpClient.CreateClient("FotoStorioAPI");
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var pagedResponse = new PagedList<ProductDTO>
                    {
                        Items = JsonSerializer.Deserialize<List<ProductDTO>>(
                            content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        ),
                        Metadata = JsonSerializer.Deserialize<PagingMetadata>(
                            response.Headers.GetValues("Pagination")
                            .First(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        )
                    };

                    return pagedResponse;
                }

                return null;
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