using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Uczen
{

    public class PobierajUczniaService : Application.Interfejsy.Uczen.IPobierajUcznia
    {
        readonly private AppDbContext _dbContext;
        public PobierajUczniaService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SprawdzCzyUczenIstniejePoPeselAsync(string pesel)
        {
            bool istnieje = await _dbContext.Uczniowie.AnyAsync(x => x.Pesel == pesel);
            return istnieje;

        }
    }
}
