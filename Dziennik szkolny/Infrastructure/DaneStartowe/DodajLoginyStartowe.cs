using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajLoginyStartowe
    {
        private readonly UserManager<LoginUzytkownika> _userManager;
        public DodajLoginyStartowe(UserManager<LoginUzytkownika> userManager)
        {
            _userManager = userManager;
        }
        public async Task DodajLoginyStartoweAsync()
        {
            //lista logionów
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
            //Dodawnie użytkownika
            foreach (var dane in loginy)
            {
                var istnieje = await _userManager.FindByNameAsync(dane[0]);

                if (istnieje == null)
                {
                    var nowyLogin = new LoginUzytkownika
                    {
                        UserName = dane[0],
                        Email = dane[1]
                    };


                    var wynik = await _userManager.CreateAsync(
                        nowyLogin,
                        dane[2]);
                }
            }
        }
    }
}
