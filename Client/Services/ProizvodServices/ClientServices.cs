using Client.Services.KategorijaServices;
using Client.Services.ToolsService;


namespace Client.Services.ProizvodServices;

public class ClientServices(HttpClient http, IToolsService toolsService) : IProizvodService, IKategorijaService
{
    // postavlja osnovnu rutu za sve metode
    private const string _proizvodBaseUrl = "api/proizvod";
    private const string _kategorijaBaseUrl = "api/kategorija";

    /* SVOJSTVA */
    public Action? KategorijaAction { get; set ; }
    public List<Kategorija> SveKategorije { get; set; }

    public Action? ProizvodAction { get; set ; }
    public List<Proizvod> SviProizvodi { get; set; }
    public List<Proizvod> PreporuceniProizvodi { get; set; }
    public List<Proizvod> ProizvodiIsteKategorije { get; set; }

    /* PROIZVODI */

    // Prilikom dodavanja novog proizvoda Post metodom, ako je uspesno dodat onda se poziva Get metoda koja vraca listu svih proizvoda iz baze ( AzuriranjeListeProizvoda(proizvod)),a ako vec postoji u bazi onda se ne poziva Get metoda
    public async Task<ServiceResponse> DodajProizvod(Proizvod proizvod)
    {
        // serijalizujemo C# objekat (proizvod koji korisnik zeli da sacuva) pomocu metode SerializeObj, zatim na osnovu tog serijalizovanog objekta generisemo StringContent objekat koji saljemo serveru
        var odgovor = await http.PostAsync(_proizvodBaseUrl, toolsService.GenerateStringContent(toolsService.SerializeObj(proizvod)));

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

        if (!preporuceniProizvod && SviProizvodi is null)
        { 
            SviProizvodi = await GetProizvodeSaApi(preporuceniProizvod);
            ProizvodAction?.Invoke();
            return;
        }

        // bolji nacin umesto 2 if-a
        else
        {
            if (preporuceniProizvod && PreporuceniProizvodi is null)
            {
                PreporuceniProizvodi = await GetProizvodeSaApi(preporuceniProizvod);
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

    // metoda koja daje nasumicni preporuceni proizvod
    public Proizvod GetNasumicniProizvod()
    {
        if (PreporuceniProizvodi is null) 
            return null!;

        Random randomNumbers = new();

        // najmanji id
        int min = PreporuceniProizvodi.Min(_ => _.Id);

        //najveci id, + 1 je kako bi se i maks id ukljucio u nasumicni odabir
        int max = PreporuceniProizvodi.Max(_ => _.Id) + 1;

        // random id izmedju ta 2, ukljucujuci i njih
        int randomId = randomNumbers.Next(min, max);

        // ako neki proizvod je sa id-jem koji ima istu vrednost kao nasumicniId vrati ga, ako ne postoji takav proizvod vrati null
        return PreporuceniProizvodi.FirstOrDefault(_ => _.Id == randomId)!;
    }
    
    /*KATEGORIJE*/

    // Prilikom dodavanja nove kategorije Post metodom, ako je uspesno dodata onda se poziva Get metoda koja vraca listu svih kategorija iz baze ( AzuriranjeListeProizvoda(proizvod)),a ako vec postoji u bazi onda se ne poziva Get metoda
    public async Task<ServiceResponse> DodajKategoriju(Kategorija model)
    {
        // serijalizujemo C# objekat (kategoriju koji korisnik zeli da sacuva) pomocu metode SerializeObj, zatim na osnovu tog serijalizovanog objekta generisemo StringContent objekat koji saljemo serveru
        var odgovor = await http.PostAsync(_kategorijaBaseUrl, toolsService.GenerateStringContent(toolsService.SerializeObj(model)));

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
            SveKategorije =  await GetKategorijeSaApi();
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
