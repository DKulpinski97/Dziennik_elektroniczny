using Dziennik_szkolny.Models;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajRoleStartowe
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public DodajRoleStartowe(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task DodajRoleStartoweAsync()
        {
            List<string> role = new()
        {
            "Admin",
            "Nauczyciel",
            "Uczen",
            "Brak roli",
            "Dyrektor",
            "ViceDyrektor",
            "Sekretarka",
            "Rodzic"
        };

            foreach (string nazwaRoli in role)
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
