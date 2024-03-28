global using SharedLibrary.Models;
global using SharedLibrary.Responses;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Client.Services.ProizvodServices;
using Client.Services.ToolsService;
using Client.Services.KategorijaServices;
using Client.Services.DialogServices;
using Blazored.LocalStorage;
using Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Client.Services.AuthServices;


namespace Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzEzNDk2OEAzMjM0MmUzMDJlMzBYeU9BTmtkOHVzNjdYVVZ6dlZ5bXR6d21zcmJVUkdzV3AxS1liVUVZTjBJPQ==");

        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7151/") });

        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

        builder.Services.AddScoped<IProizvodService, ClientServices>();
        builder.Services.AddSingleton<IToolsService, Tools>();
        builder.Services.AddScoped<IKategorijaService, ClientServices>();
        builder.Services.AddScoped<MessageDialogService>();
        builder.Services.AddScoped<INalog, NalogService>();

       
        builder.Services.AddSyncfusionBlazor();


        await builder.Build().RunAsync();
    }
}
