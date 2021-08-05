using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;
using FotoStorio.Client.Contracts;
using FotoStorio.Client.Providers;
using FotoStorio.Client.Services;
using FotoStorio.Shared.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FotoStorio.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("FotoStorioAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/"));

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            
            builder.Services.AddApiAuthorization(opt => opt.UserOptions.RoleClaim = "role");
            builder.Services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("IsAdmin", policyBuilder =>
                {
                    policyBuilder.RequireClaim("role", "Administrator");

                });

                config.AddPolicy("IsUser", policyBuilder =>
                {
                    policyBuilder.RequireClaim("role", "User");

                });
            });

            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddBlazoredToast();

            await builder.Build().RunAsync();
        }
    }
}
