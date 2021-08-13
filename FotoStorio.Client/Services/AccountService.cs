using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        
        public async Task<Address> SaveUserAddressAsync(Address address)
        {
            // var savedToken = await _localStorage.GetItemAsync<string>("authToken");

            // if (string.IsNullOrWhiteSpace(savedToken))
            // {
            //     return null;
            // }

            // var client = _httpClient.CreateClient("FotoStorioAPI");
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

            // string json = JsonConvert.SerializeObject(address);
            // StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // var response = await _httpClient.PutAsync(GetApiEndpoint("address"), httpContent);

            // if (response.IsSuccessStatusCode)
            // {
            //     var newAddress = JsonConvert.DeserializeObject<Address>(
            //         await response.Content.ReadAsStringAsync()
            //     );

            //     return newAddress;
            // }

            return null;
        }
    }
}