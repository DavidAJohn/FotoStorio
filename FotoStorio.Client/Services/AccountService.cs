using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using FotoStorio.Client.Contracts;
using FotoStorio.Shared.DTOs;
using FotoStorio.Shared.Models.Identity;

namespace FotoStorio.Client.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILocalStorageService _localStorage;
        
        public AccountService(IHttpClientFactory httpClient, ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public async Task<AddressDTO> GetUserAddressAsync() 
        {
            var storedToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(storedToken))
            {
                return new AddressDTO {};
            }

            try
            {
                var client = _httpClient.CreateClient("FotoStorioAPI");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);

                var address = await client.GetFromJsonAsync<AddressDTO>("accounts/address");

                if (address == null)
                {
                    return new AddressDTO {};
                }

                return address;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
            }
        }
        
        public async Task<AddressDTO> SaveUserAddressAsync(AddressDTO addressDTO)
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

                HttpContent serializedContent = new StringContent(JsonSerializer.Serialize(addressDTO));
                serializedContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PutAsync("accounts/address", serializedContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var updatedAddress = JsonSerializer.Deserialize<AddressDTO>(
                        responseContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    return updatedAddress;
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