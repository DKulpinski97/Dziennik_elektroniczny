using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel;

namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IZarzadzajUzytkownikem
    {
        Task<bool> DodajUzytkownikaAsync(UzytkownikaViewModel model);
        Task<bool> DodajRoleUzytkownikowiAsync(LoginUzytkownika uzytkownik, List<string> idRoli);
        Task<bool >DodajInformacjeUzytkownikaAsync(LoginUzytkownika uzytkownik, UzytkownikaViewModel model);
    }
}
