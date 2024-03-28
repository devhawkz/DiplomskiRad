using Microsoft.IdentityModel.Tokens;
using Server.Data;
using SharedLibrary;
using SharedLibrary.DTOs;
using SharedLibrary.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Server.Repository.KorisnikRespository;

public class KorisnickiNalog(UserManager<AppKorisnik> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config) : IKorisnickiNalog
{
    public async Task<ServiceResponse> Registracija(KorisnikDTO korisnikDTO)
    {
        if (korisnikDTO is null) return new ServiceResponse(false, "Uneti neispravni podaci");

        var noviKorisnik = new AppKorisnik
        {
            Ime = korisnikDTO.Ime,
            Email = korisnikDTO.Email,
            // enkriptuje lozinku auto -- hash i salt koncept koristi
            PasswordHash = korisnikDTO.Lozinka,
            UserName = korisnikDTO.Email
        };

        // pristupa dbcontext klasi i trazi korisnika sa datim mejlom
        var korisnik = await userManager.FindByEmailAsync(noviKorisnik.Email);
        if (korisnik is not null) return new ServiceResponse(false, "Korisnik sa tom imejl adresom već postoji.");

        var kreirajKorisnika = await userManager.CreateAsync(noviKorisnik!, korisnikDTO.Lozinka);
        if (!kreirajKorisnika.Succeeded) return new ServiceResponse(false, "Došlo je do greške... Pokušajte ponovo");


        //Dodeljivanje default role : Admin za prvu registraciju, ostalo su korisnici

        // trazi rolu Admin u Identity sistemu, ako je ne pronadje to znaci da ne postoji korisnicki nalog sa tom rolom
        var checkAdmin = await roleManager.FindByNameAsync("Admin");
        if (checkAdmin is null)
        {
            // kreira novu rolu u identity sistemu
            await roleManager.CreateAsync(new IdentityRole() { Name = "Admin"});
            // dodaje se rola korisniku
            await userManager.AddToRoleAsync(noviKorisnik, "Admin");
            return new ServiceResponse(true, "Nalog je kreiran");
        }
        else
        {
            var checkUser = await roleManager.FindByNameAsync("User");
            if(checkUser is null)
                await roleManager.CreateAsync(new IdentityRole() { Name = "User" });

            await userManager.AddToRoleAsync(noviKorisnik, "User");
            return new ServiceResponse(true, "Nalog je kreiran");

        }

    }

    public async Task<PrijavaResponse> Prijava(PrijavaDTO prijavaDTO)
    {
        if (prijavaDTO is null) return new PrijavaResponse(false, null!, "Morate popuniti sva polja");

        var getKorisnika = await userManager.FindByEmailAsync(prijavaDTO.Email);
        if(getKorisnika is null) return new PrijavaResponse(false, null!, "Ne postoji korisnik sa tom imejl adresom!");

        // proverava da li se lozinka korisnika iz baze poklapa sa unetom lozinkom naloga sa istim imejlom u bazi kao i u unosu prilikom prijave
        bool proveraLozinke = await userManager.CheckPasswordAsync(getKorisnika, prijavaDTO.Lozinka);
        if (!proveraLozinke) return new PrijavaResponse(false, null!, "Pogrešan imejl ili lozinka");

        //uzima se rola korisnika
        var getRoluKorisnika = await userManager.GetRolesAsync(getKorisnika);

        // sluzi za upravljanje korisnickim info tokom trajanja sesije, .First() vraca prvu rolu korisnika
        var sesijaKorisnika = new SesijaKorisnika(getKorisnika.Id, getKorisnika.Ime, getKorisnika.Email, getRoluKorisnika.First());

        string token= GenerisiToken(sesijaKorisnika);
        return new PrijavaResponse(true, token, "Uspešna prijava");
    }

    private string GenerisiToken(SesijaKorisnika sesijaKorisnika)
    {
        var sigurnosniKljuc = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
        //Ova linija koda kreira nove potpisne kredencijale(SigningCredentials) koje se koriste za potpisivanje tokena ili digitalnih poruka
        var credentials = new SigningCredentials(sigurnosniKljuc, SecurityAlgorithms.HmacSha512);
    
        //Ova linija koda kreira niz korisničkih tvrdnji(claims) koje se obično koriste za identifikaciju i autorizaciju korisnika unutar sistema
                var korisnickiClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, sesijaKorisnika.Id),
            new Claim(ClaimTypes.Name, sesijaKorisnika.Ime),
            new Claim(ClaimTypes.Email, sesijaKorisnika.Email),
            new Claim(ClaimTypes.Role, sesijaKorisnika.Role)
        };

        var token = new JwtSecurityToken(
            issuer: config["JWT:Issuer"],
            audience: config["JWT:Audience"],
            claims: korisnickiClaims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
            );

        //Ova linija koda generira JWT token iz predanog tokena
       return new JwtSecurityTokenHandler().WriteToken(token);

    }
}
