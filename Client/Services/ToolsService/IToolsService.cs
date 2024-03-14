using System.Text.Json;

namespace Client.Services.ToolsService;

public interface IToolsService
{
    public  JsonSerializerOptions JsonOptions();
    public  StringContent GenerateStringContent(string serialiazedObj);
    public string SerializeObj<T>(T modelObject);
    public T DeserializeJsonString<T>(string jsonString);
    public IList<T> DeserializeJsonStringList<T>(string jsonString);
    public ServiceResponse ProveriStatusKod(HttpResponseMessage odgovor);
    public Task<string> CitajSadrzaj(HttpResponseMessage odgovor);
    public string GetNovuLabelu(DateTime datumPostavljanja);
    public string GetOpis(string opis);

}
