using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Modele;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Uzytkownik
{
    public class PobierajUzytkownikaService : IPobierajUzytkownika
    {
        readonly private UserManager<LoginUzytkownika> _userManager;
        readonly private AppDbContext _dbContext;
        public PobierajUzytkownikaService(UserManager<LoginUzytkownika> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<LoginUzytkownika> PobierzUzytkownikaPoLoginieAsync(string login)
        {
            return await _userManager.FindByNameAsync(login);
        }
        public async Task<InformacjeUzytkownik> PobierzInformacjeUzytkownikaPoIdLoginu(string IdUzytkownika)
        {
            return  await _dbContext.InformacjeUzytkownik.FirstOrDefaultAsync(x => x.IdUzytkownika == IdUzytkownika);
        }

        public async Task<List<LoginUzytkownika>> PobierzWszystkichUzytkownikow()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<List<InformacjeUzytkownik>> PobierzWszystkieInformacjeOUzrzytkownikach()
        {
            return await _dbContext.InformacjeUzytkownik.ToListAsync();
        }
        public async Task<LoginUzytkownika> PobierzUzytkownikaPoIDAsync(string IdUzytkownika)
        {
            return await _userManager.FindByIdAsync(IdUzytkownika);
        }
    }
}