using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Models;

public class Proizvod
{

    public int Id { get; set; }

    [Required]
    public string? Naziv { get; set; } = string.Empty;

    public string? Opis { get; set; } = string.Empty;

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal Cena { get; set; }

    [Required, DisplayName("Slika proizvoda")]
    public string? Base64Img { get; set; } = string.Empty; // skladistenje slike u obliku Base64 enkodiranog niza bajtova

    public int Kolicina { get; set; }

    public bool PreporucenProizvod { get; set; }

    public DateTime DatumPostavljanja {  get; set; } = DateTime.Now;

}
