using SharedLibrary.DTOs;
using SharedLibrary.Responses;

namespace Server.Repository.KorisnikRespositories;

public interface IKorisnik
{
    Task<ServiceResponse> Registracija(KorisnikDTO korisnikDTO);
    Task<PrijavaResponse> Prijava(PrijavaDTO prijavaDTO);
}

