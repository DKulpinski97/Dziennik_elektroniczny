using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure
{
    public class AppDbContext : IdentityDbContext<LoginUzytkownika>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<InformacjeUzytkownik> InformacjeUzytkownik { get; set; }

        public DbSet<UprawnienieZarzadzaniaRola> UprawnieniaZarzadzaniaRola { get; set; }
        public DbSet<Klasa> Klasa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UprawnienieZarzadzaniaRola>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(x => x.RolaZarzadzajacaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UprawnienieZarzadzaniaRola>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(x => x.RolaZarzadzanaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Klasa>()
            .HasOne(x => x.Wychowawca)
            .WithMany()
            .HasForeignKey(x => x.WychowawcaId)
            .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
    }
}