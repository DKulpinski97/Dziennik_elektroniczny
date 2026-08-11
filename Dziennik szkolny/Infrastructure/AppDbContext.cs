using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Models;
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
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
