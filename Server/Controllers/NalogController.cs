using Microsoft.AspNetCore.Mvc;
using Server.Repository.KorisnikRespositories;
using SharedLibrary.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NalogController(IKorisnik nalogKorisnika) : ControllerBase
    {

        [HttpPost("registracija")]
        public async Task<IActionResult> Registracija(KorisnikDTO korisnikDTO)
        {
            var response = await nalogKorisnika.Registracija(korisnikDTO);
            return Ok(response);
        }

        [HttpPost("prijava")]
        public async Task<IActionResult> Prijava(PrijavaDTO prijavaDTO)
        {
            var response = await nalogKorisnika.Prijava(prijavaDTO);
            return Ok(response);
        }
    }
}
