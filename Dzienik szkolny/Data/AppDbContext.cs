using Dzienik_szkolny.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dzienik_szkolny.Data
{
    public class AppDbContext : IdentityDbContext<LoginUzytkownika>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<InformacjeRodzic> InformacjeUzytkownik { get; set; }
        public DbSet<AdresUzytkownika> AdresUzytkownika { get; set; }
        public DbSet<InformacjePracownik> InformacjePracownik { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
