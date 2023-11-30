using Blazored.LocalStorage;
using FotoStorio.Client.Contracts;
using FotoStorio.Client.Providers;
using FotoStorio.Shared.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FotoStorio.Client.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthenticationService(IHttpClientFactory httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public async Task<RegisterResult> Register(RegisterModel registerModel)
    {
        var client = _httpClient.CreateClient("FotoStorioAPI");
        var response = await client.PostAsJsonAsync("accounts/register", registerModel);

        var responseContent = await response.Content.ReadAsStringAsync();
        var registerResult = JsonSerializer.Deserialize<RegisterResult>(responseContent, _serializerOptions);

        if (response.IsSuccessStatusCode)
        {
            return new RegisterResult
            {
                Successful = true
            };
        }

        return new RegisterResult
        {
            Successful = false,
            Error = registerResult.Error
        };
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        var client = _httpClient.CreateClient("FotoStorioAPI");
        var response = await client.PostAsJsonAsync("accounts/login", loginModel);

        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<LoginResult>(responseContent, _serializerOptions);

        if (!response.IsSuccessStatusCode)
        {
            loginResult.Successful = false;
            loginResult.Error = "Invalid username or password";
            
            return loginResult;
        }

        loginResult.Successful = true;

        await _localStorage.SetItemAsync("authToken", loginResult.Token);

        ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated(loginModel.Email);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

        return loginResult;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");

        ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();

        var client = _httpClient.CreateClient("FotoStorioAPI");
        client.DefaultRequestHeaders.Authorization = null;
    }

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}