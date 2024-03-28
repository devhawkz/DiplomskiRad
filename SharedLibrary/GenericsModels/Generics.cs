
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace SharedLibrary.GenericsModels;

public static class Generics
{
    //Ova metoda SetClaimsPrincipal prima SesijaKorisnika objekt (vjerovatno predstavlja sesiju korisnika) i stvara ClaimsPrincipal objekt koji se koristi za autentikaciju korisnika
    public static ClaimsPrincipal SetClaimsPrincipal(SesijaKorisnika model)
    {
        //Stvara se nova instanca ClaimsIdentity klase i dodaje se u ClaimsPrincipal objekat
        return new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>
            {
                // tvrdnja NameIdentifier se postavlja na ID korisnika, odnosno tvrdnja NameIdentifier ima vrednost Id korisnika
                new (ClaimTypes.NameIdentifier, model.Id!),
                new (ClaimTypes.Name, model.Ime!),
                new (ClaimTypes.Email, model.Email!),
                new (ClaimTypes.Role, model.Role!),
            }, "JwtAuth"));
    }

    // prima JWT token kao argument i iz njega izvlaci info o sesiji korisnika
    public static SesijaKorisnika GetClaimsFromToken(string jwtToken)
    {
        //Stvara se novi JwtSecurityTokenHandler koji se koristi za rukovanje JWT tokenima
        var handler = new JwtSecurityTokenHandler();
        
        //Poziva se metoda ReadJwtToken(jwtToken) kako bi se pročitao JWT token i dobio JwtSecurityToken objekt
        var token = handler.ReadJwtToken(jwtToken);

        //Iz JwtSecurityToken objekta se dobivaju tvrdnje (claims) koje sadrži taj token.
        var claims = token.Claims;

        // Iz tvrdnji se izvlače potrebne informacije kao sto su id, ime, email, rola itd.
        string Id = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        string Ime = claims.First(c => c.Type == ClaimTypes.Name).Value;
        string Email = claims.First(c => c.Type == ClaimTypes.Email).Value;
        string Role = claims.First(c => c.Type == ClaimTypes.Role).Value;

        return new SesijaKorisnika(Id, Ime, Email, Role);
    }
}
