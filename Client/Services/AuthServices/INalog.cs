using SharedLibrary.DTOs;

namespace Client.Services.AuthServices
{
    public interface INalog
    {
        Task<ServiceResponse> Registracija(KorisnikDTO korisnikDTO);
        Task<PrijavaResponse> Prijava(PrijavaDTO prijavaDTO);
    }
}

