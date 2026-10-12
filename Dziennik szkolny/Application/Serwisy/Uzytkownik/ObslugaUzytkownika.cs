using Dziennik_szkolny.Application.Interfejsy;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Mapery;
using Dziennik_szkolny.Application.Modele.Uzytkownik;
using Dziennik_szkolny.Application.Walidacja.Uzytkownik;
using Dziennik_szkolny.Infrastructure.DaneStartowe;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

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
        private readonly ILogger<ObslugaUzytkownika> _logger;


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
            _zarzadzajUzytkownikema = zarzadzajUzytkownikema;
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
            if (!_walidacjaDanychUzytkownika.CzyHasloPrawidlowe(dodajUzytkownikaViewModel.Haslo))
            {
                return ("Hasło musi zawierać co najmniej 8 znaków. w tym jedną cyfrę, jedną małą i jedną wielką literę oraz jeden znak specjalny.", false);
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
            // Sprawdzenie Pesla
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
            var roleDoZarzadzania = await _pobierajUprawnieniaRoli.PobierzRoleKtorymiMozeZarzadzacAsync(roleZalogowanego);

            if (wybraneNazwyRol.Any(rola => !roleDoZarzadzania.Contains(rola)))
            {
                return ("Nie masz uprawnień do nadania jednej lub więcej wybranych ról.", false);
            }

            await _jednostkaPracy.RozpocznijTransakcjeAsync();

            try
            {
                // Dodanie konta
                if (!await _zarzadzajUzytkownikema.DodajUzytkownikaAsync(dodajUzytkownikaViewModel))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się dodać użytkownika.", false);
                }
                var nowyUzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoLoginieAsync(dodajUzytkownikaViewModel.Login);
                //Dodaj Role użytkownikowi
                if (!await _zarzadzajUzytkownikema.DodajRoleUzytkownikowiAsync(nowyUzytkownik, wybraneNazwyRol))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się przypisać ról.", false);
                }
                // Dodanie informacji użytkownika
                if (!await _zarzadzajUzytkownikema.DodajInformacjeUzytkownikaAsync(nowyUzytkownik, dodajUzytkownikaViewModel))
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
           

            //sprawdzanie czy Email jest unikalny
            if (!string.IsNullOrEmpty(edytujUzytkownikaViewModel.Email) && await _pobierajUzytkownika.PobierzEmailUzytkownikaAsync(edytujUzytkownikaViewModel.IdUzytkownika) != edytujUzytkownikaViewModel.Email.ToUpper() && await _weryfikacjaDanychLogowania.CzyIstniejeEmailAsync(edytujUzytkownikaViewModel.Email))
            {
                return ("Użytkownik o takim emailu już istnieje.", false);
            }
            // Sprawdzenie czy istnieją role o podanych identyfikatorach
            if (!await _pobierajRole.CzyIstniejaRoleAsync(edytujUzytkownikaViewModel.WybraneRole))
            {
                return ("Nie znaleziono jednej lub więcej wybranych ról.", false);
            }
            bool zmianaHasla = !string.IsNullOrEmpty(edytujUzytkownikaViewModel.Haslo);
            //======================Sprawdzanie podstawowych danych=========================
            // zgodne z konfiguracją ASP.NET Core Identity.
            if (!string.IsNullOrEmpty(edytujUzytkownikaViewModel.Haslo) && !_walidacjaDanychUzytkownika.CzyHasloPrawidlowe(edytujUzytkownikaViewModel.Haslo))
            {
                return ("Hasło musi zawierać co najmniej 8 znaków. w tym jedną cyfrę, jedną małą i jedną wielką literę oraz jeden znak specjalny.", false);
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
            var roleDoZarzadzania = await _pobierajUprawnieniaRoli.PobierzRoleKtorymiMozeZarzadzacAsync(roleZalogowanego);

            if (wybraneNazwyRol.Any(rola => !roleDoZarzadzania.Contains(rola)))
            {
                return ("Nie masz uprawnień do nadania jednej lub więcej wybranych ról.", false);
            }

            await _jednostkaPracy.RozpocznijTransakcjeAsync();
            try
            {
                // =====================
                // Aktualizacja Identity
                // =====================
                // Dodanie konta
                if (!await _zarzadzajUzytkownikema.EdytujUzytkownikaAsync(edytujUzytkownikaViewModel, zmianaHasla))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zaktualizować danych logowania.", false);
                }

                // =====================
                // Aktualizacja danych osobowych
                // =====================

                var informacje = await _pobierajUzytkownika.PobierzInformacjeUzytkownikaPoIdLoginu(edytujUzytkownikaViewModel.IdUzytkownika);


                if (informacje == null)
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie znaleziono danych użytkownika.", false);
                }
                _mapowanieUzytkownika.MapujNaInformacjeUzytkownika(informacje, edytujUzytkownikaViewModel);


                if (!await _zarzadzajUzytkownikema.EdytujInformacjeUzytkownikaAsync(informacje))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie znaleziono danych użytkownika.", false);
                }


                // =====================
                // Aktualizacja ról
                // =====================

                LoginUzytkownika uzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoLoginieAsync(edytujUzytkownikaViewModel.Login);

                if (!await _zarzadzajUzytkownikema.UsunWszystkieRoleUzytkownikowiAsync(edytujUzytkownikaViewModel.Login))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zmienić ról.", false);
                }


                var noweRole = await _pobierajRole.PobierzNazwyRolPoIdAsync(edytujUzytkownikaViewModel.WybraneRole);


                if (!await _zarzadzajUzytkownikema.DodajRoleUzytkownikowiAsync(uzytkownik, noweRole))
                {
                    await _jednostkaPracy.CofnijAsync();
                    return ("Nie udało się zmienić ról lub nie wybrano roli", false);
                }




                await _jednostkaPracy.ZatwierdzAsync();

                return ("Dane użytkownika zostały zmienione.", true);
            }
            catch (Exception ex)
            {
                await _jednostkaPracy.CofnijAsync();
                _logger.LogError(ex, "Błąd podczas edycji użytkownika {Login}", edytujUzytkownikaViewModel.Login);

                return ("Wystąpił błąd podczas edycji użytkownika.", false);
            }
        }

        public async Task<ListaUzytkownikow> PodzielUzytkownikowNaRodzicowIPracownikowAsync(ClaimsPrincipal user)
        {
            var wynik = new ListaUzytkownikow();

            var roleUzytkownika = await _pobierajRole.PobierzRoleZalogowanegoUzytkownikaAsync(user);

            var roleDoZarzadzania = await _pobierajUprawnieniaRoli.PobierzRoleKtorymiMozeZarzadzacAsync(roleUzytkownika);

            var uzytkownicy = await _pobierajUzytkownika.PobierzUzytkownikowPoRolachAsync(roleDoZarzadzania);

            var informacje = await _pobierajUzytkownika.PobierzWszystkieInformacjeOUzrzytkownikach();

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