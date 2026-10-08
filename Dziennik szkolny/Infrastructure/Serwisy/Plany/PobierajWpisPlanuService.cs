using Dziennik_szkolny.Application.Interfejsy.Plany;
using Dziennik_szkolny.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Plany
{
    public class PobierajWpisPlanuService : IPobierajWpisPlanu
    {
        readonly private AppDbContext _dbContext;
        public PobierajWpisPlanuService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SprawdzCzyIstniejeWpisPlanuAsync(int idKlasy, byte lekcja, DzienTygodnia dzien)
        {
            bool istnieje = await _dbContext.WpisyPlanu.AnyAsync(p => p.IdKlasy == idKlasy && p.Lekcja == lekcja && p.Dzien == dzien);
            return istnieje;
        }
    }
}
