using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajLoginyStartowe
    {
        private readonly UserManager<LoginUzytkownika> _userManager;

        public DodajLoginyStartowe(
            UserManager<LoginUzytkownika> userManager)
        {
            _userManager = userManager;
        }

        public async Task DodajLoginyStartoweAsync()
        {
            List<string[]> loginy =
            [
                new[] { "Admin", "Admin@gmail.com", "Admin" },
                new[] { "Dyrektor", "Dyrektor@gmail.com", "Dyrektor" },
                new[] { "Sekretarka", "Sekretarka@gmail.com", "Sekretarka" },

                new[] { "Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1" },
                new[] { "Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2" },
                new[] { "Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3" },

                new[] { "Rodzic1", "Rodzic1@gmail.com", "Rodzic1" },
                new[] { "Rodzic2", "Rodzic2@gmail.com", "Rodzic2" },
                new[] { "Rodzic3", "Rodzic3@gmail.com", "Rodzic3" }
            ];

            foreach (var dane in loginy)
            {
                string login = dane[0];
                string email = dane[1];
                string haslo = dane[2];

                var istnieje = await _userManager.FindByNameAsync(login);

                if (istnieje != null)
                {
                    continue;
                }

                var nowyLogin = new LoginUzytkownika
                {
                    UserName = login,
                    Email = email
                };

                var wynik = await _userManager.CreateAsync(
                    nowyLogin,
                    haslo);

                if (!wynik.Succeeded)
                {
                    var bledy = string.Join(
                        "; ",
                        wynik.Errors.Select(x =>
                            $"{x.Code}: {x.Description}"));

                    throw new InvalidOperationException(
                        $"Nie udało się utworzyć użytkownika '{login}'. " +
                        $"Błędy: {bledy}");
                }
            }
        }
    }
}