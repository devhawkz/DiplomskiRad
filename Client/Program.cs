global using SharedLibrary.Models;
global using SharedLibrary.Responses;
global using Client.Models;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Client.Services.ProizvodServices;
using Client.Services.ToolsService;
using Client.Services.KategorijaServices;
using Client.Services.DialogServices;
using Client.Services.KorisnikServices;
using Client.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Client.Services.KorpaServices;

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

        builder.Services.AddScoped<IProizvodService, ProizvodService>();
        builder.Services.AddSingleton<IToolsService, Tools>();
        builder.Services.AddScoped<IKategorijaService, KategorijaService>();
        builder.Services.AddScoped<IKorisnikService, KorisnikService>();
        builder.Services.AddScoped<IKorpa, KorpaService>();

        builder.Services.AddScoped<AuthenticationService>();
        builder.Services.AddScoped<MessageDialogService>();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        builder.Services.AddAuthorizationCore();

        builder.Services.AddSyncfusionBlazor();
        builder.Services.AddBlazoredLocalStorage();

        await builder.Build().RunAsync();
    }
}
