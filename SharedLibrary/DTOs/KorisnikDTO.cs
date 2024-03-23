using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DTOs;

public class KorisnikDTO
{
    public int Id {  get; set; }

    [Required]
    public string? Ime { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string? Lozinka { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string? PotvrdaLozinke { get; set; } = string.Empty;


}
