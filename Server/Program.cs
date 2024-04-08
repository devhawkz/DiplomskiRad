global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Server.Data;
using Server.Repository.KategorijaResposities;
using Server.Repository.ProizvodRespositories;
using Server.Repository.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Server.Repository.KorisnikRespositories;
using System.Runtime;
using Swashbuckle.AspNetCore.Filters;

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
        

        // pocetak
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Konekcioni string nije pronadjen"));
        });

        builder.Services.AddScoped<IProizvod, ProizvodRepository>();
        builder.Services.AddScoped<ITools, ToolsRespository>();
        builder.Services.AddScoped<IKategorija, KategorijaRespository>();
        builder.Services.AddScoped<IKorisnik, KorisnikRespository>();


        // Identity i JWT autentifikacija
        //Identity

        builder.Services.AddIdentity<AppKorisnik, IdentityRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager() // ne koristi se mozda
            .AddRoles<IdentityRole>();


        //JWT
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
            };
        });


        // Dodavanje autentifikacije u swagger ui
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        // kraj


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
