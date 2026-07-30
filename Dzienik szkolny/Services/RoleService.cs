using Dzienik_szkolny.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dzienik_szkolny.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<IdentityRole>> PobierzRole()
        {
            return await _roleManager.Roles
    .Where(x => x.Name != "Brak roli" &&
                x.Name != "Uczeń")
    .OrderBy(x => x.Name)
    .ToListAsync();
        }
        public async Task<bool> ZmienNazweRoli(string roleId, string nowaNazwa, string staraNazwa)
        {
            var rola = await _roleManager.FindByIdAsync(roleId);

            if (rola == null)
            {
                return false;
            }
            if (rola.Name != staraNazwa)
            {
                return false;
            }
            rola.Name = nowaNazwa;
            rola.NormalizedName = nowaNazwa.ToUpper();

            var wynik = await _roleManager.UpdateAsync(rola);

            return wynik.Succeeded;
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


        public async Task<bool> UsunRole(string roleId)
        {
            var rola = await _roleManager.FindByIdAsync(roleId);

            if (rola == null)
            {
                return false;
            }

            var wynik = await _roleManager.DeleteAsync(rola);

            return wynik.Succeeded;
        }
    }
}