using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dziennik_szkolny.Application.Mapery
{
    public class MapowanieUzytkownika
    {
        private readonly MapowanieRoli _mapowanieRoli = new MapowanieRoli();

        public EdytujUzytkownikaViewModel MapujNaUzytkownikaViewModel(InformacjeUzytkownik informacjeUzytkownik, LoginUzytkownika login, List<string> wybraneRole, List<SelectListItem> dostepneRole)
        {
            return new EdytujUzytkownikaViewModel
            {
                Login = login.UserName,
                Email = login.Email,
                Haslo = null,
                Imie = informacjeUzytkownik.Imie,
                Nazwisko = informacjeUzytkownik.Nazwisko,
                Pesel = informacjeUzytkownik.Pesel,
                Telefon = informacjeUzytkownik.Telefon,
                Miasto = informacjeUzytkownik.Miasto,
                Ulica = informacjeUzytkownik.Ulica,
                NrMieszkania = informacjeUzytkownik.NrMieszkania,
                DostepneRole = dostepneRole,
                WybraneRole = wybraneRole,
                IdUzytkownika = login.Id
            };
        }
        public void MapujNaInformacjeUzytkownika(InformacjeUzytkownik informacjeUzytkownik, SzkieletUzytkownikaViewModel uzytkownikaViewModel)
        {
            informacjeUzytkownik.Imie = uzytkownikaViewModel.Imie;
            informacjeUzytkownik.Nazwisko = uzytkownikaViewModel.Nazwisko;
            informacjeUzytkownik.Pesel = uzytkownikaViewModel.Pesel;
            informacjeUzytkownik.Telefon = uzytkownikaViewModel.Telefon;
            informacjeUzytkownik.Miasto = uzytkownikaViewModel.Miasto;
            informacjeUzytkownik.Ulica = uzytkownikaViewModel.Ulica;
            informacjeUzytkownik.NrMieszkania = uzytkownikaViewModel.NrMieszkania;
        }
        public DodajUzytkownikaViewModel MapujDostepneRoleVievModel(List<DaneRoli> dostepneRole)
        {
            var wszystkieRole = _mapowanieRoli.MapujRoleNaSelectList(dostepneRole);
            return new DodajUzytkownikaViewModel
            {
                DostepneRole = wszystkieRole
            };
        }
        public void MapujDostepneRoleDoIStniejacegoVievModel(SzkieletUzytkownikaViewModel uzytkownikaViewModel, List<DaneRoli> dostepneRole)
        {
            var wszystkieRole = _mapowanieRoli.MapujRoleNaSelectList(dostepneRole);

            uzytkownikaViewModel.DostepneRole = wszystkieRole;
        }
    }
}
