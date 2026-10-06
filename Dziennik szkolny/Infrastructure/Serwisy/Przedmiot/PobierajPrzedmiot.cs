using Dziennik_szkolny.Application.Interfejsy.Przedmioty;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Przedmiot
{
    public class PobierajPrzedmiot : IPobierajPrzedmiot
    {
        readonly private AppDbContext _dbContext;
        public PobierajPrzedmiot(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> SprawdzCzyPrzedmiotIstniejePoNazwieAsync(string nazwaPrzedmiotu)
        {

            bool istnieje = await _dbContext.Przedmioty.AnyAsync(p => p.NazwaPrzedmiotu == nazwaPrzedmiotu);
            return istnieje;
        }
    }
}
