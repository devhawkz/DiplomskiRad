using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace Client.Services.ProizvodServices;

public interface IProizvodService
{
    Task<ServiceResponse> DodajProizvod(Proizvod proizvod);
    Task<List<Proizvod>> GetProizvode(bool preporuceniProizvod);
}
