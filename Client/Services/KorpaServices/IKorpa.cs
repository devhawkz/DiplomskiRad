namespace Client.Services.KorpaServices;

public interface IKorpa
{
    public Action KorpaAction { get; set; }
    public int KorpaCount { get; set; }
    Task GetKorpaCount();
    Task<ServiceResponse> DodajUKorpu(Proizvod model, int updateKolicinu = 1);
    Task<List<Narudzbina>> MojeNarudzbine();
    Task<ServiceResponse> ObrisiProizvodIzKorpe(Narudzbina korpa);
    bool isKorpaLoaderVidljiv {  get; set; }
}
