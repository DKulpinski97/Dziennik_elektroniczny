using Microsoft.AspNetCore.Identity;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Infrastructure.DaneStartowe;
namespace Dziennik_szkolny.Infrastructure.Serwisy.Role
{
    public class ZarzadzajRolamiService : IZarzadzajRolami
    {
        private readonly IPobierajUprawnieniaRoli _pobierajUprawnieniaRoli;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Dziennik_szkolny.Infrastructure.DaneStartowe.DaneStartowe _daneStartowe;
        public ZarzadzajRolamiService(RoleManager<IdentityRole> roleManager, Dziennik_szkolny.Infrastructure.DaneStartowe.DaneStartowe daneStartowe, IPobierajUprawnieniaRoli pobierajUprawnieniaRoli)
        {
            _roleManager = roleManager;
            _daneStartowe = daneStartowe;
            _pobierajUprawnieniaRoli = pobierajUprawnieniaRoli;
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


        public async Task<bool> DodajRole(string nazwaRoli)
        {
            var istnieje = await _roleManager.RoleExistsAsync(nazwaRoli);

            if (istnieje)
            {
                return false;
            }

            var wynik = await _roleManager.CreateAsync(
                new IdentityRole(nazwaRoli)
            );

            return wynik.Succeeded;
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
                wynik.Succeeded,wynik.Succeeded? "Rola usunięta pomyślnie.": "Nie udało się usunąć roli."
            );
        }

    }
}