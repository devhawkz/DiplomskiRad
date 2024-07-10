using SharedLibrary.Models;

namespace Server.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{ 
    // inicijalizuje svojstvo na njegovu default vrednost, operator default! signalizira kompajleru da vrednost ne tretira kao null, cak kada ona to tehnicki i jeste, koristi se kako bi se omogucilo odlozeno inicijalizovanje svojstava koje EFCore automatski popunjava
    public DbSet<Proizvod> Proizvodi { get; set; } = default!;
    public DbSet<Kategorija> Kategorije { get; set; } = default!;
    public DbSet<KorisnickiNalog> KorisnickiNalozi { get; set; } = default!;
    public DbSet<KorisnickaUloga> KorisnickeUloge {  get; set; } = default!;
    public DbSet<Uloga> Uloge { get; set; } = default!;
    public DbSet<TokenInfo> TokenInfo { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Proizvod>()
               .HasIndex(p => p.KategorijaId)
               .HasDatabaseName("IX_KategorijaId");

        modelBuilder.Entity<Proizvod>()
               .HasOne(p => p.Kategorija)
               .WithMany(k => k.Proizvodi)
               .HasForeignKey(p => p.KategorijaId)
               .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<KorisnickaUloga>()
                .HasOne(ku => ku.KorisnickiNalog)
                .WithMany(kn => kn.KorisnickeUloge)
                .HasForeignKey(ku => ku.KorisnickiNalogId)
                .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<KorisnickaUloga>()
               .HasOne(ku => ku.Uloga)
               .WithMany(u => u.KorisnickeUloge)
               .HasForeignKey(ku => ku.UlogaId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
