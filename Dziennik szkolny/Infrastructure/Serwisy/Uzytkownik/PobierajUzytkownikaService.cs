using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Uzytkownik
{
    public class PobierajUzytkownikaService : IPobierajUzytkownika
    {
        readonly private UserManager<LoginUzytkownika> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        readonly private AppDbContext _dbContext;
        public PobierajUzytkownikaService(UserManager<LoginUzytkownika> userManager, AppDbContext dbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }

        public async Task<LoginUzytkownika> PobierzUzytkownikaPoLoginieAsync(string login)
        {
            return await _userManager.FindByNameAsync(login);
        }
        public async Task<InformacjeUzytkownik> PobierzInformacjeUzytkownikaPoIdLoginu(string IdUzytkownika)
        {
            return await _dbContext.InformacjeUzytkownik.FirstOrDefaultAsync(x => x.IdUzytkownika == IdUzytkownika);
        }

        public async Task<List<LoginUzytkownika>> PobierzWszystkichUzytkownikow()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<List<InformacjeUzytkownik>> PobierzWszystkieInformacjeOUzrzytkownikach()
        {
            return await _dbContext.InformacjeUzytkownik.ToListAsync();
        }
        public async Task<List<InformacjeUzytkownik>> PobierzUzytkownikowImieINazwiskoIIDPoLiscieID(List<string> idUzytkownikow)
        {
         return await _dbContext.InformacjeUzytkownik
        .Where(x => idUzytkownikow.Contains(x.IdUzytkownika))
        .Select(x => new InformacjeUzytkownik
        {
            IdUzytkownika = x.IdUzytkownika,
            Imie = x.Imie,
            Nazwisko = x.Nazwisko
        })
        .ToListAsync();
        }
        public async Task<LoginUzytkownika> PobierzUzytkownikaPoIDAsync(string IdUzytkownika)
        {
            return await _userManager.FindByIdAsync(IdUzytkownika);
        }
        public async Task<List<UzytkownikZRolamiDto>> PobierzUzytkownikowPoRolachAsync(List<string> role)
        {
            var roleUzytkownikow = await (
               from ur in _dbContext.UserRoles
               join r in _dbContext.Roles on ur.RoleId equals r.Id
               join u in _dbContext.Users on ur.UserId equals u.Id
               where role.Contains(r.Name)
               select new
               {
                   u.Id,
                   Login = u.UserName,
                   NazwaRoli = r.Name
               }).ToListAsync();

            return roleUzytkownikow
                .GroupBy(x => new { x.Id, x.Login })
                .Select(g => new UzytkownikZRolamiDto
                {
                    Id = g.Key.Id,
                    Login = g.Key.Login,
                    Role = g.Select(x => x.NazwaRoli).Distinct().ToList()
                })
                .ToList();
        }

        public async Task<string> PobierzIdUzytkownikaPoLoginieAsync(string login)
        {
            var uzytkownik = await _userManager.FindByNameAsync(login);
            return uzytkownik?.Id;
        }

        public async Task<bool> SprawdzCzyInformacjeUzytkownikaIstniejaPoIdUzytkownikaAsync(string idUzytkownika)
        {
            bool istnieje = await _dbContext.InformacjeUzytkownik.AnyAsync(x => x.IdUzytkownika == idUzytkownika);
            return istnieje;

        }
        public async Task<string> PobierzEmailUzytkownikaAsync(string idUzytkownika)
        {
            return await _userManager.Users
                .Where(u => u.Id == idUzytkownika)
                .Select(u => u.NormalizedEmail)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> CzyPeselIstniejeAsync(string pesel)
        {
            return await _dbContext.InformacjeUzytkownik.AnyAsync(x => x.Pesel == pesel);
        }
        public async Task<bool> CzyIstniejeInnyUzytkownikOTymPeseluAsync(string pesel, string idUzytkownika)
        {
            return await _dbContext.InformacjeUzytkownik.AnyAsync(x => x.Pesel == pesel && x.IdUzytkownika != idUzytkownika);
        }
        public async Task<bool> CzyUzytkownikJestOpiekunemAsync(string idUzytkownika)
        {
            return await _dbContext.Uczniowie.AnyAsync(x => x.IdOpiekun1 == idUzytkownika || x.IdOpiekun2 == idUzytkownika);
        }
        public async Task<bool> CzyUzytkownikProwadziPrzedmiotAsync(string idUzytkownika)
        {
            return await _dbContext.PrzypisanePrzedmioty.AnyAsync(p => p.IdNauczyciela == idUzytkownika);
        }
        public async Task<bool> CzyUzytkownikJestWychowawcaAsync(string idUzytkownika)
        {
            return await _dbContext.Klasa.AnyAsync(w => w.IdWychowawcy == idUzytkownika);
        }
        public async Task<bool> SprawdzCzyJestUzytkownikKtoryPosiadaTylkoTaRoleAsync(string idRoli)
        {
            return await _dbContext.UserRoles.AnyAsync(ur => ur.RoleId == idRoli
            && !_dbContext.UserRoles.Any(ur2 => ur2.UserId == ur.UserId && ur2.RoleId != idRoli));
        }
    }
}