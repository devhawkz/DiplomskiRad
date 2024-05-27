using Microsoft.AspNetCore.WebUtilities;
using Server.Data;
using Server.Repository.EmailRespository;
using Server.Repository.Tools;
using SharedLibrary.DTOs;
using SharedLibrary.Responses;
using System.Text;

namespace Server.Repository.KorisnikRespositories;

public class KorisnickiNalogRespository(DataContext context, ITools tools, IEmail emailService) : IKorisnickiNalog
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
                RefreshToken = refreshToken,
                DatumIsteka = DateTime.Now

            });
            await tools.Sacuvaj();
        }

        // ako postoji azuriraju se njegovi podaci 
        else
        {
            getKorisnika.RefreshToken = refreshToken;
            getKorisnika.AccessToken = accessToken;
            getKorisnika.DatumIsteka = DateTime.Now.AddMinutes(15);
            await tools.Sacuvaj();
        }
    }

    private async Task<ServiceResponse> IspravnostPodataka(KorisnikDTO model)
    {
        if (model is null)
            return new ServiceResponse(false, "Morate uneti sve podatke!");

        var pronadji = await context.KorisnickiNalozi.FirstOrDefaultAsync(_ => _.Email!.ToLower().Equals(model.Email!.ToLower()));
        if (pronadji is not null)
            return new ServiceResponse(false, "Korisnik sa tim imejlom već postoji");

        return new ServiceResponse(true, "Svi uneti podaci su ispravni");

    }


    public async Task<ServiceResponse> RegistracijaKorisnika(KorisnikDTO model)
    {

        var ispravniPodaci = await IspravnostPodataka(model);

        if (!ispravniPodaci.Flag)
            return new ServiceResponse(false, ispravniPodaci.Poruka);

        else
        {
            var korisnik = context.KorisnickiNalozi.Add(new KorisnickiNalog()
            {
                Ime = model.Ime,
                Email = model.Email,
                Lozinka = BCrypt.Net.BCrypt.HashPassword(model.Lozinka)
            }).Entity;

            await tools.Sacuvaj();
            
            // dodeljivanje uloge
            var proveraKorisnika = await context.Uloge.FirstOrDefaultAsync(_ => _.Naziv!.ToLower().Equals("korisnik"));
            int UlogaID = 0;

            if (proveraKorisnika is null)
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
                KorisnickiNalogId = korisnik.Id
            });
            await tools.Sacuvaj();


            await emailService.SendEmailAsync(model.Email!, "Registracija Uspešna", "Vaša registracija je uspešno završena.");

            return new ServiceResponse(true, "Nalog je uspešno kreiran!");
        }
       

    }

    public async Task<ServiceResponse> RegistracijaAdmina(KorisnikDTO model)
    {

        var ispravniPodaci = await IspravnostPodataka(model);

        if (!ispravniPodaci.Flag)
            return new ServiceResponse(false, ispravniPodaci.Poruka);

        else
        {
            var korisnik = context.KorisnickiNalozi.Add(new KorisnickiNalog()
            {
                Ime = model.Ime,
                Email = model.Email,
                Lozinka = BCrypt.Net.BCrypt.HashPassword(model.Lozinka)
            }).Entity;

            await tools.Sacuvaj();

            // dodeljivanje uloge
            var proveraAdmina = await context.Uloge.FirstOrDefaultAsync(_ => _.Naziv!.ToLower().Equals("Admin"));
            int UlogaID = 0;

            if (proveraAdmina is null)
            {
                var adminRezultat = context.Uloge.Add(new Uloga()
                {
                    Naziv = "Admin"
                }).Entity;

                await tools.Sacuvaj();
                UlogaID = adminRezultat.Id;

            }

            context.KorisnickeUloge.Add(new KorisnickaUloga()
            {
                UlogaId = UlogaID == 0 ? proveraAdmina!.Id : UlogaID,
                KorisnickiNalogId = korisnik.Id
            });
            await tools.Sacuvaj();


            await emailService.SendEmailAsync(model.Email!, "Registracija Uspešna", "Vaša registracija je uspešno završena.");

            return new ServiceResponse(true, "Nalog je uspešno kreiran!");
        }


    }

    // ovaj token (parametar metode) se dobija iz zaglavlja http zahteva
    public async Task<SesijaKorisnika> GetKorisnikaPoTokenu(string token)
    {
        var rezultat = await context.TokenInfo.FirstOrDefaultAsync(_=> _.AccessToken!.Equals(token));
        if (rezultat is null) return null!;

        var getKorisnikInfo = await context.KorisnickiNalozi.FirstOrDefaultAsync(_ => _.Id ==  rezultat.KorisnickiNalogId);
        if(getKorisnikInfo is null) return null!;

        if (rezultat.DatumIsteka < DateTime.Now) return null!;

        var getUloguKorisnika = await context.KorisnickeUloge.FirstOrDefaultAsync(_ => _.KorisnickiNalogId == getKorisnikInfo.Id);
        if(getUloguKorisnika is null) return null!;

        var uloga = await context.Uloge.FirstOrDefaultAsync(_ => _.Id == getUloguKorisnika.UlogaId);
        if(uloga is null) return null!;

        return new SesijaKorisnika()
        {
            Ime = getKorisnikInfo.Ime,
            Email = getKorisnikInfo.Email,
            Uloga = uloga.Naziv
        };
    }

    public async Task<PrijavaResponse> GetRefreshToken(PostRefreshTokenDTO model)
    {
        // dekodira iz Base64String-a u binarni niz
        var dekodiraniToken = WebEncoders.Base64UrlDecode(model.RefreshToken!);

        // dekodira iz binarnog niza u string
        string normalniToken = Encoding.UTF8.GetString(dekodiraniToken);

        var getToken = await context.TokenInfo.FirstOrDefaultAsync(_ => _.RefreshToken!.Equals(normalniToken));

        // korisnik pokusava da osvezi access token koriscenjem tokena koji je stariji od svog datuma isteka za 5 minuta ili koriscenjem refresh tokena koji ne postoji u bazi 
        if (getToken is null || getToken.DatumIsteka < DateTime.Now.AddMinutes(-5))
            return new PrijavaResponse(false, "Token nije validan ili je istekao!", null, null);

        // Generisanje novog tokena
        if (getToken.DatumIsteka < DateTime.Now.AddMinutes(5))
        {
            var (noviAccessToken, noviRefreshToken) = await tools.GenerisiTokene();
            await SacuvajTokenInfo(getToken.KorisnickiNalogId, noviAccessToken, noviRefreshToken);
            return new PrijavaResponse(true, "refresh-token-completed", noviAccessToken, noviRefreshToken);
        }

        return new PrijavaResponse(true, "Token je još uvek validan");
    }

    public async Task<bool> Odjava(string accessToken)
    {
        var tokenInfo = await context.TokenInfo.FirstOrDefaultAsync(t => t.AccessToken == accessToken);
        if (tokenInfo == null)
            return false;

        context.TokenInfo.Remove(tokenInfo);
        await tools.Sacuvaj();
        return true;
    }
}
