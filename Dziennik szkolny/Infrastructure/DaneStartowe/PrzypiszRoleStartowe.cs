using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class PrzypiszRoleStartowe
    {
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DaneStartowe _daneStartowe;

        public PrzypiszRoleStartowe(UserManager<LoginUzytkownika> userManager,RoleManager<IdentityRole> roleManager,DaneStartowe daneStartowe)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _daneStartowe = daneStartowe;
        }
        public async Task PrzypiszRoleStartoweAsync()
        {
            
            //lista roli po kolei przypisywanych do użytkowników startowych

            

            //Pobieranie ID Roli na podstawie nazwy roli i przypisanie ich do listy IdRoli
            List<List<string>> IdRoli = new List<List<string>>();

            foreach (var PojedynczyZakresRoli in _daneStartowe.PrzypisanieRoli)
            {
                List<string> listaId = new List<string>();

                foreach (var nazwaRoli in PojedynczyZakresRoli)
                {
                    var rola = await _roleManager.FindByNameAsync(nazwaRoli);

                    if (rola != null)
                    {
                        listaId.Add(rola.Id);
                    }
                }

                IdRoli.Add(listaId);
            }
            //przypisuje role do użytkownika
            for (var i = 0; i < _daneStartowe.PrzypisanieRoli.Count; i++)
            {
                var nazwaRoli = _daneStartowe.PrzypisanieRoli[i];
                var userLogin = _daneStartowe.Loginy[i][0];
                var user = await _userManager.FindByNameAsync(userLogin);

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
