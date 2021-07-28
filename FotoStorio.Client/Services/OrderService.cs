using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using FotoStorio.Client.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models.Orders;

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

        public async Task<OrderDTO> CreateOrderAsync(OrderCreateDTO order)
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

                    var createdOrder = JsonSerializer.Deserialize<Order>(
                        responseContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    var returnOrder = new OrderDTO {
                        OrderId = createdOrder.Id.ToString(),
                        SendToAddress = createdOrder.SendToAddress
                    };

                    return returnOrder;
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }
    }
}