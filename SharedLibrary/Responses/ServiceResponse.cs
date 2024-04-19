namespace SharedLibrary.Responses;

public record class ServiceResponse(bool Flag, string Poruka=null!);

public record class PrijavaResponse(bool Flag, string Poruka=null!, string Token = null!, string RefreshToken = null!);


