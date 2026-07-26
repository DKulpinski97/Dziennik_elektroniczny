using Dzienik_szkolny.Data;
using Dzienik_szkolny.Models;
using Dzienik_szkolny.Services.Interfaces;
using Dzienik_szkolny.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Dzienik_szkolny.Services
{
    public class UzytkownikService : IUzytkownikService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<LoginUzytkownika> _userManager;

        public UzytkownikService(
            AppDbContext appDbContext,
            UserManager<LoginUzytkownika> userManager)
        {
            _context = appDbContext;
            _userManager = userManager;
        }


        public async Task<string> DodajUzytkownikaAsync(DodajUzytkownikaViewModel dodajUzytkownikaViewModel)
        {
            var istniejeLogin = await _userManager.FindByNameAsync(
                dodajUzytkownikaViewModel.Login);

            if (istniejeLogin != null)
            {
                return "Użytkownik o takim loginie już istnieje.";
            }


            var istniejeEmail = await _userManager.FindByEmailAsync(
                dodajUzytkownikaViewModel.Email);

            if (istniejeEmail != null)
            {
                return "Użytkownik o takim emailu już istnieje.";
            }


            var nowyLogin = new LoginUzytkownika
            {
                UserName = dodajUzytkownikaViewModel.Login,
                Email = dodajUzytkownikaViewModel.Email
            };


            var wynik = await _userManager.CreateAsync(
                nowyLogin,
                dodajUzytkownikaViewModel.Haslo);


            if (!wynik.Succeeded)
            {
                var bledy = string.Join(
                    ", ",
                    wynik.Errors.Select(x => x.Description));

                return$"Nie udało się utworzyć użytkownika: {bledy}";
            }


            // przypisanie ról
            if (dodajUzytkownikaViewModel.IdRoli != null &&
                dodajUzytkownikaViewModel.IdRoli.Any())
            {
                foreach (var idRoli in dodajUzytkownikaViewModel.IdRoli)
                {
                    var rola = await _userManager
                        .GetRolesAsync(nowyLogin);

                    await _userManager.AddToRoleAsync(
                        nowyLogin,
                        idRoli);
                }
            }


            var informacje = new InformacjeUzytkownik
            {
                IdUzytkownika = nowyLogin.Id,

                Imie = dodajUzytkownikaViewModel.Imie,
                Nazwisko = dodajUzytkownikaViewModel.Nazwisko,
                Pesel = dodajUzytkownikaViewModel.Pesel,
                Telefon = dodajUzytkownikaViewModel.Telefon,
                Miasto = dodajUzytkownikaViewModel.Miasto,
                Ulica = dodajUzytkownikaViewModel.Ulica,
                NrMieszkania = dodajUzytkownikaViewModel.NrMieszkania
            };


            await _context.InformacjeUzytkownik.AddAsync(informacje);

            await _context.SaveChangesAsync();

            return "Użytkownik dodany";
        }

    }
}