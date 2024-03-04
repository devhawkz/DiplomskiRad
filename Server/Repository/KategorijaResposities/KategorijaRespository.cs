using Server.Data;
using Server.Repository.ProizvodRespositories;
using Server.Repository.Tools;
using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace Server.Repository.KategorijaResposities;

public class KategorijaRespository(DataContext context, ITools tools) : IKategorija
{
    // sluzi da se odabere koja vrsta metode ProveriImeUBazi treba da se izvrsi
    private readonly string vrsta = "kategorija";

    public async Task<ServiceResponse> DodajKategoriju(Kategorija model)
    {
        if (model is null) return new ServiceResponse(false, "Nije izabrana nijedna kategorija");

        // dekonstrukcija n-torke (tuple-a), ! je null - forgiving operator isto kao i dole
        var (flag, poruka) = await tools.ProveriImeUBazi(vrsta ,model.Naziv!);

        if(flag) 
        {
            context.Kategorije.Add(model);
            await tools.Sacuvaj();
            return new ServiceResponse(true, "Kategorija je uspešno dodata");
        }

        // vrednosti iz false grane metode ProveriImeUBazi tools klase
        return new ServiceResponse(flag, poruka);
    }

    public async Task<List<Kategorija>> GetKategorije() => await context.Kategorije.ToListAsync();        
    
}
