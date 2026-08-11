using Dziennik_szkolny.Application.Interfejsy;
using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Models;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Dziennik_szkolny.Application.Serwisy
{
    public class UzytkownikService : IUzytkownikService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleService _roleService;


        public UzytkownikService(
            AppDbContext appDbContext,
            UserManager<LoginUzytkownika> userManager,
            RoleManager<IdentityRole> roleManager,
            IRoleService roleService)
        {
            _context = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _roleService = roleService;
        }


        public async Task<(string Komunikat, UzytkownikaViewModel Uzytkownik, bool CzyUdane)> DodajUzytkownikaAsync(
            UzytkownikaViewModel model)
        {
            // Walidacja telefonu
            if (string.IsNullOrWhiteSpace(model.Telefon) || !Regex.IsMatch(model.Telefon, @"^\d{9}$"))
            {
                return (
                    "Telefon musi zawierać dokładnie 9 cyfr.",
                    model,
                    false);
            }


            // Walidacja PESEL
            if (!CzyPoprawnyPesel(model.Pesel))
            {
                return (
                    "Podany numer PESEL jest niepoprawny.",
                    model,
                    false);
            }


            // Sprawdzenie loginu
            if (await CzyIstniejeLoginAsync(model.Login))
            {
                return (
                    "Użytkownik o takim loginie już istnieje.",
                    model,
                    false);
            }


            // Sprawdzenie emaila
            if (await CzyIstniejeEmailAsync(model.Email))
            {
                return (
                    "Użytkownik o takim emailu już istnieje.",
                    model,
                    false);
            }


            if (!await CzyIstniejaRoleAsync(model.WybraneRole))
            {
                return (
                    "Nie znaleziono jednej lub więcej wybranych ról.",
                    model,
                    false);
            }


            await using var transakcja = await _context.Database.BeginTransactionAsync();


            try
            {
                // Dodanie konta
                var wynikKonta = await DodajUzytkownikaDoBazyAsync(model);


                if (wynikKonta.Uzytkownik == null)
                {
                    return (wynikKonta.Komunikat, model, false);
                }


                var nowyLogin = wynikKonta.Uzytkownik;

                // Dodanie wybranych ról użytkownikowi
                if (!await DodajRoleUzytkownikowiAsync(
                    nowyLogin,
                    model.WybraneRole))
                {
                    await _userManager.DeleteAsync(nowyLogin);

                    await transakcja.RollbackAsync();

                    return ("Nie udało się przypisać ról.", model, false);
                }


                // Dodanie informacji użytkownika
                await DodajInformacjeUzytkownikaAsync(nowyLogin, model);


                await _context.SaveChangesAsync();

                await transakcja.CommitAsync();


                return ("Użytkownik dodany", model, true);
            }
            catch (Exception ex)
            {
                await transakcja.RollbackAsync();

                return ($"Błąd: {ex.Message} Inner: {ex.InnerException?.Message}", model, false);
            }
        }



        private async Task<(string Komunikat, LoginUzytkownika Uzytkownik, bool CzyUdane)> DodajUzytkownikaDoBazyAsync(UzytkownikaViewModel model)
        {
            var nowyLogin = new LoginUzytkownika
            {
                UserName = model.Login,
                Email = model.Email
            };


            var wynik = await _userManager.CreateAsync(
                nowyLogin,
                model.Haslo);


            if (!wynik.Succeeded)
            {
                var bledy = string.Join(", ",
                    wynik.Errors.Select(x => x.Description));


                return ($"Nie udało się utworzyć użytkownika: {bledy}", null, false);
            }


            return ("OK", nowyLogin, true);
        }



        private async Task<bool> DodajRoleUzytkownikowiAsync(LoginUzytkownika uzytkownik, List<string> idRoli)
        {
            if (idRoli == null || !idRoli.Any())
            {
                return true;
            }


            foreach (var idRoliItem in idRoli)
            {
                var rola = await _roleManager.FindByIdAsync(idRoliItem);


                if (rola == null)
                {
                    return false;
                }


                var wynik = await _userManager.AddToRoleAsync(uzytkownik, rola.Name);


                if (!wynik.Succeeded)
                {
                    return false;
                }
            }


            return true;
        }



        private async Task DodajInformacjeUzytkownikaAsync(LoginUzytkownika uzytkownik, UzytkownikaViewModel model)
        {
            long ostatniId = await _context.InformacjeUzytkownik.Select(x => (long?)x.IdOsoby).MaxAsync() ?? 0;

            long nowyId = ostatniId + 1;
            var informacje = new InformacjeUzytkownik
            {
                IdOsoby = nowyId,
                IdUzytkownika = uzytkownik.Id,
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Pesel = model.Pesel,
                Telefon = model.Telefon,
                Miasto = model.Miasto,
                Ulica = model.Ulica,
                NrMieszkania = model.NrMieszkania
            };


            await _context.InformacjeUzytkownik.AddAsync(informacje);
        }



        private async Task<bool> CzyIstniejeLoginAsync(string login)
        {
            var znormalizowanyLogin = _userManager.NormalizeName(login);


            return await _userManager.Users.AnyAsync(x => x.NormalizedUserName == znormalizowanyLogin);
        }



        private async Task<bool> CzyIstniejeEmailAsync(string email)
        {
            var znormalizowanyEmail = _userManager.NormalizeEmail(email);


            return await _userManager.Users.AnyAsync(x => x.NormalizedEmail == znormalizowanyEmail);
        }



        private async Task<bool> CzyIstniejaRoleAsync(List<string> idRoli)
        {
            if (idRoli == null || !idRoli.Any())
            {
                return true;
            }


            foreach (var idRoliItem in idRoli)
            {
                var rola = await _roleManager.FindByIdAsync(idRoliItem);


                if (rola == null)
                {
                    return false;
                }
            }


            return true;
        }



        private bool CzyPoprawnyPesel(string pesel)
        {
            if (string.IsNullOrWhiteSpace(pesel) || pesel.Length != 11 || !pesel.All(char.IsDigit))
            {
                return false;
            }


            int rok = int.Parse(pesel.Substring(0, 2));
            int miesiac = int.Parse(pesel.Substring(2, 2));
            int dzien = int.Parse(pesel.Substring(4, 2));


            int stulecie;


            if (miesiac >= 1 && miesiac <= 12)
            {
                stulecie = 1900;
            }
            else if (miesiac >= 21 && miesiac <= 32)
            {
                stulecie = 2000;
                miesiac -= 20;
            }
            else if (miesiac >= 41 && miesiac <= 52)
            {
                stulecie = 2100;
                miesiac -= 40;
            }
            else if (miesiac >= 61 && miesiac <= 72)
            {
                stulecie = 2200;
                miesiac -= 60;
            }
            else if (miesiac >= 81 && miesiac <= 92)
            {
                stulecie = 1800;
                miesiac -= 80;
            }
            else
            {
                return false;
            }


            try
            {
                _ = new DateTime(
                    stulecie + rok,
                    miesiac,
                    dzien);
            }
            catch
            {
                return false;
            }


            int[] wagi =
            {
                1, 3, 7, 9,
                1, 3, 7, 9,
                1, 3
            };


            int suma = 0;


            for (int i = 0; i < 10; i++)
            {
                suma += (pesel[i] - '0') * wagi[i];
            }


            int kontrolna =
                (10 - (suma % 10)) % 10;


            return kontrolna == pesel[10] - '0';
        }

        public async Task<UzytkownikaViewModel> PrzygotujDaneDoEdycjiAsync(InformacjeUzytkownik informacjeUzytkownik, string iDUser)
        {
            var login = await _userManager.FindByIdAsync(iDUser);


            var nazwyRol = await _userManager.GetRolesAsync(login);


            List<string> roleUzytkownika = new();

            foreach (var nazwaRoli in nazwyRol)
            {
                var rola = await _roleManager.FindByNameAsync(nazwaRoli);

                if (rola != null)
                {
                    roleUzytkownika.Add(rola.Id);
                }
            }
            UzytkownikaViewModel uzytkownikaViewModel = new UzytkownikaViewModel
            {
                Login = login.UserName,
                Email = login.Email,
                Haslo = null, // Hasło nie jest przechowywane w jawnej postaci.
                Imie = informacjeUzytkownik.Imie,
                Nazwisko = informacjeUzytkownik.Nazwisko,
                Pesel = informacjeUzytkownik.Pesel,
                Telefon = informacjeUzytkownik.Telefon,
                Miasto = informacjeUzytkownik.Miasto,
                Ulica = informacjeUzytkownik.Ulica,
                NrMieszkania = informacjeUzytkownik.NrMieszkania,
                DostepneRole = (await _roleService.PobierzRole()).Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.Nazwa
                }).ToList(),
                WybraneRole = roleUzytkownika,
                idUzytkownika = login.Id,
                IdDanych = informacjeUzytkownik.IdOsoby
            };

            return uzytkownikaViewModel;
        }
        public async Task<(bool Sukces, string Komunikat)> EdytujUzytkownikaAsync(UzytkownikaViewModel uzytkownikaViewModel)
        {
            var login = await _userManager.FindByIdAsync(uzytkownikaViewModel.idUzytkownika);

            if (login == null)
            {
                return (false, "Nie znaleziono użytkownika.");
            }


            // Sprawdzenie podstawowych danych
            if (string.IsNullOrWhiteSpace(uzytkownikaViewModel.Login))
            {
                return (false, "Login jest wymagany.");
            }


            // Sprawdzenie czy login nie należy do innego użytkownika
            var istniejeLogin = await _userManager.FindByNameAsync(uzytkownikaViewModel .Login);

            if (istniejeLogin != null && istniejeLogin.Id != login.Id)
            {
                return (false, "Podany login jest już zajęty.");
            }


            using var transakcja = await _context.Database.BeginTransactionAsync();

            try
            {
                // =====================
                // Aktualizacja Identity
                // =====================

                login.UserName = uzytkownikaViewModel.Login;
                login.NormalizedUserName = uzytkownikaViewModel.Login.ToUpper();

                login.Email = uzytkownikaViewModel.Email;
                login.NormalizedEmail = uzytkownikaViewModel.Email.ToUpper();


                // jeżeli podano nowe hasło
                if (!string.IsNullOrWhiteSpace(uzytkownikaViewModel.Haslo))
                {
                    var hasher = new PasswordHasher<LoginUzytkownika>();

                    login.PasswordHash = hasher.HashPassword(
                        login,
                        uzytkownikaViewModel.Haslo
                    );
                }


                var wynikIdentity = await _userManager.UpdateAsync(login);

                if (!wynikIdentity.Succeeded)
                {
                    await transakcja.RollbackAsync();
                    return (false, "Nie udało się zaktualizować konta.");
                }



                // =====================
                // Aktualizacja danych osobowych
                // =====================

                var informacje = await _context.InformacjeUzytkownik
                    .FirstOrDefaultAsync(x => x.IdUzytkownika == login.Id);


                if (informacje == null)
                {
                    await transakcja.RollbackAsync();
                    return (false, "Nie znaleziono danych użytkownika.");
                }


                informacje.Imie = uzytkownikaViewModel.Imie;
                informacje.Nazwisko = uzytkownikaViewModel.Nazwisko;
                informacje.Pesel = uzytkownikaViewModel.Pesel;
                informacje.Telefon = uzytkownikaViewModel.Telefon;
                informacje.Miasto = uzytkownikaViewModel.Miasto;
                informacje.Ulica = uzytkownikaViewModel.Ulica;
                informacje.NrMieszkania = uzytkownikaViewModel.NrMieszkania;


                _context.InformacjeUzytkownik.Update(informacje);

                await _context.SaveChangesAsync();



                // =====================
                // Aktualizacja ról
                // =====================

                var obecneRole = await _userManager.GetRolesAsync(login);


                var usunRole = await _userManager.RemoveFromRolesAsync(
                    login,
                    obecneRole);


                if (!usunRole.Succeeded)
                {
                    await transakcja.RollbackAsync();
                    return (false, "Nie udało się usunąć starych ról.");
                }


                var noweRole = await PobierzNazwyRolAsync(uzytkownikaViewModel.WybraneRole);


                var dodajRole = await _userManager.AddToRolesAsync(
                    login,
                    noweRole);


                if (!dodajRole.Succeeded)
                {
                    await transakcja.RollbackAsync();
                    return (false, "Nie udało się przypisać nowych ról.");
                }



                await transakcja.CommitAsync();

                return (true, "Dane użytkownika zostały zmienione.");
            }
            catch
            {
                await transakcja.RollbackAsync();

                return (false, "Wystąpił błąd podczas edycji użytkownika.");
            }
        }
        private async Task<List<string>> PobierzNazwyRolAsync(List<string> idRol)
        {
            if (idRol == null || idRol.Count == 0)
            {
                return new List<string>();
            }

            return await _roleManager.Roles
                .Where(x => idRol.Contains(x.Id))
                .Select(x => x.Name)
                .ToListAsync();
        }
    }
}