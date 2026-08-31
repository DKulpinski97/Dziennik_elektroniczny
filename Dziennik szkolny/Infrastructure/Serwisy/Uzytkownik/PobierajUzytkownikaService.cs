using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Modele;
using Dziennik_szkolny.Application.ObiektyTransferuDanych;
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
        public async Task<List<UzytkownikZRolamiDto>> PobierzUzytkownikowPoRolachAsync(List<string> role)
        {
            var wynik = new List<UzytkownikZRolamiDto>();

            foreach (var nazwaRoli in role)
            {
                var uzytkownicy =await _userManager.GetUsersInRoleAsync(nazwaRoli);

                foreach (var uzytkownik in uzytkownicy)
                {
                    var istnieje = wynik.FirstOrDefault(x => x.Id == uzytkownik.Id);

                    if (istnieje != null)
                    {
                        istnieje.Role.Add(nazwaRoli);
                        continue;
                    }

                    wynik.Add(new UzytkownikZRolamiDto
                    {
                        Id = uzytkownik.Id,
                        Login = uzytkownik.UserName,
                        Role = [nazwaRoli]
                    });
                }
            }

            return wynik;
        }
    }
}