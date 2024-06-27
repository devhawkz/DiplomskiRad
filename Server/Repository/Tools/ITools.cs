using SharedLibrary.Responses;

namespace Server.Repository.Tools;

public interface ITools
{
    Task<ServiceResponse> ProveriImeUBazi(string vrsta, string ime);
    Task<(string AccessToken, string RefreshToken)> GenerisiTokene();
    Task Sacuvaj();
}
