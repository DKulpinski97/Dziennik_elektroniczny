using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class PrzypiszRoleStartowe
    {
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DaneStartowe _daneStartowe;

        public PrzypiszRoleStartowe(UserManager<LoginUzytkownika> userManager, RoleManager<IdentityRole> roleManager, DaneStartowe daneStartowe)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _daneStartowe = daneStartowe;
        }
        public async Task PrzypiszRoleStartoweAsync()
        {

            //przypisuje role do użytkownika
            for (var i = 0; i < _daneStartowe.PrzypisanieRoli.Count; i++)
            {
                var nazwaRoli = _daneStartowe.PrzypisanieRoli[i];
                var userLogin = _daneStartowe.Loginy[i][0];
                var user = await _userManager.FindByNameAsync(userLogin);

                if (user == null)
                {
                    continue;
                }
                foreach (var PojedynczaRola in nazwaRoli)
                {
                    bool posiadaRole = await _userManager.IsInRoleAsync(user, PojedynczaRola);

                    if (!posiadaRole)
                    {
                        await _userManager.AddToRoleAsync(user, PojedynczaRola);
                    }
                }
            }
        }
    }
}
