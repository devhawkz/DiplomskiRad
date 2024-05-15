namespace Client.Models;

public class Narudzbina
{
    public int Id { get; set; }
    public string? Naziv { get; set; }
    public decimal Cena { get; set; }
    public int Kolicina { get; set; }
    public string? Slika { get; set; }

    public decimal UkupnaCena => Kolicina * Cena;
}
