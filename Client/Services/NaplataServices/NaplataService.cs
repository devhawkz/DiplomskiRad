
using Client.Services.ToolsService;

namespace Client.Services.NaplataServices;

public class NaplataService(HttpClient http, IToolsService tools) : INaplata
{
    public async Task<string> Racun(List<Narudzbina> listaStavki)
    {
        var odgovor = await http.PostAsync("api/naplata/racun", tools.GenerateStringContent(tools.SerializeObj(listaStavki)));

        var url = await odgovor.Content.ReadAsStringAsync();
        return url;
    }
}
