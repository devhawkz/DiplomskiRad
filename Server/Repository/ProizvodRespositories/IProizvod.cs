using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace Server.Repository.ProizvodRespositories;

public interface IProizvod
{
    Task<ServiceResponse> DodajProizvod(Proizvod proizvod);
    Task<List<Proizvod>> GetProizvode(bool preporuceniProizvod);
    Task<ServiceResponse> ObrisiProizvod(int proizvodId);
}
