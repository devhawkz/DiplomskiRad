using Blazored.LocalStorage;
using Client.Services.ProizvodServices;
using Client.Services.ToolsService;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Runtime.InteropServices;

namespace Client.Services.KorpaServices;

public class KorpaService(ILocalStorageService lokalnoSkladiste, IToolsService tools, IProizvodService proizvodService) : IKorpa
{
    public Action KorpaAction { get; set; }
    public int KorpaCount { get; set; }
    public bool isKorpaLoaderVidljiv { get; set; }

    public async Task<ServiceResponse> DodajUKorpu(Proizvod model, int updateKolicinu = 1)
    {
        string poruka = string.Empty;
        var mojaKorpa = new List<StorageKorpa>();
        var getKorpuIzSkladista = await GetKorpuIzLokalnogSkladista();

        if(!string.IsNullOrEmpty(getKorpuIzSkladista))
        {
            mojaKorpa = (List<StorageKorpa>)tools.DeserializeJsonStringList<StorageKorpa>(getKorpuIzSkladista);
            var proveriDaLiJeProizvodVecDodat = mojaKorpa.FirstOrDefault(_ => _.ProizvodId == model.Id);

            if(proveriDaLiJeProizvodVecDodat is null)
            {
                mojaKorpa.Add(new StorageKorpa() { ProizvodId = model.Id, Kolicina = 1 });
                poruka = "Proizvod je dodat u korpu";
            }

            else
            {
                var azuriranProizvod = new StorageKorpa() { ProizvodId = model.Id, Kolicina = updateKolicinu };

                mojaKorpa.Remove(proveriDaLiJeProizvodVecDodat!);
                mojaKorpa.Add(azuriranProizvod);
                poruka = "Proizvod je ažuriran";        
            }
        }

        else
        {
            mojaKorpa.Add(new StorageKorpa() { ProizvodId = model.Id, Kolicina = 1 });
            poruka = "Proizvod je dodat u korpu";
        }

        await ObrisiKorpiIzLokalnogSkladista();
        await SetKorpuULokalnoSkladiste(tools.SerializeObj(mojaKorpa));
        await GetKorpaCount();
        return new ServiceResponse(true, poruka);
        
    }

    public async Task GetKorpaCount()
    {
        string korpaString = await GetKorpuIzLokalnogSkladista();
        if (string.IsNullOrEmpty(korpaString))
            KorpaCount = 0;
        else 
            KorpaCount = tools.DeserializeJsonStringList<StorageKorpa>(korpaString).Count();

        //KorpaAction.Invoke();
    }

    public async Task<List<Narudzbina>> MojeNarudzbine()
    {
        isKorpaLoaderVidljiv = true;
        var lista = new List<Narudzbina>();
        string mojaKorpaString = await GetKorpuIzLokalnogSkladista();

        if (string.IsNullOrEmpty(mojaKorpaString))
        {
            isKorpaLoaderVidljiv = false;
            return null;
        }

        var mojaKorpaLista = tools.DeserializeJsonStringList<StorageKorpa>(mojaKorpaString);

        // kako bi se ucitali proizvodi
        await proizvodService.GetProizvode(false);

        foreach(var stavkaIzKorpe in mojaKorpaLista)
        {
            var proizvod = proizvodService.SviProizvodi.FirstOrDefault(_ => _.Id == stavkaIzKorpe.ProizvodId);

            lista.Add(new Narudzbina()
            { 
                Id = proizvod!.Id,
                Naziv = proizvod.Naziv,
                Kolicina = stavkaIzKorpe.Kolicina,
                Cena = proizvod.Cena,
                Slika = proizvod.Base64Img
            });
        }
        isKorpaLoaderVidljiv = false;
        await GetKorpaCount();
        return lista;
    }

    public async Task<ServiceResponse> ObrisiProizvodIzKorpe(Narudzbina korpa)
    {
        var mojaKorpaLista = tools.DeserializeJsonStringList<StorageKorpa>(await GetKorpuIzLokalnogSkladista());

        if (mojaKorpaLista is null) return new ServiceResponse(false, "Nijedan proizvod nije pronađen!");
        
        mojaKorpaLista.Remove(mojaKorpaLista.FirstOrDefault(_ => _.ProizvodId == korpa.Id)!);
        await ObrisiKorpiIzLokalnogSkladista();
        await SetKorpuULokalnoSkladiste(tools.SerializeObj(mojaKorpaLista));
        await GetKorpaCount();
        return new ServiceResponse(true, "Proizvod uspešno obrisan");
    }


     /*Pomocne metode */

    private async Task<string> GetKorpuIzLokalnogSkladista() => await lokalnoSkladiste.GetItemAsStringAsync("korpa");
    private async Task SetKorpuULokalnoSkladiste(string korpa) => await lokalnoSkladiste.SetItemAsStringAsync("korpa", korpa);
    private async Task ObrisiKorpiIzLokalnogSkladista() => await lokalnoSkladiste.RemoveItemAsync("korpa");
}
