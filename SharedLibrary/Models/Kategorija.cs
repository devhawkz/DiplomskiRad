using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharedLibrary.Models;

public class Kategorija
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Polje Naziv je obavezno.")]
    public string? Naziv { get; set; } = string.Empty;

    [JsonIgnore]
    //relacije -- 1 kategorija moze da ima vise proizvoda, 1:N
    public List<Proizvod>? Proizvodi {  get; set; }

}
