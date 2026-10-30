using Dziennik_szkolny.Application.Interfejsy;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Mapery;
using Dziennik_szkolny.Application.Modele.Uzytkownik;
using Dziennik_szkolny.Application.Walidacja.Uzytkownik;
using Dziennik_szkolny.Domain;
using Dziennik_szkolny.Infrastructure.DaneStartowe;
using Dziennik_szkolny.ViewModel.Uzytkownik;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Security.Claims;

namespace Dziennik_szkolny.Application.Serwisy.Uzytkownik
{
    public class ObslugaUzytkownika : IObslugaUzytkownika
    {
        private readonly IWeryfikacjaDanychLogowania _weryfikacjaDanychLogowania;
        private readonly IPobierajRole _pobierajRole;
        private readonly WalidacjaDanychUzytkownika _walidacjaDanychUzytkownika;
        private readonly IZarzadzajUzytkownikem _zarzadzajUzytkownikiem;
        private readonly IPobierajUzytkownika _pobierajUzytkownika;
        private readonly IJednostkaPracy _jednostkaPracy;
        private readonly MapowanieUzytkownika _mapowanieUzytkownika = new MapowanieUzytkownika();
        private readonly DaneStartowe _daneStartowe;
        private readonly IPobierajUprawnieniaRoli _pobierajUprawnieniaRoli;
        private readonly ILogger<ObslugaUzytkownika> _logger;
        private static readonly StringComparer comparer =StringComparer.Create(new CultureInfo("pl-PL"), ignoreCase: false);

        public ObslugaUzytkownika(
            IWeryfikacjaDanychLogowania weryfikacjaDanychLogowania,
            IPobierajRole pobierajRole,
            IZarzadzajUzytkownikem zarzadzajUzytkownikema,
            IPobierajUzytkownika pobierajUzytkownika,
            WalidacjaDanychUzytkownika walidacjaDanychUzytkownika,
            IJednostkaPracy jednostkaPracy,
            DaneStartowe daneStartowe,
            IPobierajUprawnieniaRoli pobierajUprawnieniaRoli,
            ILogger<ObslugaUzytkownika> logger)
        {
            _weryfikacjaDanychLogowania = weryfikacjaDanychLogowania;
            _pobierajRole = pobierajRole;
            _walidacjaDanychUzytkownika = walidacjaDanychUzytkownika;
            _zarzadzajUzytkownikiem = zarzadzajUzytkownikema;
            _pobierajUzytkownika = pobierajUzytkownika;
            _jednostkaPracy = jednostkaPracy;
            _daneStartowe = daneStartowe;
            _pobierajUprawnieniaRoli = pobierajUprawnieniaRoli;
            _logger = logger;
        }

        public async Task<(string Komunikat, bool CzyUdane)> DodajUzytkownikaAsync(DodajUzytkownikaViewModel dodajUzytkownikaViewModel, ClaimsPrincipal zalogowanyUzytkownik)
        {
            // Sprawdzenie czy istnieją role o podanych identyfikatorach
            if (!await _pobierajRole.CzyIstniejaRoleAsync(dodajUzytkownikaViewModel.WybraneRole))
            {
                return ("Nie znaleziono jednej lub więcej wybranych ról.", false);
            }
            // Sprawdzanie hasła zgodne z konfiguracją ASP.NET Core Identity.
            var sprawdzenie = _walidacjaDanychUzytkownika.CzyHasloPrawidlowe(dodajUzytkownikaViewModel.Haslo);
            if (!sprawdzenie.wynik)
            {
                return (sprawdzenie.wiadomosc, false);
            }
            // Walidacja telefonu
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyTelefon(dodajUzytkownikaViewModel.Telefon))
            {
                return ("Telefon musi zawierać dokładnie 9 cyfr.", false);
            }
            // Walidacja PESEL
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyPesel(dodajUzytkownikaViewModel.Pesel))
            {
                return ("Podany numer PESEL jest niepoprawny.", false);
            }
            // Sprawdzenie loginu
            if (await _weryfikacjaDanychLogowania.CzyIstniejeLoginAsync(dodajUzytkownikaViewModel.Login))
            {
                return ("Użytkownik o takim loginie już istnieje.", false);
            }
            // Sprawdzenie PESEL
            if (await _pobierajUzytkownika.CzyPeselIstniejeAsync(dodajUzytkownikaViewModel.Pesel))
            {
                return ("Użytkownik o takim PESELu już istnieje.", false);
            }
            // Sprawdzenie emaila
            if (await _weryfikacjaDanychLogowania.CzyIstniejeEmailAsync(dodajUzytkownikaViewModel.Email))
            {
                return ("Użytkownik o takim emailu już istnieje.", false);
            }
            var wybraneNazwyRol = await _pobierajRole.PobierzNazwyRolPoIdAsync(dodajUzytkownikaViewModel.WybraneRole);
            var roleZalogowanego = await _pobierajRole.PobierzRoleZalogowanegoUzytkownikaAsync(zalogowanyUzytkownik);
            var roleDoZarzadzania = await _pobierajUprawnieniaRoli.PobierzNazwyRolKtorymiMozeZarzadzacAsync(roleZalogowanego);

