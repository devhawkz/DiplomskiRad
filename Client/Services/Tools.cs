using System.Text.Json.Serialization;
using System.Text.Json;

namespace Client.Services;

internal static class Tools
{

    //služi za konfiguriranje opcija za serializaciju i deserializaciju JSON podataka pomoću JsonSerializerOptions klase koja dolazi s .NET Core bibliotekom
    internal static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            //Ova postavka omogućuje prisustvo zareza nakon posljednjeg elementa u JSON nizu. Uobičajeno je da standard JSON-a ne dozvoljava trailing zareze, ali ova postavka omogućuje njihovu upotrebu.
            AllowTrailingCommas = true,

            //Ova postavka omogućuje prisustvo zareza nakon posljednjeg elementa u JSON nizu. Uobičajeno je da standard JSON-a ne dozvoljava trailing zareze, ali ova postavka omogućuje njihovu upotrebu.
            PropertyNameCaseInsensitive = true,

            // Ova postavka omogućuje definiranje pravila za pretvaranje imena svojstava iz C# objekata u JSON i obrnuto. U ovom slučaju, postavka je postavljena na JsonNamingPolicy.CamelCase, što znači da će imena svojstava biti formatirana u camelCase prilikom serializacije u JSON.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            // Ova postavka definira kako se tretiraju nepodudarajuća svojstva tijekom deserializacije. Ovdje je postavljena na JsonUnmappedMemberHandling.Skip, što znači da će sva nepodudarajuća svojstva biti preskočena tijekom deserializacije, a neće uzrokovati grešku.
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
        };
    }

    // koristi se za generisanje StringContent objekta koji se koristi za HTTP zahtjeve, posebno za slanje JSON podataka na server, objasnjenje koda u svesci
    internal static StringContent GenerateStringContent(string serializedObj) => new(serializedObj, System.Text.Encoding.UTF8, "application/json");

    // ova metoda serijalizuje objekat bilo koje klase u Json objekat na osnovu prethodno definisane standardizacije
    internal static string SerializeObj(object modelObj) => JsonSerializer.Serialize(modelObj, JsonOptions());

    // genericka metoda, vraca objekat tipa T, ova metoda omogućuje brzu i jednostavnu deserializaciju JSON stringa u objekt određenog tipa, koristeći zadane ili prilagođene opcije deserializacije
    internal static T DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonOptions());

    //  služi za deserializaciju JSON stringa u listu objekata određenog tipa T, ova metoda omogućuje brzu i jednostavnu deserializaciju JSON stringa u listu objekata određenog tipa, koristeći zadane ili prilagođene opcije deserializacije.
    internal static IList<T> DeserializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions());

}
