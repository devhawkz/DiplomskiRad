using SharedLibrary.DTOs;

namespace Client.Services.KorisnikServices;

public interface IKorisnikService
{
    Task<ServiceResponse> Registracija(KorisnikDTO model);
    Task<PrijavaResponse> Prijava(PrijavaDTO model);
}
