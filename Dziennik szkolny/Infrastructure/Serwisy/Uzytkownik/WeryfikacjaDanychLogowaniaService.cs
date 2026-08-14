using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Uzytkownik
{
        
    public class WeryfikacjaDanychLogowaniaService : IWeryfikacjaDanychLogowania
    {
        private readonly UserManager<LoginUzytkownika> _userManager;


        public WeryfikacjaDanychLogowaniaService(UserManager<LoginUzytkownika> userManager)
        {
            _userManager = userManager;
        }
        public  async Task<bool> CzyIstniejeLoginAsync(string login)
        {
            var znormalizowanyLogin = _userManager.NormalizeName(login);


            bool istnieje = await _userManager.Users.AnyAsync(x => x.NormalizedUserName == znormalizowanyLogin);
            return istnieje;
        }



        public  async Task<bool> CzyIstniejeEmailAsync(string email)
        {
            var znormalizowanyEmail = _userManager.NormalizeEmail(email);


            bool istnieje = await _userManager.Users.AnyAsync(x => x.NormalizedEmail == znormalizowanyEmail);
            return istnieje;
        }
    }
}
