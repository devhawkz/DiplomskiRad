using Client.Services.ToolsService;
using SharedLibrary.DTOs;
using System.Runtime.InteropServices;

namespace Client.Services.KorisnikServices;

public class KorisnikService(HttpClient http, IToolsService tools) : IKorisnikService
{
    private const string _baseUrl = "api/korisnik";

    public async Task<PrijavaResponse> Prijava(PrijavaDTO model)
    {
        var response = await http.PostAsync($"{_baseUrl}/prijava", tools.GenerateStringContent(tools.SerializeObj(model)));

        if (!response.IsSuccessStatusCode)
            return new PrijavaResponse(false, "Došlo je do greške. Pokušajte ponovo...", null!, null!);

        var apiResponse = await ReadContent(response);
        return tools.DeserializeJsonString<PrijavaResponse>(apiResponse);
    }

    public async Task<ServiceResponse> RegistracijaKorisnika(KorisnikDTO model)
    {
        var response = await http.PostAsync($"{_baseUrl}/registracija-korisnika", tools.GenerateStringContent(tools.SerializeObj(model)));
        var rezultat = CheckResponse(response);

        if(!rezultat.Flag)
            return rezultat;

        // prevodi odgovor iz HttpResponseMessage u string(i dalje je JSON string)
        var apiResponse = await ReadContent(response);

        // prevodi JSON string u objekat tipa ServiceResponse
        return tools.DeserializeJsonString<ServiceResponse>(apiResponse);
    }

    public async Task<ServiceResponse> RegistracijaAdmina(KorisnikDTO model)
    {
        var response = await http.PostAsync($"{_baseUrl}/registracija-admina", tools.GenerateStringContent(tools.SerializeObj(model)));
        var rezultat = CheckResponse(response);

        if (!rezultat.Flag)
            return rezultat;

        // prevodi odgovor iz HttpResponseMessage u string(i dalje je JSON string)
        var apiResponse = await ReadContent(response);

        // prevodi JSON string u objekat tipa ServiceResponse
        return tools.DeserializeJsonString<ServiceResponse>(apiResponse);
    }

    private async Task<string> ReadContent(HttpResponseMessage response) => await response.Content.ReadAsStringAsync();
   
    private static ServiceResponse CheckResponse(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return new ServiceResponse(false, "Došlo je do greške. Pokušajte ponovo...");
        else
            return new ServiceResponse(true, null!);
    }

}
