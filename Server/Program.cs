global using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Repository.KategorijaResposities;
using Server.Repository.KorisnikRespositories;
using Server.Repository.ProizvodRespositories;
using Server.Repository.Tools;
using Server.Repository.NaplataRespositories;
using Server.Repository.EmailRespository;
using Microsoft.AspNetCore.Authentication;
using Server.AuthHandler;
using Stripe;



namespace Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        // pocetak
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Konekcioni string nije pronadjen"));
        });

        // konfig autorizacije i autentfikacije
        builder.Services.AddAuthentication("CustomScheme")
            .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("CustomScheme", null);

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("Korisnik"));
        });

        builder.Services.AddScoped<IProizvod, ProizvodRepository>();
        builder.Services.AddScoped<ITools, ToolsRespository>();
        builder.Services.AddScoped<IKategorija, KategorijaRespository>();
        builder.Services.AddScoped<IKorisnickiNalog, KorisnickiNalogRespository>();
        builder.Services.AddScoped<INaplata, NaplataRespository>();
        builder.Services.AddTransient<IEmail, EmailService>();




		var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseWebAssemblyDebugging();
        }

      
        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapRazorPages();
        
        app.MapControllers();

        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
