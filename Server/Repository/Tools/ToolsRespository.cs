using Server.Data;
using SharedLibrary.Responses;

namespace Server.Repository.Tools;

internal class ToolsRespository(DataContext context) : ITools
{
    //Proverava da li postoji proizvod sa tim imenom u bazi, ovde znak uzvicnika je null-forgiving operator, naglasavamo kompajleru da na navedenom mestu da tu ne ocekujemo null vrednost, i ako je tip koji koristimo nullable.
    public async Task<ServiceResponse> ProveriImeUBazi(string vrsta, string ime)
    {
        if(vrsta.Equals("proizvod"))
        {
            var proizvod = await context.Proizvodi.FirstOrDefaultAsync(p => p.Naziv.ToLower()!.Equals(ime.ToLower()));
            return proizvod is null ? new ServiceResponse(true, null) : new ServiceResponse(false, "Proizvod već postoji");
        }
   
        var kategorija = await context.Kategorije.FirstOrDefaultAsync(p => p.Naziv.ToLower()!.Equals(ime.ToLower()));
        return kategorija is null ? new ServiceResponse(true, null) : new ServiceResponse(false, "Kategorija već postoji");
        
    }

    // dobra praksa
    public async Task Sacuvaj() => await context.SaveChangesAsync();
}



