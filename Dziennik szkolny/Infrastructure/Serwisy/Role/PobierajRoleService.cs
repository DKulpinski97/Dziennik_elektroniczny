using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Role
{
    public class PobierajRoleService:IPobierajRole

    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public PobierajRoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
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
    }

}
