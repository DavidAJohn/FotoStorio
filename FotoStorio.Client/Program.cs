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
using Polly;
using Serilog;
using Polly.Extensions.Http;

namespace FotoStorio.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("FotoStorioAPI", c => c.BaseAddress = 
                new Uri(builder.HostEnvironment.BaseAddress + "api/"))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            
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

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            // Retry with jitter: https://github.com/App-vNext/Polly/wiki/Retry-with-jitter
            Random jitter = new Random();

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))  // exponential backoff (2, 4, 8, 16, 32 secs)
                        + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)), // plus some jitter: up to 1 second
                    onRetry: (exception, retryCount, context) =>
                    {
                        Log.Warning("Retry {retryCount} of {policyKey} at {operationKey}, due to: {exception}.",
                            retryCount, context.PolicyKey, context.OperationKey, exception);
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );
        }
    }
}
