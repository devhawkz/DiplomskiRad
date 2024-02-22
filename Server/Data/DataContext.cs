
using SharedLibrary.Models;

namespace Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        // označava da se očekuje da će ova vrijednost biti postavljena na nešto drugo prije nego što se koristi. To znači da će, ako nije eksplicitno postavljena drugačija vrijednost, ova svojstvo biti postavljeno na njegovu zadanu vrijednost.
        public DbSet<Proizvod> Proizvodi { get; set; } = default!; 
    }
}
