using Server.Data;
using SharedLibrary.Models;
using SharedLibrary.Responses;
using Server.Repository.Tools;

namespace Server.Repository.ProizvodRespositories;

public class ProizvodRepository(DataContext context, ITools tools) : IProizvod
{
    // sluzi da se odabere koja verzija metode ProveriImeUBazi treba da se izvrsi
    private const string vrsta = "proizvod";

    public async Task<ServiceResponse> DodajProizvod(Proizvod proizvod)
    {
        if (proizvod is null) return new ServiceResponse(false, "Nije izabran nijedan proizvod");

        // dekonstrukcija n-torke (tuple-a), ! je null - forgiving operator isto kao i dole
        var (flag, poruka) = await tools.ProveriImeUBazi(vrsta, proizvod.Naziv!);

        // ako je true
        if (flag)
        {
            context.Proizvodi.Add(proizvod);
            await tools.Sacuvaj();
            return new ServiceResponse(true, "Proizvod je sačuvan");
        }

        // vrednosti iz false grane metode ProveriImeUBazi
        return new ServiceResponse(flag, poruka);
    }

    public async Task<List<Proizvod>> GetProizvode(bool preporuceniProizvod)
    {
        //sa IQueryable upiti se formiraju i optimizuju pre nego sto se izvrse nad bazom, izvrsavaju se tek onda kada se podaci zatraze 
        IQueryable<Proizvod> upit = context.Proizvodi
            .Include(p => p.Kategorija)
            .AsNoTracking();

        // _ je placeholder, nema potrebe za eksplicitnim imenovanjem promenljive ili parametra
        if (preporuceniProizvod)
        {
            upit = upit.Where(p => p.PreporucenProizvod);
            return upit.ToList();
        }

        else
            return upit.ToList();
    }

    public async Task<ServiceResponse> ObrisiProizvod(int id)
    {
        var proizvod = await context.Proizvodi.FindAsync(id);

        if (proizvod is null)
            return new ServiceResponse(false, "Proizvod sa tim id ne postoji u bazi");
        
        context.Proizvodi.Remove(proizvod);
        await tools.Sacuvaj();
        return new ServiceResponse(true, "Proizvod je uspešno obrisan");

    }

    public async Task<ServiceResponse> AzurirajProizvod(Proizvod model)
    {
        var proizvod = await context.Proizvodi.FindAsync(model.Id);

        if(proizvod is null)
            return new ServiceResponse(false, "Proizvod sa tim id ne postoji u bazi");

        proizvod.Id = model.Id;
        proizvod.Naziv = model.Naziv;
        proizvod.PreporucenProizvod = model.PreporucenProizvod;
        proizvod.Kolicina = model.Kolicina;
        proizvod.DatumPostavljanja = model.DatumPostavljanja;
        proizvod.Opis = model.Opis;
        proizvod.Base64Img = model.Base64Img;
        proizvod.Cena = model.Cena;
        proizvod.KategorijaId = model.KategorijaId;

        await tools.Sacuvaj();

        return new ServiceResponse(true, "Proizvod je ažuriran");
    }
}
