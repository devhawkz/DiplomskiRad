using Client.Models;
using Stripe.Checkout;

namespace Server.Repository.NaplataRespositories;

public interface INaplata
{
    string KreirajSesijuNaplate(List<Narudzbina> stavkeIzKorpe);
}
