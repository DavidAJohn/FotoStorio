using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using FotoStorio.Client.Contracts;
using FotoStorio.Shared.DTOs;

namespace FotoStorio.Client.Services
{
    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILocalStorageService _localStorage;
        public OrderService(IHttpClientFactory httpClient, ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public async Task<OrderDetailsDTO> CreateOrderAsync(OrderCreateDTO order)
        {
            if (order == null)
            {
                return null;
            }

            var storedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(storedToken))
            {
                return null;
            }

            try 
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

                HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(order));
                serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("orders", serializedContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var createdOrder = JsonSerializer.Deserialize<OrderDetailsDTO>(
                        responseContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    return createdOrder;
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }

        public async Task<List<OrderDetailsDTO>> GetOrdersForUserAsync()
        {
            var storedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(storedToken))
            {
                return null;
            }

            try 
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

                var orders = await client.GetFromJsonAsync<List<OrderDetailsDTO>>("orders");

                return orders;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }

        public async Task<OrderDetailsDTO> GetOrderByIdAsync(int orderId)
        {
            var storedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(storedToken))
            {
                return null;
            }

            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

                var order = await client.GetFromJsonAsync<OrderDetailsDTO>($"orders/{orderId}");

                return order;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }
    }
}