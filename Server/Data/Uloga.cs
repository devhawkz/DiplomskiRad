namespace Server.Data;

public class Uloga
{
    public int Id { get; set; }
    public string? Naziv {  get; set; }

    // veza M:N sa korisnikom
    public List<KorisnickaUloga> KorisnickeUloge { get; set; }
}
