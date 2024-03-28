using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Runtime.InteropServices;
using SharedLibrary.GenericsModels;
using System.Security.Claims;

namespace Client.Authentication;

public class CustomAuthenticationStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    //Ovaj kod kreira anonimnog korisnika (ClaimsPrincipal) koji nema nikakve tvrdnje (claims), stvaraju se 2 instance klase ClaimsPrincipal,Ovaj kod kreira novu instancu klase ClaimsPrincipal i proslijeđuje drugu instancu klase ClaimsPrincipal kao argument konstruktoru. Drugim riječima, stvara se nova instanca koja je inicijalizirana drugom instancom klase ClaimsPrincipal 
    private ClaimsPrincipal _anonymous = new(new ClaimsPrincipal());

    // ova metoda sluzi za dobijanje stanja autentifikacije korisnika
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {   // uzima token iz lokalnog skladista 
            string stringToken = await localStorage.GetItemAsStringAsync("token");

            // ako je token prazan ili nije pronadjen vraca se stanje autentifikacije koje odgovara anonimnom korisniku (sto znaci da korisnik nije autentifikovan)
            if (string.IsNullOrWhiteSpace(stringToken))
                return new AuthenticationState(_anonymous);

            //ako se token uspesno uzme, iz skladista, izvlace se tvrdnje korisnika iz tokena, te tvrdnje sadrze info o korisniku i privilegijama koje ima 
            var tvrdnje = Generics.GetClaimsFromToken(stringToken);

            // koristi se metoda SetClaimsPrincipal kako bi se stvorio ClaimsPrincipal objekat koji sadrzi tvrdnje o korisniku
            var claimsPrincipal = Generics.SetClaimsPrincipal(tvrdnje);

            //vraca se stanje autentifikacije koje sadrzi ovaj ClaimsPrincipal objekat
            return new AuthenticationState(claimsPrincipal);
        }

        catch
        {
            //Ako se dogodi bilo kakva greška pri pokušaju dohvatanja tokena ili obradi tvrdnji, metoda će uhvatiti iznimku i vratiti stanje autentikacije koje odgovara anonimnom korisniku.
            return new AuthenticationState(_anonymous);
        }

    }

    //Ova metoda UpdateAuthenticationState se koristi za ažuriranje stanja autentikacije u aplikaciji na osnovu predanog tokena. -- login i logout
    public async Task UpdateAuthenticationState(string? token)
    {
        // Prvo se kreira nova instanca ClaimsPrincipal objekta koji će predstavljati autentificiranog korisnika.
        ClaimsPrincipal claimsPrincipal = new();

        //Provjerava se da li je predani token prazan ili null
        if (!string.IsNullOrWhiteSpace(token))
        {
            // Ako token nije prazan, izvlače se tvrdnje korisnika iz tokena pomoću funkcije Generics.GetClaimsFromToken(token)
            var sesijaKorisnika = Generics.GetClaimsFromToken(token);

            // Koristi se funkcija Generics.SetClaimsPrincipal kako bi se stvorio ClaimsPrincipal objekt koji sadrži tvrdnje korisnika.
            claimsPrincipal = Generics.SetClaimsPrincipal(sesijaKorisnika);

            //Ako token nije prazan, ažurira se lokalno skladište (localStorage) sa novim tokenom
            await localStorage.SetItemAsStringAsync("token", token);
        }
        else
        {
            
            claimsPrincipal = _anonymous;

            //Ako je token prazan, brise se  iz lokalnog skladišta(localStorage)
            await localStorage.RemoveItemAsync("token");
        }

        //obavještava se Blazor aplikacija o promjeni stanja autentikacije pozivanjem NotifyAuthenticationStateChanged metode. Kao argument se predaje rezultat asinkronog poziva Task.FromResult, koji sadrži novo stanje autentikacije kreirano na osnovu claimsPrincipal.
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}
