using Client.Services.ProizvodServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Repository.KategorijaResposities;
using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KategorijaController(IKategorija kategorijaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Kategorija>>> GetKategorije()
    {
        var kategorije = await kategorijaService.GetKategorije();
        return Ok(kategorije);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse>> DodajKategoriju(Kategorija kategorijaModel)
    {
        if (kategorijaModel is null) return BadRequest("Nije izabrana nijedna kategorija"!);

        var odgovor = await kategorijaService.DodajKategoriju(kategorijaModel);
        return Ok(odgovor);
    }

    [HttpPost("obrisi-kategoriju")]
    public async Task<ActionResult<ServiceResponse>> ObrisiKategoriju(string nazivKategorije)
    {

        if (string.IsNullOrEmpty(nazivKategorije) || string.IsNullOrWhiteSpace(nazivKategorije) || nazivKategorije.Equals(""))
            return BadRequest("Nije izabrana nijedna kategorija!");

        var odgovor = await kategorijaService.ObrisiKategoriju(nazivKategorije);
        return Ok(odgovor);
    }
}
