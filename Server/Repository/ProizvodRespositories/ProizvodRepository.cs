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
        // _ je placeholder, nema potrebe za eksplicitnim imenovanjem promenljive ili parametra
        if (preporuceniProizvod)
            return await context.Proizvodi.Where(p => p.PreporucenProizvod).ToListAsync();
        else
            return await context.Proizvodi.ToListAsync();
    }

}
