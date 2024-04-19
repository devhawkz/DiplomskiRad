using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.DTOs;

public class PrijavaDTO
{
    [Required, EmailAddress, DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string? Lozinka { get; set; }
}