            if (wybraneNazwyRol.Any(rola => !roleDoZarzadzania.Contains(rola)))
            {
                return ("Nie masz uprawnień do nadania jednej lub więcej wybranych ról.", false);
            }

            await _jednostkaPracy.RozpocznijTransakcjeAsync();

            try
            {
                // Dodanie konta
                if (!await _zarzadzajUzytkownikiem.DodajUzytkownikaAsync(dodajUzytkownikaViewModel))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się dodać użytkownika.", false);
                }

                var nowyUzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoLoginieAsync(dodajUzytkownikaViewModel.Login);
                //Dodaj Role użytkownikowi
                if (!await _zarzadzajUzytkownikiem.DodajRoleUzytkownikowiAsync(nowyUzytkownik, wybraneNazwyRol))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się przypisać ról.", false);
                }
                // Dodanie informacji użytkownika
                if (!await _zarzadzajUzytkownikiem.DodajInformacjeUzytkownikaAsync(nowyUzytkownik, dodajUzytkownikaViewModel))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się dodać informacji o użytkowniku.", false);
                }
                await _jednostkaPracy.ZatwierdzAsync();

                return ("Użytkownik dodany", true);
            }
            catch (Exception ex)
            {
                await _jednostkaPracy.CofnijAsync();
                _logger.LogError(ex, "Błąd podczas dodawania użytkownika {Login}", dodajUzytkownikaViewModel.Login);

                return ("Błąd: Nie udało się dodać użytkownika. Błąd systemu, spróbuj ponownie.", false);
            }
        }

        public async Task<(string Komunikat, bool CzyUdane)> EdytujUzytkownikaAsync(EdytujUzytkownikaViewModel edytujUzytkownikaViewModel, ClaimsPrincipal zalogowanyUzytkownik)
        {
            // Sprawdzenie czy istnieją role o podanych identyfikatorach
            if (!await _pobierajRole.CzyIstniejaRoleAsync(edytujUzytkownikaViewModel.WybraneRole))
            {
                return ("Nie znaleziono jednej lub więcej wybranych ról.", false);
            }
            bool zmianaHasla = !string.IsNullOrEmpty(edytujUzytkownikaViewModel.Haslo);
            //======================Sprawdzanie podstawowych danych=========================
            // zgodne z konfiguracją ASP.NET Core Identity.
            var sprawdzenie = _walidacjaDanychUzytkownika.CzyHasloPrawidlowe(edytujUzytkownikaViewModel.Haslo);
            if (zmianaHasla && !sprawdzenie.wynik)
            {
                return (sprawdzenie.wiadomosc, false);
            }

            // Walidacja telefonu
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyTelefon(edytujUzytkownikaViewModel.Telefon))
            {
                return ("Telefon musi zawierać dokładnie 9 cyfr.", false);
            }

            // Walidacja PESEL
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyPesel(edytujUzytkownikaViewModel.Pesel))
            {
                return ("Podany numer PESEL jest niepoprawny.", false);
            }
            //sprawdzanie czy pesel jest unikalny
            if (await _pobierajUzytkownika.CzyIstniejeInnyUzytkownikOTymPeseluAsync(edytujUzytkownikaViewModel.Pesel, edytujUzytkownikaViewModel.IdUzytkownika))
            {
                return ("Użytkownik o takim PESELu już istnieje.", false);
            }

            // Sprawdzenie loginu
            if (await _weryfikacjaDanychLogowania.CzyIstniejeInnyLoginAsync(edytujUzytkownikaViewModel.Login, edytujUzytkownikaViewModel.IdUzytkownika))
            {
                return ("Użytkownik o takim loginie już istnieje.", false);
            }

            // Sprawdzenie emaila
            if (await _weryfikacjaDanychLogowania.CzyIstniejeInnyEmailAsync(edytujUzytkownikaViewModel.Email, edytujUzytkownikaViewModel.IdUzytkownika))
            {
                return ("Użytkownik o takim emailu już istnieje.", false);
            }

            var wybraneNazwyRol = await _pobierajRole.PobierzNazwyRolPoIdAsync(edytujUzytkownikaViewModel.WybraneRole);
            var roleZalogowanego = await _pobierajRole.PobierzRoleZalogowanegoUzytkownikaAsync(zalogowanyUzytkownik);
            var roleDoZarzadzania = await _pobierajUprawnieniaRoli.PobierzNazwyRolKtorymiMozeZarzadzacAsync(roleZalogowanego);

            if (wybraneNazwyRol.Any(rola => !roleDoZarzadzania.Contains(rola)))
            {
                return ("Nie masz uprawnień do nadania jednej lub więcej wybranych ról.", false);
            }

            await _jednostkaPracy.RozpocznijTransakcjeAsync();
            try
            {
                // Aktualizacja konta

                // Aktualizacja Identity

                if (!await _zarzadzajUzytkownikiem.EdytujUzytkownikaAsync(edytujUzytkownikaViewModel, zmianaHasla))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zaktualizować danych logowania.", false);
                }

                // Aktualizacja danych osobowych
                var informacje = await _pobierajUzytkownika.PobierzInformacjeUzytkownikaPoIdLoginu(edytujUzytkownikaViewModel.IdUzytkownika);

                if (informacje == null)
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie znaleziono danych użytkownika.", false);
                }
                _mapowanieUzytkownika.MapujNaInformacjeUzytkownika(informacje, edytujUzytkownikaViewModel);

                if (!await _zarzadzajUzytkownikiem.EdytujInformacjeUzytkownikaAsync(informacje))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zaktualizować danych użytkownika.", false);
                }

                // Aktualizacja ról
                var uzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoLoginieAsync(edytujUzytkownikaViewModel.Login);

                if (!await _zarzadzajUzytkownikiem.UsunWszystkieRoleUzytkownikowiAsync(edytujUzytkownikaViewModel.Login))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zmienić ról.", false);
                }

                if (!await _zarzadzajUzytkownikiem.DodajRoleUzytkownikowiAsync(uzytkownik, wybraneNazwyRol))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zmienić ról lub nie wybrano roli.", false);
                }

                await _jednostkaPracy.ZatwierdzAsync();

                return ("Dane użytkownika zostały zmienione.", true);
            }
            catch (Exception ex)
            {
                await _jednostkaPracy.CofnijAsync();
                _logger.LogError(ex, "Błąd podczas edycji użytkownika {Login}", edytujUzytkownikaViewModel.Login);

                return ("Błąd: Wystąpił błąd podczas edycji użytkownika.", false);
            }
        }

        public async Task<ListaUzytkownikow> PodzielUzytkownikowNaRodzicowIPracownikowAsync(ClaimsPrincipal user)
        {
            var wynik = new ListaUzytkownikow();

            var roleUzytkownika = await _pobierajRole.PobierzRoleZalogowanegoUzytkownikaAsync(user);

            var roleDoZarzadzania = await _pobierajUprawnieniaRoli.PobierzNazwyRolKtorymiMozeZarzadzacAsync(roleUzytkownika);

            var uzytkownicy = await _pobierajUzytkownika.PobierzUzytkownikowPoRolachAsync(roleDoZarzadzania);

            var informacje = await _pobierajUzytkownika.PobierzUzytkownikowImieINazwiskoIIDPoLiscieID(uzytkownicy.Select(u => u.Id).ToList());

            var informacjePoId = informacje.ToDictionary(x => x.IdUzytkownika);

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

                if (uzytkownik.Role.Any(r =>_daneStartowe.RolePracownika.Contains(r)) ||
                    !uzytkownik.Role.Contains(NazwyRoli.Rodzic))
                {
                    wynik.Pracownicy.Add(element);
                }
                else
                {
                    wynik.Rodzice.Add(element);
                }
            }
            wynik.Rodzice = wynik.Rodzice.OrderBy(x => x.Text, comparer).ToList();
            wynik.Pracownicy = wynik.Pracownicy.OrderBy(x => x.Text, comparer).ToList();
            return wynik;
        }
    }
}