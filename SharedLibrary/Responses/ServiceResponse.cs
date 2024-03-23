namespace SharedLibrary.Responses;

public record class ServiceResponse(bool Flag, string Poruka);

public record class PrijavaResponse(bool Flag, string Token, string Poruka);


