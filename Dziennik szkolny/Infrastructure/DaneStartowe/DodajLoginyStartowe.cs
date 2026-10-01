using Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych;
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
                var dto = new DTOLogin(dane.Login, dane.Email, dane.Haslo);


                var istnieje = await _userManager.FindByNameAsync(dane.Login);

                if (istnieje != null)
                {
                    continue;
                }

                var nowyLogin = dto.DoEncja();


                var wynik = await _userManager.CreateAsync(nowyLogin,dane.Haslo);

                if (!wynik.Succeeded) 
                { 
                    var bledy = string.Join("; ", wynik.Errors.Select(x => $"{x.Code}: {x.Description}")); 
                    throw new InvalidOperationException($"Nie udało się utworzyć użytkownika '{dto.Login}'. " + $"Błędy: {bledy}"); }
            }
        }
    }
}