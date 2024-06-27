using Server.Data;
using SharedLibrary.Responses;
using System.Security.Cryptography;

namespace Server.Repository.Tools;

internal class ToolsRespository(DataContext context) : ITools
{
    //Proverava da li postoji proizvod sa tim imenom u bazi, ovde znak uzvicnika je null-forgiving operator, naglasavamo kompajleru da na navedenom mestu da tu ne ocekujemo null vrednost, i ako je tip koji koristimo nullable.
    public async Task<ServiceResponse> ProveriImeUBazi(string vrsta, string ime)
    {
        if(vrsta.Equals("proizvod"))
        {
            var proizvod = await context.Proizvodi.FirstOrDefaultAsync(p => p.Naziv!.ToLower()!.Equals(ime.ToLower()));
            return proizvod is null ? new ServiceResponse(true, "Proizvod ne postoji") : new ServiceResponse(false, "Proizvod već postoji");
        }
   
        var kategorija = await context.Kategorije.FirstOrDefaultAsync(k => k.Naziv!.ToLower()!.Equals(ime.ToLower()));
        return kategorija is null ? new ServiceResponse(true, "Kategorija ne postoji") : new ServiceResponse(false, "Kategorija već postoji");
        
    }

   

    public async Task<(string AccessToken, string RefreshToken)> GenerisiTokene()
    {
        string accessToken = GenerisiToken(256);
        string refreshToken = GenerisiToken(64);

        // da bi se sprecila mogucnost da se isti token koristi za vise razlicitih korisnika, jer ce se ovi tokeni cuvati u bazi podataka
        while (!await VerifikujToken(accessToken))
            accessToken = GenerisiToken(256);

        while (!await VerifikujToken(refreshToken))
            refreshToken = GenerisiToken(64);

        return (accessToken, refreshToken);
    }

    private async Task<bool> VerifikujToken(string refreshToken = null!, string accessToken = null!)
    {
        TokenInfo tokenInfo = new();
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var getRefreshToken = await context.TokenInfo
              .FirstOrDefaultAsync(_ => _.RefreshToken!.Equals(refreshToken));
            return getRefreshToken is null ? true : false;
        }

        else
        {
            var getAccessToken = await context.TokenInfo
                .FirstOrDefaultAsync(_ => _.AccessToken!.Equals(accessToken));
            // pojednostavljana verzija povratne vrednosti u odnosu na prvu if granu , ista je logika
            return getAccessToken is null;
        }
    }

    private static string GenerisiToken(int brojBajtova) => Convert.ToBase64String(RandomNumberGenerator.GetBytes(brojBajtova));


    // dobra praksa
    public async Task Sacuvaj() => await context.SaveChangesAsync();
}



