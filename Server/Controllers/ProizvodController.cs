using Microsoft.AspNetCore.Mvc;
using Server.Repository.ProizvodRespositories;
using SharedLibrary.Models;
using SharedLibrary.Responses;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProizvodController(IProizvod proizvodService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Proizvod>>> GetProizvode(bool preporuceniProizvodi)
    {
        var proizvodi = await proizvodService.GetProizvode(preporuceniProizvodi);
        return Ok(proizvodi);
    }


    [HttpPost]
    public async Task<ActionResult<ServiceResponse>> DodajProizvode(Proizvod proizvodModel)
    {
        if (proizvodModel is null)
            return BadRequest("Nije izabran nijedan proizvod");

        var odgovor = await proizvodService.DodajProizvod(proizvodModel);
        return Ok(odgovor);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse>> ObrisiProizvod(int id)
    {
        if (id <= 0)
            return BadRequest("Nije izabran nijedan proizvod");

        var odgovor = await proizvodService.ObrisiProizvod(id);
        return Ok(odgovor);
    }


    [HttpPut]
    public async Task<ActionResult<ServiceResponse>> AzurirajProizvod(Proizvod proizvodModel)
    {
        if (proizvodModel is null)
            return BadRequest("Nije izabran nijedan proizvod");

        var odgovor = await proizvodService.AzurirajProizvod(proizvodModel);
        return Ok(odgovor);
    }
}
