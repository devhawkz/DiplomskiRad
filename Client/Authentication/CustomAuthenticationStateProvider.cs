using Client.Services.ToolsService;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Client.Authentication;

public class CustomAuthenticationStateProvider(AuthenticationService authenticationService, IToolsService tools) : AuthenticationStateProvider
{
    // koristi se kada korisnik nije identifikovan
    private ClaimsPrincipal _gost = new(new ClaimsIdentity()); 

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try 
        {
            var getSesijuKorisnika = await authenticationService.GetDetaljeKorisnika();

            if (getSesijuKorisnika is null || string.IsNullOrEmpty(getSesijuKorisnika.Email))
                return await Task.FromResult(new AuthenticationState(_gost));

            var claimsPrincipal = authenticationService.SetClaimPrincipal(getSesijuKorisnika);
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        catch
        {
            return await Task.FromResult(new AuthenticationState(_gost));
        }
    }

    public async Task UpdateAuthenticationState(TokenProp tokenProp)
    {
        ClaimsPrincipal claimsPrincipal = new();

        //prijava
        if (tokenProp is not null && !string.IsNullOrEmpty(tokenProp!.Token))
        {
            await authenticationService.SetTokenULokalnoSkladiste(tools.SerializeObj(tokenProp));
            var getSesijuKorisnika = await authenticationService.GetDetaljeKorisnika();

            if (getSesijuKorisnika is not null && !string.IsNullOrEmpty(getSesijuKorisnika!.Email))
                claimsPrincipal = authenticationService.SetClaimPrincipal(getSesijuKorisnika);
        }

        // odjava
        else
        {
            claimsPrincipal = _gost;
            await authenticationService.UkloniTokenIzLokalnogSkladista();
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}
