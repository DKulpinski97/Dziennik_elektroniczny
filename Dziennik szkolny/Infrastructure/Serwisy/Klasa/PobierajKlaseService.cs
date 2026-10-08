using Dziennik_szkolny.Application.Interfejsy.Klasa;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Klasa
{
    public class PobierajKlaseService : IPobierajKlase
    {
        readonly private AppDbContext _dbContext;
        public PobierajKlaseService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> PobieranieIdKlasyPoDacieIOznaczeniuAsync(string oznaczenie, int rokRozpoczecia)
        {
            var klasa = await _dbContext.Klasa.FirstOrDefaultAsync(x => x.Oznaczenie == oznaczenie && x.RokRozpoczecia == rokRozpoczecia);
            return klasa?.IdKlasy ?? 0;
        }
        public async Task<List<Dziennik_szkolny.Domain.Entities.Klasa>> PobierzWszystkieKlasy()
        {
            return await _dbContext.Klasa.ToListAsync();
        }

        public async Task<bool> SprawdzCzyKlasaIstniejePoDacieIOznaczeniuAsync(string oznaczenie, int rokRozpoczecia)
        {
            bool istnieje = await _dbContext.Klasa.AnyAsync(x => x.Oznaczenie == oznaczenie && x.RokRozpoczecia == rokRozpoczecia);
            return istnieje;
        }
    }
}
