using SharedLibrary.DTOs;
using SharedLibrary.Responses;

namespace Server.Repository.KorisnikRespositories;

public interface IKorisnickiNalog
{
    Task<ServiceResponse> RegistracijaKorisnika(KorisnikDTO model);
    Task<ServiceResponse> RegistracijaAdmina(KorisnikDTO model);
    Task<PrijavaResponse> Prijava(PrijavaDTO model);
    Task<SesijaKorisnika> GetKorisnikaPoTokenu(string token);
    Task<PrijavaResponse> GetRefreshToken(PostRefreshTokenDTO model);
    Task<bool> Odjava(string accessToken);
}
