using Blazored.LocalStorage;
using FotoStorio.Client.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace FotoStorio.Client.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILocalStorageService _localStorage;

        public PaymentService(IHttpClientFactory httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<PaymentIntentResult> CreateOrUpdatePaymentIntent(Basket basket)
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

                var intentToCreate = new PaymentIntentCreateDTO
                {
                    Items = basket.BasketItems,
                    PaymentIntentId = basket.PaymentIntentId
                };

                HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(intentToCreate));
                serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("payments", serializedContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var createdIntent = JsonSerializer.Deserialize<PaymentIntentResult>(
                        responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    return createdIntent;
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
