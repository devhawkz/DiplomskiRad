namespace Server.Data;

// koristi se kao usermanager klasa
public class AppKorisnik : IdentityUser
{
    public string? Ime { get; set; } = string.Empty;
}
