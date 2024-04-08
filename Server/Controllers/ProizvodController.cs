using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse>> DodajProizvode(Proizvod proizvodModel)
    {
        if (proizvodModel is null)
            return BadRequest("Nije izabran nijedan proizvod");

        var odgovor = await proizvodService.DodajProizvod(proizvodModel);
        return Ok(odgovor);
    }
}
