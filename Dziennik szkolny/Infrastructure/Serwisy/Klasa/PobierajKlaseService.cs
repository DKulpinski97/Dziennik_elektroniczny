using Dziennik_szkolny.Application.Interfejsy.Klasa;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Klasa
{
    public class PobierajKlaseService : IPobierajKlase
    {
        readonly private AppDbContext _dbContext;
        public PobierajKlaseService(AppDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public async Task<bool> SprawdzCzyKlasaIstniejePoDacieIOznaczeniuAsync(string oznaczenie, int rokRozpoczecia)
        {
            bool istnieje = await _dbContext.Klasa.AnyAsync(x => x.Oznaczenie == oznaczenie && x.RokRozpoczecia == rokRozpoczecia);
            return istnieje;
        }
    }
}
