using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel.Uzytkownik;

namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IZarzadzajUzytkownikem
    {
        Task<bool> DodajUzytkownikaAsync(DodajUzytkownikaViewModel model);
        Task<bool> EdytujUzytkownikaAsync(EdytujUzytkownikaViewModel model, bool zmienHaslo);
        Task<bool> EdytujInformacjeUzytkownikaAsync(InformacjeUzytkownik informacjeUzytkownika);
        Task<bool> DodajRoleUzytkownikowiAsync(LoginUzytkownika uzytkownik, List<string> NazwaRoli);
        Task<bool> DodajInformacjeUzytkownikaAsync(LoginUzytkownika uzytkownik, SzkieletUzytkownikaViewModel model);
        Task<bool> UsunWszystkieRoleUzytkownikowiAsync(string login);
        Task<(bool Stan, string komunikat)> UsunUzytkownika(string idUzytkownika);
    }
}
