using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DTOs;

public class KorisnikDTO
{   
    public int Id { get; set; }

    [Required]
    public string? Ime { get; set; }

    [Required, EmailAddress]
    public string? Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string? Lozinka { get; set; }

    [Required, DataType(DataType.Password), Compare(nameof(Lozinka))]
    public string? PotvrdaLozinke { get; set; }
}
