namespace Server.Data;

public class KorisnickiNalog
{
    public int Id { get; set; }
    public string? Ime {  get; set; }
    public string? Email { get; set; }
    public string? Lozinka { get; set; }

    // veza M:N sa ulogom
    public List<KorisnickaUloga> KorisnickeUloge { get; set; }

}
