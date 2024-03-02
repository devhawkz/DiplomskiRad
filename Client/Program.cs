global using SharedLibrary.Contracts;
global using SharedLibrary.Models;
global using SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client.Services;
using Syncfusion.Blazor;

namespace Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzEzNDk2OEAzMjM0MmUzMDJlMzBYeU9BTmtkOHVzNjdYVVZ6dlZ5bXR6d21zcmJVUkdzV3AxS1liVUVZTjBJPQ==");

        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddScoped<IProizvod, ClientServices>();

        builder.Services.AddSyncfusionBlazor();

        await builder.Build().RunAsync();
    }
}
