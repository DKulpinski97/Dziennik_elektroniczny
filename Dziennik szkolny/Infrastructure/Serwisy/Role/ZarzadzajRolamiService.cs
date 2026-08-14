using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Application.Interfejsy.Role;
namespace Dziennik_szkolny.Infrastructure.Serwisy.Role
{
    public class ZarzadzajRolamiService : IZarzadzajRolami
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public ZarzadzajRolamiService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
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
            rola.NormalizedName = nowaNazwa.ToUpperInvariant();

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