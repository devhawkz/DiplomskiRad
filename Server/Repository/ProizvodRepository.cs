using Server.Data;
using SharedLibrary.Contracts;
using SharedLibrary.Models;
using SharedLibrary.Responses;
using System.Transactions;

namespace Server.Repository;

public class ProizvodRepository(DataContext context) : IProizvod
{
   

    public async Task<ServiceResponse> DodajProizvod(Proizvod proizvod)
    {
        if (proizvod is null) return new ServiceResponse(false, "Nije izabran nijedan proizvod");

        // dekonstrukcija n-torke (tuple-a), ! je null - forgiving operator isto kao i dole
        var (flag, poruka) = await ProveriImeUBazi(proizvod.Naziv!);

        // ako je true
        if(flag)
        {
            context.Proizvodi.Add(proizvod);
            await Sacuvaj();
            return new ServiceResponse(true, "Proizvod je sačuvan");
        }

        // vrednosti iz false grane metode ProveriImeUBazi
        return new ServiceResponse(flag, poruka);
    }

    public async Task<List<Proizvod>> GetProizvode(bool preporuceniProizvod)
    {
        // _ je placeholder, nema potrebe za eksplicitnim imenovanjem promenljive ili parametra
        if(preporuceniProizvod)
            return await context.Proizvodi.Where(_=> _.PreporucenProizvod).ToListAsync();
        else
            return await context.Proizvodi.ToListAsync();
    }


    //Proverava da li postoji proizvod sa tim imenom u bazi, ovde znak uzvicnika je null-forgiving operator, naglasavamo kompajleru da na navedenom mestu da tu ne ocekujemo null vrednost, i ako je tip koji koristimo nullable.
    private async Task<ServiceResponse> ProveriImeUBazi(string ime)
    {
        var proizvod = await context.Proizvodi.FirstOrDefaultAsync(p => p.Naziv.ToLower()!.Equals(ime.ToLower()));
        return proizvod is null ? new ServiceResponse(true, null) : new ServiceResponse(false, "Proizvod već postoji");

    }

    // dobra praksa
    private async Task Sacuvaj() => await context.SaveChangesAsync();
}
