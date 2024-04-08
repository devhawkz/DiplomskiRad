using Microsoft.AspNetCore.Identity;

namespace Server.Data;

public class AppKorisnik : IdentityUser
{
    public string? Ime { get; set; }
}
