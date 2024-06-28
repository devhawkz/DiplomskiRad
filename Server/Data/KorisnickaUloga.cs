namespace Server.Data;

public class KorisnickaUloga
{
    public int Id { get; set; }

    public int KorisnickiNalogId { get; set; }
    public KorisnickiNalog KorisnickiNalog { get; set; }
    
    public Uloga Uloga { get; set; }
    public int UlogaId {  get; set; }
}
