using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace Client.Services.ProizvodServices;

public interface IProizvodService
{
    // ovaj delegat koristimo kako bi obavestili aplikaciju ili komponentu o promeni, apl. se nece reload-ovati
    Action? ProizvodAction { get; set; }
    Task<ServiceResponse> DodajProizvod(Proizvod proizvod);
    Task GetProizvode(bool preporuceniProizvod);
    List<Proizvod> SviProizvodi { get; set; }
    List<Proizvod> PreporuceniProizvodi { get; set; }
    Task GetProizvodeIsteKategorije(int kategorijaId);
    List<Proizvod> ProizvodiIsteKategorije { get; set; }
    bool IsVisible { get; set; }
    Proizvod GetNasumicniProizvod();
    Task<ServiceResponse> ObrisiProizvod(int id);
    Task<ServiceResponse> AzurirajProizvod(Proizvod proizvod);
    Task<Proizvod> GetProizvodPoId(int id);
}
