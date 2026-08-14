using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Uzytkownik
{
    public class PobierajUzytkownikaService : IPobierajUzytkownika
    {
        readonly private UserManager<LoginUzytkownika> _userManager;
        public PobierajUzytkownikaService(UserManager<LoginUzytkownika> userManager)
        {
            _userManager = userManager;
        }

        public async Task<LoginUzytkownika> PobierzUzytkownikaPoLoginieAsync(string login)
        {
            return await _userManager.FindByNameAsync(login);
        }
    }
}