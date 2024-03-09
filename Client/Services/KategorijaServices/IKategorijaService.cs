namespace Client.Services.KategorijaServices;

public interface IKategorijaService
{
    // ovaj delegat koristimo kako bi obavestili aplikaciju ili komponentu o promeni apl. se nece reload-ovati
    Action? KategorijaAction { get; set; }
    Task<ServiceResponse> DodajKategoriju(Kategorija model);
    Task GetKategorije();
    List<Kategorija> SveKategorije { get; set; }
}
