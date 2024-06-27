using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace Server.Repository.KategorijaResposities;

public interface IKategorija
{
    Task<ServiceResponse> DodajKategoriju(Kategorija model);
    Task<List<Kategorija>> GetKategorije();
    Task<ServiceResponse> ObrisiKategoriju(string nazivKategorije);
}
