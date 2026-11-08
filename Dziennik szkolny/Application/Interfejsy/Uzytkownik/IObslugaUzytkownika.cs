using Dziennik_szkolny.Application.Modele.Uzytkownik;
using Dziennik_szkolny.ViewModel.Uzytkownik;
using System.Security.Claims;

namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IObslugaUzytkownika
    {
        Task<(string Komunikat, bool CzyUdane)> DodajUzytkownikaAsync(DodajUzytkownikaViewModel dodajUzytkownikaViewModel, ClaimsPrincipal zalogowanyUzytkownik);
        Task<(string Komunikat, bool CzyUdane)> EdytujUzytkownikaAsync(EdytujUzytkownikaViewModel edytujUzytkownikaViewModel, ClaimsPrincipal zalogowanyUzytkownik);
        Task<ListaUzytkownikow> PodzielUzytkownikowNaRodzicowIPracownikowAsync(ClaimsPrincipal user);
    }
}
