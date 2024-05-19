using SharedLibrary.DTOs;

namespace Client.Services.KorisnikServices;

public interface IKorisnikService
{
    Task<ServiceResponse> RegistracijaKorisnika(KorisnikDTO model);
    Task<ServiceResponse> RegistracijaAdmina(KorisnikDTO model);
    Task<PrijavaResponse> Prijava(PrijavaDTO model);
}
