using Client.Authentication;
using Client.Services.ToolsService;
using SharedLibrary.Models;


namespace Client.Services.ProizvodServices;

public class ProizvodService(HttpClient http, IToolsService toolsService, AuthenticationService authService) : IProizvodService
{
    // postavlja osnovnu rutu za sve metode
    private const string _proizvodBaseUrl = "api/proizvod";
   
    /*Svojstva*/

    public Action? ProizvodAction { get; set ; }
    public List<Proizvod> SviProizvodi { get; set; }
    public List<Proizvod> PreporuceniProizvodi { get; set; }
    public List<Proizvod> ProizvodiIsteKategorije { get; set; }
    
    public bool IsVisible { get; set; }

    private static readonly Random random = new();

    /* PROIZVODI */

    // Prilikom dodavanja novog proizvoda Post metodom, ako je uspesno dodat onda se poziva Get metoda koja vraca listu svih proizvoda iz baze ( AzuriranjeListeProizvoda(proizvod)),a ako vec postoji u bazi onda se ne poziva Get metoda
    public async Task<ServiceResponse> DodajProizvod(Proizvod proizvod)
    {
        await authService.GetDetaljeKorisnika();
        var privateHttp = await authService.AddZaglavljeToHttpClient();

        // serijalizujemo C# objekat (proizvod koji korisnik zeli da sacuva) pomocu metode SerializeObj, zatim na osnovu tog serijalizovanog objekta generisemo StringContent objekat koji saljemo serveru
        var odgovor = await privateHttp.PostAsync(_proizvodBaseUrl, toolsService.GenerateStringContent(toolsService.SerializeObj(proizvod)));

        // vraca true ako je status code u opsegu od 200-299
        var rezultat = toolsService.ProveriStatusKod(odgovor);

        // cita se odgovor iz response body-ja sa servera(sa api-ja) kao json string, sadrzi service response objekat u obliku Json objekta
        var apiOdgovor = await toolsService.CitajSadrzaj(odgovor);

        // vraca deserijalizovani json objekat kao odgovor (vraca service response, true ili false)
        var podaci =  toolsService.DeserializeJsonString<ServiceResponse>(apiOdgovor);

         // kako bi se dodao novi proizvod u listu proizvoda koja se ne nalazi na serveru zbog boljih perfomansi, vec se nalazi u memoriji pregledaca korisnika, poziva se servis sa servera koji salje sve prozivode iz baze, api poziv
        if (!podaci.Flag) return podaci;

        // kako bi se dodao novi proizvod u listu proizvoda koja se ne nalazi na serveru zbog boljih perfomansi, vec se nalazi u memoriji pregledaca korisnika
        await AzuriranjeListeProizvoda(proizvod);
        return podaci;
    }

    // metoda koja azurira listu proizvoda koja se nalazi u memoriji pregledaca, nakon sto se doda novi proizvod
    private async Task AzuriranjeListeProizvoda(Proizvod proizvod)
    {
        PreporuceniProizvodi = null!;
        SviProizvodi = null!;

        await GetProizvode(proizvod.PreporucenProizvod);
        await GetProizvode(!proizvod.PreporucenProizvod);
    }

    // glavna get metoda
    public async Task GetProizvode(bool preporuceniProizvod)
    {

        if (!preporuceniProizvod)
        {
            IsVisible = true; // pokrece se screen loader
            SviProizvodi = await GetProizvodeSaApi(preporuceniProizvod);
            IsVisible = false; // gasi se screen loader
            ProizvodAction?.Invoke(); 
            return;
        }

        // bolji nacin umesto 2 if-a
        else
        {
            if (preporuceniProizvod && PreporuceniProizvodi is null)
            {
                IsVisible = true;
                PreporuceniProizvodi = await GetProizvodeSaApi(preporuceniProizvod);
                IsVisible = false;
                ProizvodAction?.Invoke();
                return;
            }
        }
              
    }

    // metoda koja uzima proizvode sa servera
    private async Task<List<Proizvod>> GetProizvodeSaApi(bool preporuceniProizvod)
    {
        //zahtevamo od servera sve proizvode ili ako preporuceno nije null onda samo preporucene proizvode
        var odgovor = await http.GetAsync($"{_proizvodBaseUrl}?preporuceniProizvodi={preporuceniProizvod}");

        // isto radi kao u metodi GetKategorije
        var (flag, _) = toolsService.ProveriStatusKod(odgovor);

        // ako je flag false prekida se izvrsenje metode i ne vraca se nista
        if (!flag) return null;

        // rezultat promenljiva sadrzi json string
        var rezultat = await toolsService.CitajSadrzaj(odgovor);

        // ? - null conditional operator, ! - null forgiving operator, ovde se vraca c# objekat koji je tipa List<Proizvod> koji je deserijalizovan iz JsonStringa koji se cuva u promenljivoj rezultat
        return (List<Proizvod>?)toolsService.DeserializeJsonStringList<Proizvod>(rezultat)!;

    }

    // metoda koja vraca sve proizvode iste kategorije
    public async Task GetProizvodeIsteKategorije(int kategorijaId)
    {
        bool preporuceno = false;
        await GetProizvode(preporuceno);

        // iz liste svih proizvoda koji se nalaze vec u memoriji pregledaca uzimaju se samo oni sa datom kategorijom
        ProizvodiIsteKategorije = SviProizvodi.Where(_ => _.KategorijaId == kategorijaId).ToList();
        ProizvodAction?.Invoke();
    }

    public Proizvod GetNasumicniProizvod()
    {
        if (PreporuceniProizvodi is null || PreporuceniProizvodi.Count == 0)
            return null!;

        int randomId = random.Next(0, PreporuceniProizvodi.Count);

        return PreporuceniProizvodi[randomId]!;
    }

    public async Task<ServiceResponse> ObrisiProizvod(int id)
    {
        await authService.GetDetaljeKorisnika();
        var privateHttp = await authService.AddZaglavljeToHttpClient();

        var odgovor = await privateHttp.DeleteAsync($"{_proizvodBaseUrl}/{id}");
        var rezultat = toolsService.ProveriStatusKod(odgovor);

       
        var apiOdgovor = await toolsService.CitajSadrzaj(odgovor);

        var podaci = toolsService.DeserializeJsonString<ServiceResponse>(apiOdgovor);

        if (!podaci.Flag) return podaci;

        
        SviProizvodi.Clear();
        await GetProizvode(false);
        return podaci;
    }
}
