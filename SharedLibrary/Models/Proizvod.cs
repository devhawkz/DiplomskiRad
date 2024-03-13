using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedLibrary.Models;

public class Proizvod
{

    public int Id { get; set; }

    [Required(ErrorMessage = "Polje Naziv je obavezno.")]
    public string? Naziv { get; set; } = string.Empty;

    [Required(ErrorMessage = "Polje Opis je obavezno.")]
    public string? Opis { get; set; } = string.Empty;

    [Required, Column(TypeName = "decimal(18,2)"), Range(0.1, 99999.99, ErrorMessage = "Polje Cena je obavezno. Mora biti u opsegu od 0.1 do 99999.99")]
    public decimal Cena { get; set; }

    [Required(ErrorMessage = "Polje Slika je obavezno."), DisplayName("Slika proizvoda")]
    public string? Base64Img { get; set; } // skladistenje slike u obliku Base64 enkodiranog niza bajtova

    [Required, Range(1, 999, ErrorMessage = "Polje Količina je obavezno. Mora biti u opsegu od 1 do 999")]
    public int Kolicina { get; set; }

    public bool PreporucenProizvod { get; set; }

    public DateTime DatumPostavljanja {  get; set; } = DateTime.Now;


    //Relacije: Vise proizvoda moze biti u istoj kategoriji, N:1
    [Required(ErrorMessage = "Odabir Kategorije je obavezan.")]
    public Kategorija? Kategorija { get; set; }

    //strani kljuc tabele kategorija
    public int KategorijaId { get; set; }

}
