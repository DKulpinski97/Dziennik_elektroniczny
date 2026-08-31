using Dziennik_szkolny.Application.Interfejsy;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Mapery;
using Dziennik_szkolny.Application.Modele.Uzytkownik;
using Dziennik_szkolny.Application.Walidacja.Uzytkownik;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Infrastructure.DaneStartowe;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Dziennik_szkolny.Application.Serwisy.Uzytkownik
{
    public class ObslugaUzytkownika : IObslugaUzytkownika
    {
        private readonly IWeryfikacjaDanychLogowania _weryfikacjaDanychLogowania;
        private readonly IPobierajRole _pobierajRole;
        private readonly WalidacjaDanychUzytkownika _walidacjaDanychUzytkownika;
        private readonly IZarzadzajUzytkownikem _zarzadzajUzytkownikema;
        private readonly IPobierajUzytkownika _pobierajUzytkownika;
        private readonly IJednostkaPracy _jednostkaPracy;
        private readonly MapowanieUzytkownika _mapowanieUzytkownika = new MapowanieUzytkownika();
        private readonly DaneStartowe _daneStartowe;
        private readonly IPobierajUprawnieniaRoli _pobierajUprawnieniaRoli;


        public ObslugaUzytkownika(
            IWeryfikacjaDanychLogowania weryfikacjaDanychLogowania,
            IPobierajRole pobierajRole,
            IZarzadzajUzytkownikem zarzadzajUzytkownikema,
            IPobierajUzytkownika pobierajUzytkownika,
            WalidacjaDanychUzytkownika walidacjaDanychUzytkownika,
            IJednostkaPracy jednostkaPracy,
            DaneStartowe daneStartowe,
            IPobierajUprawnieniaRoli pobierajUprawnieniaRoli)
        {
            _weryfikacjaDanychLogowania = weryfikacjaDanychLogowania;
            _pobierajRole = pobierajRole;
            _walidacjaDanychUzytkownika = walidacjaDanychUzytkownika;
            _zarzadzajUzytkownikema = zarzadzajUzytkownikema;
            _pobierajUzytkownika = pobierajUzytkownika;
            _jednostkaPracy = jednostkaPracy;
            _daneStartowe = daneStartowe;
            _pobierajUprawnieniaRoli = pobierajUprawnieniaRoli;
        }


        public async Task<(string Komunikat, UzytkownikaViewModel Uzytkownik, bool CzyUdane)> DodajUzytkownikaAsync(UzytkownikaViewModel model)
        {
            // TODO: W wersji wdrożeniowej zastosować wymagania silnego hasła
            // zgodne z konfiguracją ASP.NET Core Identity.
           /* if (!_walidacjaDanychUzytkownika.CzyHasloPrawidlowe(model.Haslo))
            {
                return ("Hasło musi zawierać co najmniej 3 znaki.", model, false);
            }*/
            // Walidacja telefonu
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyTelefon(model.Telefon))
            {
                return ("Telefon musi zawierać dokładnie 9 cyfr.", model, false);
            }


            // Walidacja PESEL
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyPesel(model.Pesel))
            {
                return ("Podany numer PESEL jest niepoprawny.", model, false);
            }


            // Sprawdzenie loginu
            if (await _weryfikacjaDanychLogowania.CzyIstniejeLoginAsync(model.Login))
            {
                return ("Użytkownik o takim loginie już istnieje.", model, false);
            }


            // Sprawdzenie emaila
            if (await _weryfikacjaDanychLogowania.CzyIstniejeEmailAsync(model.Email))
            {
                return ("Użytkownik o takim emailu już istnieje.", model, false);
            }


            if (!await _pobierajRole.CzyIstniejaRoleAsync(model.WybraneRole))
            {
                return ("Nie znaleziono jednej lub więcej wybranych ról.", model, false);
            }

            await _jednostkaPracy.RozpocznijTransakcjeAsync();

            try
            {
                // Dodanie konta
                if (!await _zarzadzajUzytkownikema.DodajUzytkownikaAsync(model))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się dodać użytkownika.", model, false);
                }
                var nowyUzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoLoginieAsync(model.Login);
                //Dodaj Role użytkownikowi
                if (!await _zarzadzajUzytkownikema.DodajRoleUzytkownikowiAsync(nowyUzytkownik, model.WybraneRole))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się przypisać ról.", model, false);
                }
                // Dodanie informacji użytkownika
                if (!await _zarzadzajUzytkownikema.DodajInformacjeUzytkownikaAsync(nowyUzytkownik, model))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się dodać informacji o użytkowniku.", model, false);
                }
                await _jednostkaPracy.ZatwierdzAsync();


                return ("Użytkownik dodany", model, true);
            }
            catch (Exception ex)
            {
                await _jednostkaPracy.CofnijAsync();

                return ($"Błąd: {ex.Message} Inner: {ex.InnerException?.Message}", model, false);
            }
        }

        public async Task<(string Komunikat, UzytkownikaViewModel Uzytkownik, bool CzyUdane)> EdytujUzytkownikaAsync(UzytkownikaViewModel uzytkownikaViewModel)
        {
            //======================Sprawdzanie podstawowych danych=========================
            // TODO: W wersji wdrożeniowej zastosować wymagania silnego hasła
            // zgodne z konfiguracją ASP.NET Core Identity.
            bool zmianaHasla = !string.IsNullOrWhiteSpace(uzytkownikaViewModel.Haslo);
            /*if (zmianaHasla && !_walidacjaDanychUzytkownika.CzyHasloPrawidlowe(uzytkownikaViewModel.Haslo))
            {
                return ("Hasło musi zawierać co najmniej 3 znaki.", uzytkownikaViewModel, false);

            }*/

            // Walidacja telefonu
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyTelefon(uzytkownikaViewModel.Telefon))
            {
                return ("Telefon musi zawierać dokładnie 9 cyfr.", uzytkownikaViewModel, false);
            }


            // Walidacja PESEL
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyPesel(uzytkownikaViewModel.Pesel))
            {
                return ("Podany numer PESEL jest niepoprawny.", uzytkownikaViewModel, false);
            }


            // Sprawdzenie loginu
            if (await _weryfikacjaDanychLogowania.CzyIstniejeInnyLoginAsync(uzytkownikaViewModel.Login, uzytkownikaViewModel.idUzytkownika))
            {
                return ("Użytkownik o takim loginie już istnieje.", uzytkownikaViewModel, false);
            }


            // Sprawdzenie emaila
            if (await _weryfikacjaDanychLogowania.CzyIstniejeInnyEmailAsync(uzytkownikaViewModel.Email, uzytkownikaViewModel.idUzytkownika))
            {
                return ("Użytkownik o takim emailu już istnieje.", uzytkownikaViewModel, false);
            }


            if (!await _pobierajRole.CzyIstniejaRoleAsync(uzytkownikaViewModel.WybraneRole))
            {
                return ("Nie znaleziono jednej lub więcej wybranych ról.", uzytkownikaViewModel, false);
            }

            await _jednostkaPracy.RozpocznijTransakcjeAsync();
            try
            {
                // =====================
                // Aktualizacja Identity
                // =====================
                // Dodanie konta
                if (!await _zarzadzajUzytkownikema.EdytujUzytkownikaAsync(uzytkownikaViewModel, zmianaHasla))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zaktualizować danych logowania.", uzytkownikaViewModel, false);
                }

                // =====================
                // Aktualizacja danych osobowych
                // =====================

                var informacje = await _pobierajUzytkownika.PobierzInformacjeUzytkownikaPoIdLoginu(uzytkownikaViewModel.idUzytkownika);


                if (informacje == null)
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie znaleziono danych użytkownika.", uzytkownikaViewModel, false);
                }
                _mapowanieUzytkownika.MapujNaInformacjeUzytkownika(informacje, uzytkownikaViewModel);


                if (!await _zarzadzajUzytkownikema.EdytujInformacjeUzytkownikaAsync(informacje))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie znaleziono danych użytkownika.", uzytkownikaViewModel, false);
                }


                // =====================
                // Aktualizacja ról
                // =====================

                //var obecneRole = await _pobierajRole.PobierzRoleUzytkownikaPoLoginieAsync(uzytkownikaViewModel.Login);

                LoginUzytkownika uzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoLoginieAsync(uzytkownikaViewModel.Login);

                if (!await _zarzadzajUzytkownikema.UsunWszystkieRoleUzytkownikowiAsync(uzytkownikaViewModel.Login))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zmienić ról.", uzytkownikaViewModel, false);
                }


                var noweRole = await _pobierajRole.PobierzNazwyRolPoIdAsync(uzytkownikaViewModel.WybraneRole);


                if (!await _zarzadzajUzytkownikema.DodajRoleUzytkownikowiAsync(uzytkownik, noweRole))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zmienić ról lub nie wybrano roli", uzytkownikaViewModel, false);
                }




                await _jednostkaPracy.ZatwierdzAsync();

                return ("Dane użytkownika zostały zmienione.", uzytkownikaViewModel, true);
            }
            catch
            {
                await _jednostkaPracy.CofnijAsync();

                return ("Wystąpił błąd podczas edycji użytkownika.", uzytkownikaViewModel, false);
            }
        }

        public async Task<ListaUzytkownikow> PodzielUzytkownikowNaRodzicowIPracownikowAsync(ClaimsPrincipal user)
        {
            var wynik = new ListaUzytkownikow();

            var roleUzytkownika = await _pobierajRole.PobierzRoleZalogowanegoUzytkownikaAsync(user);

            var roleDoZarzadzania = await _pobierajUprawnieniaRoli.PobierzRoleKtorymiMozeZarzadzacAsync(roleUzytkownika);

            var uzytkownicy =await _pobierajUzytkownika.PobierzUzytkownikowPoRolachAsync(roleDoZarzadzania);

            var informacje =await _pobierajUzytkownika.PobierzWszystkieInformacjeOUzrzytkownikach();

            var informacjePoId =informacje.ToDictionary(x => x.IdUzytkownika);

            foreach (var uzytkownik in uzytkownicy)
            {
                if (!informacjePoId.TryGetValue(uzytkownik.Id, out var dane))
                {
                    continue;
                }

                var element = new SelectListItem
                {
                    Value = uzytkownik.Id,
                    Text = $"{dane.Imie} {dane.Nazwisko}"
                };

                if (uzytkownik.Role.Any(r =>
                    _daneStartowe.RolePracownika.Contains(r)))
                {
                    wynik.Pracownicy.Add(element);
                }
                else if (uzytkownik.Role.Contains("Rodzic"))
                {
                    wynik.Rodzice.Add(element);
                }
            }

            return wynik;
        }
    }
}