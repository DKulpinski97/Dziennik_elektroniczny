using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Modele;
using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Role
{
    public class PobierajRoleService:IPobierajRole

    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<LoginUzytkownika> _userManager;
        public PobierajRoleService(RoleManager<IdentityRole> roleManager, UserManager<LoginUzytkownika> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<List<DaneRoli>> PobierzRole()
        {
            return await _roleManager.Roles
                .Where(x => x.Name != "Brak roli" &&
                            x.Name != "Uczeń")
                .OrderBy(x => x.Name)
                .Select(x => new DaneRoli
                {
                    Id = x.Id,
                    Nazwa = x.Name
                })
                .ToListAsync();
        }
        public async Task<bool> CzyIstniejaRoleAsync(List<string> idRoli)
        {
            if (idRoli == null || !idRoli.Any())
            {
                return true;
            }


            foreach (var idRoliItem in idRoli)
            {
                var rola = await _roleManager.FindByIdAsync(idRoliItem);


                if (rola == null)
                {
                    return false;
                }
            }


            return true;
        }
        public async Task<IdentityRole?> PobierzRolePoIdAsync(string idRoli)
        {
            return await _roleManager.FindByIdAsync(idRoli);
        }
        public async Task<IList<string>> PobierzRoleUzytkownikaPoLoginieAsync(string login)
        {
            var uzytkownik = await _userManager.FindByNameAsync(login);

            if (uzytkownik == null)
            {
                return new List<string>();
            }

            return await _userManager.GetRolesAsync(uzytkownik);
        }
        public  async Task<List<string>> PobierzNazwyRolPoIdAsync(List<string> idRol)
        {
            if (idRol == null || idRol.Count == 0)
            {
                return new List<string>();
            }

            return await _roleManager.Roles.Where(x => idRol.Contains(x.Id)).Select(x => x.Name).ToListAsync();
        }
    }

}
