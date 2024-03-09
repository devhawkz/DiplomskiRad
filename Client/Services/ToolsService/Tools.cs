using System.Text.Json.Serialization;
using System.Text.Json;

namespace Client.Services.ToolsService;

internal class Tools : IToolsService
{

    //služi za konfiguriranje opcija za serializaciju i deserializaciju JSON podataka pomoću JsonSerializerOptions klase koja dolazi s .NET Core bibliotekom
    public JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            //Ova postavka omogućuje prisustvo zareza nakon posljednjeg elementa u JSON nizu. Uobičajeno je da standard JSON-a ne dozvoljava trailing zareze, ali ova postavka omogućuje njihovu upotrebu.
            AllowTrailingCommas = true,

            //Ova postavka čini da serializator ignoriše razliku između malih i velikih slova
            //.
            PropertyNameCaseInsensitive = true,

            // Ova postavka omogućuje definiranje pravila za pretvaranje imena svojstava iz C# objekata u JSON i obrnuto. U ovom slučaju, postavka je postavljena na JsonNamingPolicy.CamelCase, što znači da će imena svojstava biti formatirana u camelCase prilikom serializacije u JSON.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            // Ova postavka definira kako se tretiraju nepodudarajuća svojstva tijekom deserializacije. Ovdje je postavljena na JsonUnmappedMemberHandling.Skip, što znači da će sva nepodudarajuća svojstva biti preskočena tijekom deserializacije, a neće uzrokovati grešku.
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
        };
    }

    // koristi se za generisanje StringContent objekta koji se koristi za HTTP zahtjeve, posebno za slanje JSON podataka na server, objasnjenje koda u svesci -- konvertuje json objekat u StrinContent objekat(niz znakova)
    public StringContent GenerateStringContent(string serialiazedObj) => new(serialiazedObj, System.Text.Encoding.UTF8, "application/json");

    // ova metoda serijalizuje objekat bilo koje klase u Json objekat na osnovu prethodno definisane standardizacije
    public string SerializeObj<T>(T modelObject) => JsonSerializer.Serialize(modelObject, JsonOptions());

    // genericka metoda, vraca objekat tipa T, ova metoda omogućuje brzu i jednostavnu deserializaciju JSON stringa u objekt određenog tipa, koristeći zadane ili prilagođene opcije deserializacije
    public T DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonOptions())!;

    //  služi za deserializaciju JSON stringa u listu objekata određenog tipa T, ova metoda omogućuje brzu i jednostavnu deserializaciju JSON stringa u listu objekata određenog tipa, koristeći zadane ili prilagođene opcije deserializacije.
    public IList<T> DeserializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions())!;

    public ServiceResponse ProveriStatusKod(HttpResponseMessage odgovor)
    {
        if (!odgovor.IsSuccessStatusCode)
            return new ServiceResponse(false, "Došlo je do greške. Molimo pokušajte kasnije");
        else
            return new ServiceResponse(true, null);
    }

    // cita se odgovor iz response body-ja sa servera(sa api-ja) kao json string, sadrzi service response objekat u obliku Json objekta
    public async Task<string> CitajSadrzaj(HttpResponseMessage odgovor) => await odgovor.Content.ReadAsStringAsync();
}
