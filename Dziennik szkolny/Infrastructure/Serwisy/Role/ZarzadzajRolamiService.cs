using Dziennik_szkolny.Application.Interfejsy;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Dziennik_szkolny.Infrastructure.Serwisy.Role
{
    public class ZarzadzajRolamiService : IZarzadzajRolami
    {
        private readonly IPobierajUprawnieniaRoli _pobierajUprawnieniaRoli;
        private readonly RoleManager<IdentityRole> _roleManager;
        readonly private AppDbContext _context;
        private readonly Dziennik_szkolny.Infrastructure.DaneStartowe.DaneStartowe _daneStartowe;
        private readonly IJednostkaPracy _jednostkaPracy;
        public ZarzadzajRolamiService(RoleManager<IdentityRole> roleManager, Dziennik_szkolny.Infrastructure.DaneStartowe.DaneStartowe daneStartowe, 
            IPobierajUprawnieniaRoli pobierajUprawnieniaRoli, AppDbContext dbContext, IJednostkaPracy jednostkaPracy)
        {
            _roleManager = roleManager;
            _daneStartowe = daneStartowe;
            _pobierajUprawnieniaRoli = pobierajUprawnieniaRoli;
            _context = dbContext;
            _jednostkaPracy = jednostkaPracy;
        }



        public async Task<(bool Sukces, string Komunikat)> ZmienNazweRoli(string roleId, string nowaNazwa, string staraNazwa)
        {
            var rola = await _roleManager.FindByIdAsync(roleId);

            if (rola == null)
            {
                return (false, "Rola nie została znaleziona.");
            }

            if (_daneStartowe.Role.Contains(rola.Name))
            {
                return (false, "Nie można zmienić nazwy roli, która jest systemowa.");
            }

            if (rola.Name != staraNazwa)
            {
                return (false, "Rola została wcześniej zmieniona. Odśwież stronę i spróbuj ponownie.");
            }

            rola.Name = nowaNazwa;
            rola.NormalizedName = nowaNazwa.ToUpperInvariant();

            var wynik = await _roleManager.UpdateAsync(rola);

            return (wynik.Succeeded, wynik.Succeeded ? "Nazwa roli zmieniona pomyślnie." : "Nie udało się zmienić nazwy roli.");
        }


        public async Task<(bool Sukces, string Komunikat)> DodajRole(string nazwaRoli)
        {
            var istnieje = await _roleManager.RoleExistsAsync(nazwaRoli);

            if (istnieje)
            {
                return (false, "Taka rola już istnieje.");
            }

            var wynik = await _roleManager.CreateAsync(
                new IdentityRole(nazwaRoli)
            );

            return (wynik.Succeeded, wynik.Succeeded ? "Rola dodana pomyślnie." : "Nie udało się dodać roli.");
        }
        public async Task<bool> PolaczRoleZarzadzanaZZarzadzajacaAsync(string idRoliZarzadzajacej, string idRoliZarzadzanej)
        {
            

            var istnieje = await _context.UprawnieniaZarzadzaniaRola.AnyAsync(x =>
                        x.RolaZarzadzajacaId == idRoliZarzadzajacej &&
                        x.RolaZarzadzanaId == idRoliZarzadzanej);

            if (!istnieje)
            {
                _context.UprawnieniaZarzadzaniaRola.Add(new UprawnienieZarzadzaniaRola
                {
                    RolaZarzadzajacaId = idRoliZarzadzajacej,
                    RolaZarzadzanaId = idRoliZarzadzanej
                });
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<(bool Sukces,string Komunikat)> KontrolaDodawaRoliAsync(string iDzarzadzajacaRola, string nazwaNowejRoli)
        {
            await _jednostkaPracy.RozpocznijTransakcjeAsync();
            try
            {
                var tmp = await DodajRole(nazwaNowejRoli);
                if (!tmp.Sukces)
                {
                    await _jednostkaPracy.CofnijAsync();
                    return (false, tmp.Komunikat);
                }
                var zarzadzanaRola = await _roleManager.FindByNameAsync(nazwaNowejRoli);
                if (!await PolaczRoleZarzadzanaZZarzadzajacaAsync(iDzarzadzajacaRola, zarzadzanaRola.Id))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return (false, "Takie połączenie ról już istnieje.");
                }
                await _jednostkaPracy.ZatwierdzAsync();
                return (true, "Rola dodana pomyślnie.");
            }
            catch
            {
                await _jednostkaPracy.CofnijAsync();
                return (false, "Wystąpił błąd podczas dodawania roli.");
            }

        }


        public async Task<(bool Sukces, string Komunikat)> UsunRole(string roleId)
        {
            if (await _pobierajUprawnieniaRoli.CzyRolaJestUzywanaWHierarchiiAsync(roleId))
            {
                return (false, "Nie można usunąć roli, ponieważ jest częścią hierarchii zarządzania.");
            }
            var rola = await _roleManager.FindByIdAsync(roleId);

            if (rola == null)
            {
                return (false, "Rola nie została znaleziona.");
            }

            if (_daneStartowe.Role.Contains(rola.Name))
            {
                return (false, "Nie można usunąć roli, która jest systemowa.");
            }

            var wynik = await _roleManager.DeleteAsync(rola);

            return (
                wynik.Succeeded, wynik.Succeeded ? "Rola usunięta pomyślnie." : "Nie udało się usunąć roli."
            );
        }

       public (List<DaneRoli> RoleSystemowe, List<DaneRoli> PozostaleRole) PodzierRoleNaSystemoweIPozostale(ClaimsPrincipal user, List<DaneRoli> wszystkieDostempneRole)
        {
            var roleSystemowe = wszystkieDostempneRole
                .Where(r => _daneStartowe.Role.Contains(r.Nazwa))
                .ToList();
            var pozostaleRole = wszystkieDostempneRole
                .Where(r => !_daneStartowe.Role.Contains(r.Nazwa))
                .ToList();
            return (roleSystemowe, pozostaleRole);
        }

    }
}