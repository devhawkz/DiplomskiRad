using Client.Authentication;
using Client.Services.ToolsService;
using static System.Net.WebRequestMethods;

namespace Client.Services.KategorijaServices;

public class KategorijaService(HttpClient http, IToolsService toolsService, AuthenticationService authService) : IKategorijaService
{
    private const string _kategorijaBaseUrl = "api/kategorija";


    /*Svojstva*/
    public Action? KategorijaAction { get; set; }
    public List<Kategorija> SveKategorije { get; set; }

    // Prilikom dodavanja nove kategorije Post metodom, ako je uspesno dodata onda se poziva Get metoda koja vraca listu svih kategorija iz baze ( AzuriranjeListeProizvoda(proizvod)),a ako vec postoji u bazi onda se ne poziva Get metoda
    public async Task<ServiceResponse> DodajKategoriju(Kategorija model)
    {
        await authService.GetDetaljeKorisnika();
        var privateHttp = await authService.AddZaglavljeToHttpClient();

        // serijalizujemo C# objekat (kategoriju koji korisnik zeli da sacuva) pomocu metode SerializeObj, zatim na osnovu tog serijalizovanog objekta generisemo StringContent objekat koji saljemo serveru
        var odgovor = await privateHttp.PostAsync(_kategorijaBaseUrl, toolsService.GenerateStringContent(toolsService.SerializeObj(model)));

        // vraca true ako je status code u opsegu od 200-299
        var rezultat = toolsService.ProveriStatusKod(odgovor);

        // vraca true granu ProveriStatusKod metode tools klase
        if (!rezultat.Flag) return rezultat;

        // cita se odgovor iz response body-ja sa servera(sa api-ja) kao json string, sadrzi service response objekat u obliku Json objekta
        var apiOdgovor = await toolsService.CitajSadrzaj(odgovor);

        // vraca deserijalizovani json objekat kao odgovor (vraca service response, true ili false)
        var podaci = toolsService.DeserializeJsonString<ServiceResponse>(apiOdgovor);

        // u slucaju da se kategorija koju korisnik zeli da doda vec postoji u bazi
        if (!podaci.Flag) return podaci;

        // kako bi se dodao nova kategorija u listu kateogrija koja se ne nalazi na serveru zbog boljih perfomansi, vec se nalazi u memoriji pregledaca korisnika, poziva se servis sa servera koji salje sve kategorije iz baze, api poziv
        await AzuriranjeListeKategorija();
        return podaci;
    }

    // metoda koja azurira listu proizvoda koja se nalazi u memoriji pregledaca, nakon sto se doda novi proizvod
    private async Task AzuriranjeListeKategorija()
    {
        SveKategorije = null!;
        await GetKategorije();
    }

    // glavna get metoda
    public async Task GetKategorije()
    {
        //zahtevamo od servera sve kategorije ako je lista prazna
        if (SveKategorije is null)
        {
            SveKategorije = await GetKategorijeSaApi();
            KategorijaAction?.Invoke();
        }
    }

    // metoda koja uzima kategorije sa servera
    private async Task<List<Kategorija>> GetKategorijeSaApi()
    {
        var odgovor = await http.GetAsync(_kategorijaBaseUrl);

        // isto radi kao u metodi GetProizvodi
        var provera = toolsService.ProveriStatusKod(odgovor);

        // ako je flag false prekida se izvrsenje metode i ne vraca se nista 
        if (!provera.Flag) return null!;

        // rezultat promenljiva sadrzi json string
        var rezultat = await toolsService.CitajSadrzaj(odgovor);

        return (List<Kategorija>?)toolsService.DeserializeJsonStringList<Kategorija>(rezultat)!;
    }

}
