using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel;

namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IZarzadzajUzytkownikem
    {
        Task<bool> DodajUzytkownikaAsync(UzytkownikaViewModel model);
        Task<bool> EdytujUzytkownikaAsync(UzytkownikaViewModel model, bool zmienHaslo);
        Task<bool> EdytujInformacjeUzytkownikaAsync(InformacjeUzytkownik informacjeUzytkownika);
        Task<bool> DodajRoleUzytkownikowiAsync(LoginUzytkownika uzytkownik, List<string> NazwaRoli);
        Task<bool >DodajInformacjeUzytkownikaAsync(LoginUzytkownika uzytkownik, UzytkownikaViewModel model);
        Task<bool> UsunWszystkieRoleUzytkownikowiAsync(string login);
        Task<bool> UsunUzytkownika(string idUzytkownika);
    }
}
