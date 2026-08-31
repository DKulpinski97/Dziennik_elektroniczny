using Dziennik_szkolny.Models;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajRoleStartowe
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DaneStartowe _daneStartowe;

        public DodajRoleStartowe(RoleManager<IdentityRole> roleManager, DaneStartowe daneStartowe)
        {
            _roleManager = roleManager;
            _daneStartowe = daneStartowe;
        }

        public async Task DodajRoleStartoweAsync()
        {
          

            foreach (string nazwaRoli in _daneStartowe.Role)
            {
                var istnieje = await _roleManager.RoleExistsAsync(nazwaRoli);

                if (!istnieje)
                {
                    var rola = new IdentityRole(nazwaRoli);
                    await _roleManager.CreateAsync(rola);
                }
            }
        }
    }
}
