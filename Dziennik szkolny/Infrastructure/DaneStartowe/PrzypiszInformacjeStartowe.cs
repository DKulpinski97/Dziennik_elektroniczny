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

        public PrzypiszInformacjeStartowe(
            AppDbContext appDbContext,
            UserManager<LoginUzytkownika> userManager)
        {
            _context = appDbContext;
            _userManager = userManager;
        }

        public async Task PrzypiszInformacjeDodatkoweAsync()
        {
            List<string[]> informacje =
            [
                ["Admin", "Admin@gmail.com", "Admin", "AdminImie", "AdminNaz", "85010112345", "500100100", "Warszawa", "Marszałkowska", "11"],
                ["Dyrektor", "Dyrektor@gmail.com", "Dyrektor", "DyrektorImie", "DyrektorNaz", "82020223456", "500200200", "Warszawa", "Puławska", "14"],
                ["Sekretarka", "Sekretarka@gmail.com", "Sekretarka", "SekretarkaImie", "SekretarkaNaz", "90030334567", "500300300", "Warszawa", "Grochowska", "24"],

                ["Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1", "Nauczyciel1Imie", "Nauczyciel1Naz", "88040445678", "500400400", "Warszawa", "Górczewska", "1"],
                ["Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2", "Nauczyciel2Imie", "Nauczyciel2Naz", "87050556789", "500500500", "Warszawa", "Górczewska", "2"],
                ["Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3", "Nauczyciel3Imie", "Nauczyciel3Naz", "86060667890", "500600600", "Warszawa", "Targowa", "41"],

                ["Rodzic1", "Rodzic1@gmail.com", "Rodzic1", "Rodzic1Imie", "Rodzic1Naz", "85070778901", "500700700", "Warszawa", "Modlińska", "25"],
                ["Rodzic2", "Rodzic2@gmail.com", "Rodzic2", "Rodzic2Imie", "Rodzic2Naz", "84080889012", "500800800", "Warszawa", "Wawelska", "15"],
                ["Rodzic3", "Rodzic3@gmail.com", "Rodzic3", "Rodzic3Imie", "Rodzic3Naz", "83090990123", "500900900", "Warszawa", "Białobrzeska", "26"]
            ];

            List<InformacjeUzytkownik> informacjeUzytkownikow = [];

            foreach (var dane in informacje)
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