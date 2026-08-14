using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Walidacja.Uzytkownik;

namespace Dziennik_szkolny.Application.Serwisy.Uzytkownik
{
    public class UzytkownikService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IZarzadzajRolami _roleService;
        private readonly IWeryfikacjaDanychLogowania _weryfikacjaDanychLogowania;
        private readonly IPobierajRole _pobierajRole;
        private readonly WalidacjaDanychUzytkownika _walidacjaDanychUzytkownika;
        private readonly IZarzadzajUzytkownikem _zarzadzajUzytkownikema;
        private readonly IPobierajUzytkownika _pobierajUzytkownika;


        public UzytkownikService(
            AppDbContext appDbContext,
            UserManager<LoginUzytkownika> userManager,
            RoleManager<IdentityRole> roleManager,
            IZarzadzajRolami roleService,
            IWeryfikacjaDanychLogowania weryfikacjaDanychLogowania,
            IPobierajRole pobierajRole, 
            IZarzadzajUzytkownikem zarzadzajUzytkownikema,
            IPobierajUzytkownika pobierajUzytkownika,
            WalidacjaDanychUzytkownika walidacjaDanychUzytkownika)
        {
            _context = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _roleService = roleService;
            _weryfikacjaDanychLogowania = weryfikacjaDanychLogowania;
            _pobierajRole = pobierajRole;
            _walidacjaDanychUzytkownika = walidacjaDanychUzytkownika;
            _zarzadzajUzytkownikema = zarzadzajUzytkownikema;
            _pobierajUzytkownika = pobierajUzytkownika;
        }


        public async Task<(string Komunikat, UzytkownikaViewModel Uzytkownik, bool CzyUdane)> DodajUzytkownikaAsync(
            UzytkownikaViewModel model)
        {
            // Walidacja telefonu
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyTelefon(model.Telefon))
            {
                return (
                    "Telefon musi zawierać dokładnie 9 cyfr.",
                    model,
                    false);
            }


            // Walidacja PESEL
            if (!_walidacjaDanychUzytkownika.CzyPoprawnyPesel(model.Pesel))
            {
                return (
                    "Podany numer PESEL jest niepoprawny.",
                    model,
                    false);
            }


            // Sprawdzenie loginu
            if (await _weryfikacjaDanychLogowania.CzyIstniejeLoginAsync(model.Login))
            {
                return (
                    "Użytkownik o takim loginie już istnieje.",
                    model,
                    false);
            }


            // Sprawdzenie emaila
            if (await _weryfikacjaDanychLogowania.CzyIstniejeEmailAsync(model.Email))
            {
                return (
                    "Użytkownik o takim emailu już istnieje.",
                    model,
                    false);
            }


            if (!await _pobierajRole.CzyIstniejaRoleAsync(model.WybraneRole))
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
                if(!await _zarzadzajUzytkownikema.DodajUzytkownikaAsync(model))
                {
                    await transakcja.RollbackAsync();
                    return ("Nie udało się dodać użytkownika.", model, false);
                }
                var nowyUzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoLoginieAsync(model.Login);
                //Dodaj Role użytkownikowi
                if (! await _zarzadzajUzytkownikema.DodajRoleUzytkownikowiAsync(nowyUzytkownik, model.WybraneRole))
                {
                    await transakcja.RollbackAsync();
                    return ("Nie udało się przypisać ról.", model, false);
                }
                // Dodanie informacji użytkownika
                if (!await _zarzadzajUzytkownikema.DodajInformacjeUzytkownikaAsync(nowyUzytkownik, model))
                {
                    await transakcja.RollbackAsync();
                    return ("Nie udało się dodać informacji o użytkowniku.", model, false);
                }
                await transakcja.CommitAsync();


                return ("Użytkownik dodany", model, true);
            }
            catch (Exception ex)
            {
                await transakcja.RollbackAsync();

                return ($"Błąd: {ex.Message} Inner: {ex.InnerException?.Message}", model, false);
            }
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