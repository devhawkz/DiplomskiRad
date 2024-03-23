using SharedLibrary.DTOs;
using SharedLibrary.Responses;

namespace Server.Repository.KorisnikRespository;

public interface IKorisnickiNalog
{
    Task<ServiceResponse> Registracija(KorisnikDTO korisnikDTO);
    Task<PrijavaResponse> Prijava(PrijavaDTO prijavaDTO);
}
