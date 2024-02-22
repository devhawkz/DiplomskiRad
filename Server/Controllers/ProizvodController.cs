using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Contracts;
using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProizvodController : ControllerBase
{
    private readonly IProizvod _proizvodService;

    public ProizvodController(IProizvod proizvodService)
    {
        _proizvodService = proizvodService;
    }

    [HttpGet]
     public async Task<ActionResult<List<Proizvod>>> GetProizvode(bool preporuceniProizvodi)
    {
        var proizvodi = await _proizvodService.GetProizvode(preporuceniProizvodi);
        return Ok(proizvodi);
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse>> DodajProizvode(Proizvod proizvodModel)
    {
        if (proizvodModel is null)
            return BadRequest("Nije izabran nijedan proizvod");

        var odgovor = await _proizvodService.DodajProizvod(proizvodModel);
        return Ok(odgovor);
    }
}
