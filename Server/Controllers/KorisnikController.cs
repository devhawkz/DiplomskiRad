using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Repository.KorisnikRespositories;
using SharedLibrary.DTOs;
using SharedLibrary.Responses;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KorisnikController(IKorisnickiNalog nalogService) : ControllerBase
{
    [HttpPost("registracija")]
    public async Task<ActionResult<ServiceResponse>> Registracija(KorisnikDTO model)
    {
        if (model is null)
            return BadRequest("Morate uneti sve podatke!");

        var odgovor = await nalogService.Registracija(model);

        if (!odgovor.Flag)
            return Ok(odgovor);

        // auto. prijava nakon registacije korisnika
        PrijavaDTO prijavaModel = new PrijavaDTO()
        {
            Email = model.Email,
            Lozinka = model.Lozinka
        };

        var prijavaRezultat = await nalogService.Prijava(prijavaModel);

        if (prijavaRezultat.Flag)
            return Ok(new PrijavaResponse(true, "Uspešna registracija i prijava!", prijavaRezultat.Token, prijavaRezultat.RefreshToken));

        else
            return Ok(new PrijavaResponse(false, "Prijava nije uspela nakon registracije!", null, null));
        

    }

    [HttpPost("prijava")]
    public async Task<ActionResult<PrijavaResponse>> Prijava(PrijavaDTO model)
    {
        if (model is null)
            return BadRequest("Morate uneti sve podatke!");

        var odgovor = await nalogService.Prijava(model);
        return Ok(odgovor);
    }

    [HttpGet("korisnik-info")]
    public async Task<ActionResult<SesijaKorisnika>> GetInfoKorisnika()
    {
        var token = GetTokenIzZaglavlja();
        if (string.IsNullOrEmpty(token))
            return Unauthorized();

        var getKorisnika = await nalogService.GetKorisnikaPoTokenu(token!);
        if(getKorisnika is null || string.IsNullOrEmpty(getKorisnika.Email))
            return Unauthorized();

        return Ok(getKorisnika);
    }

    private string GetTokenIzZaglavlja()
    {
        string token = string.Empty;
        foreach (var header in Request.Headers)
        {
            if (header.Key.ToString().Equals("Authorization"))
            {
                token = header.Value.ToString();
                break; // postoji samo jedna vrednost pod tim kljucem
            }
        }

        return token.Split(" ").LastOrDefault()!;
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<PrijavaResponse>> RefreshToken(PostRefreshTokenDTO model)
    {
        if (model is null) return Unauthorized();
        var rezultat = await nalogService.GetRefreshToken(model);
        return Ok(rezultat);
    }
}
