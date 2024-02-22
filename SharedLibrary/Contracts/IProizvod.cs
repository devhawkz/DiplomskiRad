using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace SharedLibrary.Contracts;

public interface IProizvod
{
    Task<ServiceResponse> DodajProizvod(Proizvod proizvod);
    Task<List<Proizvod>> GetProizvode(bool preporuceniProizvod);
}
