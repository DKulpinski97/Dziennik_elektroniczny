using Dziennik_szkolny.Application.Modele.Uzytkownik;
using Dziennik_szkolny.ViewModel;
using System.Security.Claims;

namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IObslugaUzytkownika
    {
        Task<(string Komunikat, UzytkownikaViewModel Uzytkownik, bool CzyUdane)> DodajUzytkownikaAsync(UzytkownikaViewModel uzytkownikaViewModel);
        Task<(string Komunikat, UzytkownikaViewModel Uzytkownik, bool CzyUdane)> EdytujUzytkownikaAsync(UzytkownikaViewModel uzytkownikaViewModel);
        Task<ListaUzytkownikow> PodzielUzytkownikowNaRodzicowIPracownikowAsync(ClaimsPrincipal user);
    }
}
