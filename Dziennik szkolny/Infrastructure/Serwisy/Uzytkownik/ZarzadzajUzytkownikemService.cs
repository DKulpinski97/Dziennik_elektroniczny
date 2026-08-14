using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Uzytkownik
{
    public class ZarzadzajUzytkownikemService : IZarzadzajUzytkownikem
    {

        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly IPobierajRole _pobierajRole;
        private readonly AppDbContext _dbContext;

        public ZarzadzajUzytkownikemService(UserManager<LoginUzytkownika> userManager, IPobierajRole pobierajRole, AppDbContext dbContext)
        {
            _userManager = userManager;
            _pobierajRole = pobierajRole;
            _dbContext = dbContext;
        }

        public async Task<bool> DodajUzytkownikaAsync(UzytkownikaViewModel model)
        {
            var nowyLogin = new LoginUzytkownika
            {
                UserName = model.Login,
                Email = model.Email
            };

            var wynik = await _userManager.CreateAsync(nowyLogin,model.Haslo);

            return wynik.Succeeded;
        }
        public async Task<bool> EdytujUzytkownikaAsync(UzytkownikaViewModel model,bool zmienHaslo)
        {
            var uzytkownik = await _userManager.FindByIdAsync(model.idUzytkownika);

            if (uzytkownik == null)
            {
                return false;
            }

            uzytkownik.UserName = model.Login;
            uzytkownik.Email = model.Email;

            if (zmienHaslo)
            {
                uzytkownik.PasswordHash =
                    _userManager.PasswordHasher.HashPassword(
                        uzytkownik,
                        model.Haslo);
            }

            var wynik = await _userManager.UpdateAsync(uzytkownik);

            return wynik.Succeeded;
        }
        public async Task<bool> DodajRoleUzytkownikowiAsync(LoginUzytkownika uzytkownik,List<string> idRoli)
        {
            if (idRoli == null || !idRoli.Any())
            {
                return false;
            }

            foreach (var idRoliItem in idRoli)
            {
                var rola = await _pobierajRole.PobierzRolePoIdAsync(idRoliItem);

                if (rola == null)
                {
                    return false;
                }

                var wynik = await _userManager.AddToRoleAsync(uzytkownik,rola.Name);

                if (!wynik.Succeeded)
                {
                    return false;
                }
            }

            return true;
        }
        public async Task<bool> DodajInformacjeUzytkownikaAsync(LoginUzytkownika uzytkownik, UzytkownikaViewModel model)
        {
            long ostatniId = await _dbContext.InformacjeUzytkownik.Select(x => (long?)x.IdOsoby).MaxAsync() ?? 0;

            long nowyId = ostatniId + 1;
            var informacje = new InformacjeUzytkownik
            {
                IdOsoby = nowyId,
                IdUzytkownika = uzytkownik.Id,
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Pesel = model.Pesel,
                Telefon = model.Telefon,
                Miasto = model.Miasto,
                Ulica = model.Ulica,
                NrMieszkania = model.NrMieszkania
            };


            await _dbContext.InformacjeUzytkownik.AddAsync(informacje);
            var wynik = await _dbContext.SaveChangesAsync();

            return wynik > 0;


        }

        public async Task<bool> EdytujInformacjeUzytkownikaAsync(InformacjeUzytkownik informacjeUzytkownika)
        {
            _dbContext.InformacjeUzytkownik.Update(informacjeUzytkownika);

            var wynik = await _dbContext.SaveChangesAsync();
            return wynik > 0;
        }
        public async Task<bool> UsunWszystkieRoleUzytkownikowiAsync(string login)
        {
            var uzytkownik = await _userManager.FindByNameAsync(login);

            if (uzytkownik == null)
            {
                return false;
            }

            var obecneRole = await _userManager.GetRolesAsync(uzytkownik);

            if (!obecneRole.Any())
            {
                return true;
            }

            var wynik = await _userManager.RemoveFromRolesAsync(
                uzytkownik,
                obecneRole);

            return wynik.Succeeded;
        }
    }
}
