using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class PrzypiszRoleStartowe
    {
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PrzypiszRoleStartowe(UserManager<LoginUzytkownika> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task PrzypiszRoleStartoweAsync()
        {
            //lista logionów dla wglądu w kodzie i by prawidłowo przypisać role do użytkowników startowych
            List<string[]> loginy = new List<string[]>
            {
                new string[] { "Admin", "Admin@gmail.com", "Admin" },
                new string[] { "Dyrektor", "Dyrektor@gmail.com", "Dyrektor" },
                new string[] { "Sekretarka", "Sekretarka@gmail.com", "Sekretarka" },

                new string[] { "Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1" },
                new string[] { "Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2" },
                new string[] { "Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3" },

                new string[] { "Rodzic1", "Rodzic1@gmail.com", "Rodzic1" },
                new string[] { "Rodzic2", "Rodzic2@gmail.com", "Rodzic2" },
                new string[] { "Rodzic3", "Rodzic3@gmail.com", "Rodzic3" }
            };
            //lista roli po kolei przypisywanych do użytkowników startowych

            List<List<string>> NazwyRoli = new List<List<string>>
            {
                new List<string> { "Admin" },
                new List<string> { "Dyrektor" },
                new List<string> { "Sekretarka" },
                new List<string> { "Nauczyciel","Rodzic" },
                new List<string> { "Nauczyciel" },
                new List<string> { "Nauczyciel" },
                new List<string> { "Rodzic" },
                new List<string> { "Rodzic" },
                new List<string> { "Rodzic" }
            };

            //Pobieranie ID Roli na podstawie nazwy roli i przypisanie ich do listy IdRoli
            List<List<string>> IdRoli = new List<List<string>>();

            foreach (var PojedynczyZakresRoli in NazwyRoli)
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
            for (var i = 0; i < NazwyRoli.Count; i++)
            {
                var nazwaRoli = NazwyRoli[i];
                var userLogin = loginy[i][0];
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
