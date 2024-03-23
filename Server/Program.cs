global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
using Server.Data;
using Server.Repository.KategorijaResposities;
using Server.Repository.ProizvodRespositories;
using Server.Repository.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using static System.Net.WebRequestMethods;
using Syncfusion.Blazor;
using Server.Repository.KorisnikRespository;

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
        builder.Services.AddScoped<IKorisnickiNalog, KorisnickiNalog>();



        // Identity & JWT autentifikacija
        //Identity // moze se kreirati i custom IdentityRole klasa sa dodatnim info o roli korisnika(naravno ta klasa nasledjuje IdentityRole klasu)
        builder.Services.AddIdentity<AppKorisnik, IdentityRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager()
            .AddRoles<IdentityRole>();


        // JWT
        //  koristi se u ASP.NET Core aplikacijama za dodavanje podrške za autentikaciju u Dependency Injection kontejner aplikacije.
        builder.Services.AddAuthentication(options =>
        {
            // oznacava da ce se koristiti JWT token za autentifikaciju korinsika
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            // ako je autentifikacija bila neuspesna, korisnik ce biti primoran  da pokusa opet, ako nema ove linije koda nakon neuspesne autentifikacije pojavice ce se error kod 404
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            // metoda konfigurira i dodaje podršku za JWT autentikaciju u ASP.NET Core aplikaciji, omogućujući aplikaciji da autenticira korisnike na temelju JWT tokena koji se šalju s HTTP zahtjevima
        }).AddJwtBearer(options =>
        {
            // Ovaj dio konfiguracije koristi se za definiranje parametara koji će se koristiti prilikom provjere i validacije JWT tokena.
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Postavljanje ValidateIssuer na true u TokenValidationParameters označava da će se provjeravati izdavač (Issuer) JWT tokena prilikom procesa validacije.
                ValidateIssuer = true,
                //označava da će se provjeravati auditorijum (Audience) JWT tokena prilikom procesa validacije.Proverava se da li taj token pripada nasoj apl.(samo JWT tokeni koje koristi nasa apl. su validni)
                ValidateAudience = true,
                //Kada je ValidateIssuerSigningKey postavljen na true, prilikom provjere validnosti JWT tokena, provjerava se da li ključ koji se koristi za potpisivanje tokena odgovara očekivanom ključu. 
                ValidateIssuerSigningKey = true,
                //Postavljanje ValidateLifetime na true u TokenValidationParameters označava da će se provjeravati valjanost vremenskog trajanja (lifetime) JWT tokena tijekom procesa validacije.
                ValidateLifetime = true,
                //Kada postavite ValidIssuer, JWT tokeni će se smatrati valjanima samo ako izdavač (Issuer) tokena odgovara vrijednosti koja je navedena u konfiguracijskim postavkama vaše aplikacije.
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                //Kada postavite ValidAudience, JWT tokeni će se smatrati valjanima samo ako auditorijum (Audience) tokena odgovara vrijednosti koja je navedena u konfiguracijskim postavkama vaše aplikacije
                ValidAudience = builder.Configuration["JWT:Audience"],
                //Kada postavite IssuerSigningKey, prilikom provjere validnosti JWT tokena provjerava se potpis tokena koji se odgovara ključu koji ste naveli. 
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });


        // Dodavanje autentifikacije u swaggerUI
        //koristi se u ASP.NET Core aplikacijama kako bi se dodala podrška za Swagger (OpenAPI) dokumentaciju u Dependency Injection kontejner aplikacije, Nakon dodavanja podrške za Swagger dokumentaciju, možete konfigurirati Swagger generiranje kako bi se prilagodila potrebama vaše aplikacije. Na primjer, možete dodati opise ruta, verzioniranje API-ja, dodati sigurnosne sheme i druge značajke koje želite uključiti u Swagger dokumentaciju.

        builder.Services.AddSwaggerGen(options =>
        { 
            //Ova linija koda dodaje definiciju sigurnosne sheme za Swagger dokumentaciju. Konkretno, definira sigurnosnu shemu nazvanu "oauth2" koja koristi tip ApiKey autentikacije i očekuje token u zaglavlju HTTP zahtjeva.
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                // ocekuje se token u zaglavlju http-a
                In = ParameterLocation.Header,
                // ime parametra u kojem se ocekuje token
                Name = "Authorization",
                //tip sigurnosne seme, koristimo ApiKey sto znaci da se token salje kao deo http zahteva
                Type = SecuritySchemeType.ApiKey
            });
            //ova linija koda dodaje operacijski filter u Swagger konfiguraciju. Konkretno, dodaje se SecurityRequirementsOperationFilter, koji se koristi za automatsko dodavanje sigurnosnih zahtjeva (security requirements) na svaku operaciju dokumentiranu u Swagger dokumentaciji.
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
