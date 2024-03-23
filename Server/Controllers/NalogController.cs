using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Repository.KorisnikRespository;
using SharedLibrary.DTOs;
using SharedLibrary.Responses;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NalogController(IKorisnickiNalog korisnickiNalog) : ControllerBase
    {
        [HttpPost("registracija")]
        public async Task<ActionResult<ServiceResponse>> Registracija(KorisnikDTO korisnikDTO)
        {
            if (korisnikDTO is null) return BadRequest("Morate da popunite sve podatke");

            var response = await korisnickiNalog.Registracija(korisnikDTO);
            return Ok(response);
        }

        [HttpPost("prijava")]
        public async Task<ActionResult<ServiceResponse>> Prijava(PrijavaDTO prijavaDTO)
        {
            if (prijavaDTO is null) return BadRequest("Morate popuniti sve podatke");
            var response = await korisnickiNalog.Prijava(prijavaDTO);
            return Ok(response);
        }
    }
}
