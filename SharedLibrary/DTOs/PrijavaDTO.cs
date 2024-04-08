using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DTOs;

public class PrijavaDTO
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; } = string.Empty;


    [Required]
    [DataType(DataType.Password)]
    public string? Lozinka { get; set; } = string.Empty;
}
