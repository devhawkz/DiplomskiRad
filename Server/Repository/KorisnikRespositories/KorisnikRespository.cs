using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using SharedLibrary;
using SharedLibrary.DTOs;
using SharedLibrary.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Repository.KorisnikRespositories;

public class KorisnikRespository(
    UserManager<AppKorisnik> userManager,
    RoleManager<IdentityRole> roleManager,
    SignInManager<AppKorisnik> signInManager,
    IConfiguration config) 
    : IKorisnik
{
    
    public async Task<ServiceResponse> Registracija(KorisnikDTO korisnikDTO)
    {
        if (korisnikDTO is null) return new ServiceResponse(false, "Nisu uneti odgovarajući podaci!");

        var noviKorisnik = new AppKorisnik()
        {
            Ime = korisnikDTO.Ime,
            Email = korisnikDTO.Email,
            PasswordHash = korisnikDTO.Lozinka,
            UserName = korisnikDTO.Email
        };

        var korisnik = await userManager.FindByEmailAsync(noviKorisnik.Email);
        if (korisnik is not null) return new ServiceResponse(false, "Korisnik sa datom email adresom već postoji!");

        var kreiranjeKorisnika = await userManager.CreateAsync(noviKorisnik!, korisnikDTO.Lozinka);
        if (!kreiranjeKorisnika.Succeeded) return new ServiceResponse(false, "Došlo je do greške. Molimo pokušajte kasnije!");

        


        // DodeljivanjeUloge
        var checkAdmin = await roleManager.FindByNameAsync("Admin");
        if(checkAdmin is null)
        {
            await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
            await userManager.AddToRoleAsync(noviKorisnik, "Admin");
            //await signInManager.SignInAsync(noviKorisnik, isPersistent: false); // auto. prijava nakon registracije korisnika
            return new ServiceResponse(true, "Nalog uspešno kreiran!");
        }
        
        else
        {
            var checkUser = await roleManager.FindByNameAsync("User");
            if (checkUser is null)
                await roleManager.CreateAsync(new IdentityRole() { Name = "User" });

            await userManager.AddToRoleAsync(noviKorisnik, "User");
            //await signInManager.SignInAsync(noviKorisnik, isPersistent: false);
            return new ServiceResponse(true, "Nalog uspešno kreiran!");
            
        }
        
    }


    public async Task<PrijavaResponse> Prijava(PrijavaDTO prijavaDTO)
    {
        if (prijavaDTO is null) return new PrijavaResponse(false, null!, "Nisu uneti svi podaci!");

        var getKorisnika = await userManager.FindByEmailAsync(prijavaDTO.Email);
        if (getKorisnika is null) return new PrijavaResponse(false, null!, "Nije pronadjen korisnik sa tom email adresom");

        bool proveraLozinke = await userManager.CheckPasswordAsync(getKorisnika, prijavaDTO.Lozinka);
        if (!proveraLozinke)
            return new PrijavaResponse(false, null!, "Neispravna email adresa / lozinka");

        var getRoluKorisnika = await userManager.GetRolesAsync(getKorisnika);
        var sesija = new SesijaKorisnika(getKorisnika.Id, getKorisnika.Ime, getKorisnika.Email, getRoluKorisnika.First());
        string token = GeneratorTokena(sesija);
        return new PrijavaResponse(true, token!, "Uspešna prijava")
;    }

    private string GeneratorTokena(SesijaKorisnika sesija)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, sesija.Id),
            new Claim(ClaimTypes.Name, sesija.Ime),
            new Claim(ClaimTypes.Role, sesija.Rola)
        };

        var token = new JwtSecurityToken(
            issuer: config["JWT:Issuer"],
            audience: config["JWT:Audience"],
            claims: userClaims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
