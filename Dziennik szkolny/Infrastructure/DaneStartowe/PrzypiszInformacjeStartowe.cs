using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class PrzypiszInformacjeStartowe
    {
        private readonly AppDbContext _context;
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly DaneStartowe _daneStartowe;
        public PrzypiszInformacjeStartowe(
            AppDbContext appDbContext,
            UserManager<LoginUzytkownika> userManager,
            DaneStartowe daneStartowe)
        {
            _context = appDbContext;
            _userManager = userManager;
            _daneStartowe = daneStartowe;
        }

        public async Task PrzypiszInformacjeDodatkoweAsync()
        {
           

            List<InformacjeUzytkownik> informacjeUzytkownikow = [];

            foreach (var dane in _daneStartowe.Informacje)
            {
                var uzytkownik = await _userManager.FindByNameAsync(dane[0])
                    ?? throw new InvalidOperationException(
                        $"Nie znaleziono użytkownika startowego '{dane[0]}'.");

                informacjeUzytkownikow.Add(
                    PrzypiszDane(dane, uzytkownik.Id));
            }

            // Tymczasowo ręczne nadawanie ID, ponieważ AUTO_INCREMENT
            // dla IdOsoby nie działa poprawnie w obecnej konfiguracji.
            long ostatnieId = await _context.InformacjeUzytkownik
                .Select(x => (long?)x.IdOsoby)
                .MaxAsync() ?? 0;

            List<InformacjeUzytkownik> noweInformacje = [];

            foreach (var informacjeUzytkownika in informacjeUzytkownikow)
            {
                bool istnieje = await _context.InformacjeUzytkownik
                    .AnyAsync(x =>
                        x.IdUzytkownika == informacjeUzytkownika.IdUzytkownika);

                if (!istnieje)
                {
                    ostatnieId++;

                    informacjeUzytkownika.IdOsoby = ostatnieId;

                    noweInformacje.Add(informacjeUzytkownika);
                }
            }

            if (noweInformacje.Count > 0)
            {
                await _context.InformacjeUzytkownik.AddRangeAsync(noweInformacje);
                await _context.SaveChangesAsync();
            }
        }

        private InformacjeUzytkownik PrzypiszDane(
            string[] dane,
            string idUzytkownika)
        {
            return new InformacjeUzytkownik
            {
                IdUzytkownika = idUzytkownika,
                Imie = dane[3],
                Nazwisko = dane[4],
                Pesel = dane[5],
                Telefon = dane[6],
                Miasto = dane[7],
                Ulica = dane[8],
                NrMieszkania = dane[9]
            };
        }
    }
}