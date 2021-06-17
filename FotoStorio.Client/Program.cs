using FotoStorio.Client.Contracts;
using FotoStorio.Client.Services;
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

            await builder.Build().RunAsync();
        }
    }
}
