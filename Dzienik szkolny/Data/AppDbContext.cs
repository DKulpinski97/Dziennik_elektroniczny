using Dzienik_szkolny.Models;
using Microsoft.EntityFrameworkCore;

namespace Dzienik_szkolny.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        DbSet<LoginUzytkownika> LoginyUzytkowiow { get; set; }
        DbSet<InformacjeRodzic> InformacjeUzytkownik { get; set; }
        DbSet<AdresUzytkownika> AdresUzytkownika { get; set; }
        DbSet<InformacjePracownik> InformacjePracownik { get; set; }
        DbSet<LoczenieRoli> LoczenieRoli { get; set; }
        DbSet<ListaRole> ListaRole { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
