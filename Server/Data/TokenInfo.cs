using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Data;

public class TokenInfo
{
    [Key]
    public int Id { get; set; }
    public int KorisnickiNalogId { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime DatumKreiranja { get; set; } = DateTime.Now;
    public DateTime DatumIsteka {  get; set; } = DateTime.Now.AddMinutes(15);


}
