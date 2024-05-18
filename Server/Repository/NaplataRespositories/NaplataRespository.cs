using Client.Models;
using Stripe;
using Stripe.Checkout;

namespace Server.Repository.NaplataRespositories;

public class NaplataRespository : INaplata
{
    public NaplataRespository()
    {
        StripeConfiguration.ApiKey = "sk_test_51PHiy21eDbOFZwMqbBzh6COfgK6pyRj2ZoEHYhjybeEfWTzT7TEASoQAaYmuiVxdZbRqTNiP6NKPltwYGYxCIytx00MPLBjQ2l";    
    }

    // povratna vrednost je tipa string kako bi mogli da vratimo service response
    public string KreirajSesijuNaplate(List<Narudzbina> stavkeIzKorpe)
    {
        if(stavkeIzKorpe is null) return null!;

        var listaStavki = new List<SessionLineItemOptions>();
        
        // dodavanje proizvoda i njegovih detalja u checkout sesiju
        stavkeIzKorpe.ForEach(ci => listaStavki.Add(new SessionLineItemOptions()
        {
            PriceData = new SessionLineItemPriceDataOptions()
            {
                UnitAmountDecimal = ci.Cena * 100,
                Currency = "rsd",
                ProductData = new SessionLineItemPriceDataProductDataOptions()
                {
                    Name = ci.Naziv,
                    Description = ci.Id.ToString(),
                }
            },

            Quantity = ci.Kolicina
        }));

        // opcije placanja
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems = listaStavki,
            Mode = "payment",
            SuccessUrl = "https://localhost:7151/uspesna-narudzbina",
            CancelUrl = "https://localhost:7151"
        };

        // kreiranje sesije za placanje (checkout sesija)
        var servis = new SessionService();
        Session sesija = servis.Create(options);
        return sesija.Url;
    }
}
