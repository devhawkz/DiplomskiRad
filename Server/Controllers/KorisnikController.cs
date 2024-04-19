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
        return Ok(odgovor);
    }

    [HttpPost("prijava")]
    public async Task<ActionResult<PrijavaResponse>> Prijava(PrijavaDTO model)
    {
        if (model is null)
            return BadRequest("Morate uneti sve podatke!");

        var odgovor = await nalogService.Prijava(model);
        return Ok(odgovor);
    }
}
