using Dziennik_szkolny.Models;
using Dziennik_szkolny.ViewModel;

namespace Dziennik_szkolny.Services.Interfaces
{
    public interface IUzytkownikService
    {
        public Task<(string Komunikat, UzytkownikaViewModel Uzytkownik,bool CzyUdane)> DodajUzytkownikaAsync(UzytkownikaViewModel dodajUzytkownikaViewModel);
        public Task<UzytkownikaViewModel> PrzygotujDaneDoEdycjiAsync(InformacjeUzytkownik informacjeUzytkownik,string iDUser);
        public Task<(bool Sukces, string Komunikat)> EdytujUzytkownikaAsync(UzytkownikaViewModel model);
    }
}
