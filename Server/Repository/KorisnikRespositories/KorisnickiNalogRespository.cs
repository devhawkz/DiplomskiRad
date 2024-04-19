using Microsoft.AspNetCore.Mvc.Rendering;
using Server.Data;
using Server.Repository.Tools;
using SharedLibrary.DTOs;
using SharedLibrary.Responses;
using System.Security.Cryptography;

namespace Server.Repository.KorisnikRespositories;

public class KorisnickiNalogRespository(DataContext context, ITools tools
    ) : IKorisnickiNalog
{ 
    public async Task<PrijavaResponse> Prijava(PrijavaDTO model)
    {
        if (model is null)
            return new PrijavaResponse(false, "Morate uneti sve podatke!");

        var pronadjiKorisnika = await context.KorisnickiNalozi.FirstOrDefaultAsync(_ => _.Email!.Equals(model.Email!));
        if (pronadjiKorisnika is null)
            return new PrijavaResponse(false, "Korisnik nije pronađen!");

        if (!BCrypt.Net.BCrypt.Verify(model!.Lozinka, pronadjiKorisnika.Lozinka))
            return new PrijavaResponse(false, "Neispravan imejl/lozinka!");

        var (accessToken, refreshToken) = await tools.GenerisiTokene();

        //dodaje ili azurira TokenInfo
        await SacuvajTokenInfo(pronadjiKorisnika.Id, accessToken, refreshToken);

        return new PrijavaResponse(true, "Uspešna prijava!", accessToken, refreshToken);
    }

    private async Task SacuvajTokenInfo(int korisnikid, string accessToken, string refreshToken)
    {
        var getKorisnika = await context.TokenInfo
            .FirstOrDefaultAsync(_ => _.KorisnickiNalogId == korisnikid);
        // ako ne postoji vec kreirani kreira se novi tokeninfo objekat za korisnika
        if(getKorisnika is null)
        {
            context.TokenInfo.Add(new TokenInfo()
            {
                KorisnickiNalogId = korisnikid,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
            await tools.Sacuvaj();
        }

        // ako postoji azuriraju se njegovi podaci 
        else
        {
            getKorisnika.RefreshToken = refreshToken;
            getKorisnika.AccessToken = accessToken;
            getKorisnika.DatumIsteka = DateTime.Now.AddDays(1);
            await tools.Sacuvaj();
        }
    }

    public async Task<ServiceResponse> Registracija(KorisnikDTO model)
    {
        if (model is null)
            return new ServiceResponse(false, "Morate uneti sve podatke!");

        var pronadji = await context.KorisnickiNalozi.FirstOrDefaultAsync(_ => _.Email!.ToLower().Equals(model.Email!.ToLower()));
        if (pronadji is not null)
            return new ServiceResponse(false, "Korisnik sa tim imejlom već postoji");

        var korisnik = context.KorisnickiNalozi.Add(new KorisnickiNalog()
        {
            Ime = model.Ime,
            Email = model.Email,
            Lozinka = BCrypt.Net.BCrypt.HashPassword(model.Lozinka)
        }).Entity;

        await tools.Sacuvaj();

        // dodeljivanje uloge
        var proveraAdmina = await context.Uloge.FirstOrDefaultAsync(_ => _.Naziv!.ToLower().Equals("admin"));
        
        if(proveraAdmina is null)
        {
            var rezultat = context.Uloge.Add(new Uloga()
            {
                Naziv = "Admin"
            }).Entity;

            await tools.Sacuvaj();

            // povezujemo korisnika sa ulogom
            context.KorisnickeUloge.Add(new KorisnickaUloga() 
            {
                UlogaId = rezultat.Id,
                KorisnickiNalogId  = korisnik.Id
            });
            await tools.Sacuvaj();
        }

        else
        {
            var proveraKorisnika = await context.Uloge.FirstOrDefaultAsync(_ => _.Naziv!.ToLower().Equals("korisnik"));
            int UlogaID = 0;

            if(proveraKorisnika is null)
            {
                var korisnikRezultat = context.Uloge.Add(new Uloga()
                {
                    Naziv = "Korisnik"
                }).Entity;

                await tools.Sacuvaj();
                UlogaID = korisnikRezultat.Id;

            }

            context.KorisnickeUloge.Add(new KorisnickaUloga() 
            {
                UlogaId = UlogaID == 0 ? proveraKorisnika!.Id : UlogaID,
                 KorisnickiNalogId  = korisnik.Id
            });
            await tools.Sacuvaj();
        }

        return new ServiceResponse(true, "Nalog je uspešno kreiran!");

    }

    public Task<SesijaKorisnika> GetKorisnikaPoTokenu(string token)
    {
        throw new NotImplementedException();
    }


    public Task<PrijavaResponse> GetRefreshToken(PostRefreshTokenDTO model)
    {
        throw new NotImplementedException();
    }
}
