using Blazored.LocalStorage;
using Client.Services.ToolsService;
using Microsoft.AspNetCore.WebUtilities;
using SharedLibrary.DTOs;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Client.Authentication;

public class AuthenticationService(HttpClient http, ILocalStorageService lokalnoSkladiste, IToolsService tools)
{
    public async Task<SesijaKorisnika> GetDetaljeKorisnika()
    {
        var token = await GetTokenIzLokalnogSkladista();
        if (string.IsNullOrEmpty(token))
            return null!;

        var httpClient = await AddZaglavljeToHttpClient();
        var detaljiKorisnika = tools.DeserializeJsonString<TokenProp>(token);
        if (detaljiKorisnika is null || string.IsNullOrEmpty(detaljiKorisnika.Token))
            return null!;

        var response = await GetDetaljeKorisnikaSaApi();
        if (response.IsSuccessStatusCode)
        {
            return await GetSesijuKorisnika(response);
        }

        // ako je korisniku istekao access token
        else if (http.DefaultRequestHeaders.Contains("Authorization") && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var encodedToken = Encoding.UTF8.GetBytes(detaljiKorisnika.RefreshToken!);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);
            var model = new PostRefreshTokenDTO() { RefreshToken = validToken };

            var rezultat = await http.PostAsync("api/korisnik/refresh-token", tools.GenerateStringContent(tools.SerializeObj(model)));

            if (!rezultat.IsSuccessStatusCode)
            {
                Console.WriteLine($"Neuspešno osvežavanje tokena: {rezultat.StatusCode}");
                return null!;
            }

            var apiResponse = await rezultat.Content.ReadAsStringAsync();
            var prijavaResponse = tools.DeserializeJsonString<PrijavaResponse>(apiResponse);
            await SetTokenULokalnoSkladiste(tools.SerializeObj(new TokenProp()
            {
                Token = prijavaResponse.Token,
                RefreshToken = prijavaResponse.RefreshToken
            }));

            var callApiAgain = await GetDetaljeKorisnikaSaApi();
            if (callApiAgain.IsSuccessStatusCode)
                return await GetSesijuKorisnika(callApiAgain);
        }

        return null!;
    }

    private async Task<string?> GetTokenIzLokalnogSkladista() => await lokalnoSkladiste.GetItemAsStringAsync("access_token");

    public async Task<HttpClient> AddZaglavljeToHttpClient()
    {
        http.DefaultRequestHeaders.Remove("Authorization");
        
        // dodaje se access token u zaglavlje
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tools.DeserializeJsonString<TokenProp>(await GetTokenIzLokalnogSkladista()).Token);

        return http;
    }

    private async Task<HttpResponseMessage> GetDetaljeKorisnikaSaApi()
    {
        var httpClient = await AddZaglavljeToHttpClient();
        return await httpClient.GetAsync("api/korisnik/korisnik-info");
    }

    public async Task<SesijaKorisnika> GetSesijuKorisnika(HttpResponseMessage response)
    {
        var apiResponse = await response.Content.ReadAsStringAsync();
        return tools.DeserializeJsonString<SesijaKorisnika>(apiResponse);
    }

    public async Task SetTokenULokalnoSkladiste(string token) => await lokalnoSkladiste.SetItemAsStringAsync("access_token", token);

    public async Task UkloniTokenIzLokalnogSkladista() => await lokalnoSkladiste.RemoveItemAsync("access_token");

    public ClaimsPrincipal SetClaimPrincipal(SesijaKorisnika model)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(
            new List<Claim>
            {
                new(ClaimTypes.Name, model.Ime!),
                new(ClaimTypes.Email, model.Email!),
                new(ClaimTypes.Role, model.Uloga!)
            }, "AccessTokenAuth"));
    }

}
