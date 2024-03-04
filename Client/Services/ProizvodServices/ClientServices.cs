using static Client.Services.ProizvodServices.Tools;

namespace Client.Services.ProizvodServices;

public class ClientServices(HttpClient http) : IProizvodService
{
    // postavlja osnovnu rutu za sve metode
    private const string _baseUrl = "api/proizvod";

    public async Task<ServiceResponse> DodajProizvod(Proizvod proizvod)
    {
        // serijalizujemo C# objekat (proizvod koji korisnik zeli da sacuva) pomocu metode SerializeObj, zatim na osnovu tog serijalizovanog objekta generisemo StringContent objekat koji saljemo serveru
        var odgovor = await http.PostAsync(_baseUrl, GenerateStringContent(SerializeObj(proizvod)));

        // vraca true ako je status code u opsegu od 200-299
        if (!odgovor.IsSuccessStatusCode)
            return new ServiceResponse(false, "Došlo je do greške. Molimo pokušajte kasnije");

        // cita se odgovor iz response body-ja sa servera(sa api-ja) kao json string, sadrzi service response objekat u obliku Json objekta
        var apiOdgovor = await odgovor.Content.ReadAsStringAsync();

        // vraca deserijalizovani json objekat kao odgovor (vraca service response, true ili false)
        return DeserializeJsonString<ServiceResponse>(apiOdgovor);

    }

    public async Task<List<Proizvod>> GetProizvode(bool preporuceniProizvod)
    {
        //zahtevamo od servera sve proizvode ili ako preporuceno nije null onda samo preporucene proizvode
        var odgovor = await http.GetAsync($"{_baseUrl}?preporuceniProizvodi={preporuceniProizvod}");

        if (!odgovor.IsSuccessStatusCode)
            return null;

        var rezultat = await odgovor.Content.ReadAsStringAsync();

        // vraca listu proizvoda, [.. ] umesto .ToList();
        return [.. DeserializeJsonStringList<Proizvod>(rezultat)];
    }
}
