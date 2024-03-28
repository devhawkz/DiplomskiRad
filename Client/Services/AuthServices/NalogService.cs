using Client.Services.ToolsService;
using SharedLibrary.DTOs;
using System.Net.Http.Json;

namespace Client.Services.AuthServices;

public class NalogService(HttpClient client, IToolsService toolsService) : INalog
{
    private const string _baseUrl = "api/Nalog";

    public async Task<PrijavaResponse> Prijava(PrijavaDTO prijavaDTO)
    {
        var zahtev = await client.PostAsJsonAsync($"{_baseUrl}/prijava", prijavaDTO);

        // citanje odgovora
        if(!zahtev.IsSuccessStatusCode) 
            return new PrijavaResponse(false, null, "Došlo je do greške. Pokušajte ponovo kasnije...");
        
        var response = await zahtev.Content.ReadFromJsonAsync<PrijavaResponse>();
        return response!;
        
    }

    public Task<ServiceResponse> Registracija(KorisnikDTO korisnikDTO)
    {
        throw new NotImplementedException();
    }
}
