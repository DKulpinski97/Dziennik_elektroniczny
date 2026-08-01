using Dzienik_szkolny.ViewModels;

namespace Dzienik_szkolny.Services.Interfaces
{
    public interface IUzytkownikService
    {
        public Task<(string Komunikat, DodajUzytkownikaViewModel Uzytkownik)> DodajUzytkownikaAsync(DodajUzytkownikaViewModel dodajUzytkownikaViewModel);
    }
}
