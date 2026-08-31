using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajLoginyStartowe
    {
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly DaneStartowe _daneStartowe;

        public DodajLoginyStartowe(
            UserManager<LoginUzytkownika> userManager,DaneStartowe daneStartowe)
        {
            _userManager = userManager;
            _daneStartowe = daneStartowe;
        }

        public async Task DodajLoginyStartoweAsync()
        {
           

            foreach (var dane in _daneStartowe.Loginy)
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