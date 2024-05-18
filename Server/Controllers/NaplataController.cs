using Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Repository.NaplataRespositories;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]

public class NaplataController(INaplata naplataService) : ControllerBase
{
    [HttpPost("racun")]
    public ActionResult KreirajSesijuPlacanja(List<Narudzbina> listaStavki)
    {
        var url = naplataService.KreirajSesijuNaplate(listaStavki);
        return Ok(url); ;
    }
}
