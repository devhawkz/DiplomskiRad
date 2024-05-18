namespace Client.Services.NaplataServices;

public interface INaplata
{
    Task<string> Racun(List<Narudzbina> listaStavki);
} 
