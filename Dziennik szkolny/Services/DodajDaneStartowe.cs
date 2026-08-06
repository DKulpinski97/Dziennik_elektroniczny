using Dziennik_szkolny.Data;
using Dziennik_szkolny.Models;
using Dziennik_szkolny.ViewModel;
using Dziennik_szkolny.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Services
{
    public class DodajDaneStartowe : IDodajDaneStartowe
    {
        private readonly AppDbContext _context;
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DodajDaneStartowe(
            AppDbContext appDbContext,
            UserManager<LoginUzytkownika> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task DodajDaneStartoweAsync()
        {
            //przypisywanie roli 
            List<string> role = new List<string> { "Admin", "Nauczyciel", "Uczen", "Brak roli", "Dyrektor", "ViceDerektor", "Sekretarka", "Rodzić" };
            await DodajRoleStartowe(role);

            //lista logionów
            List<string[]> loginy = new List<string[]>
            {
                new string[] { "Admin", "Admin@gmail.com", "Admin" },
                new string[] { "Dyrektor", "Dyrektor@gmail.com", "Dyrektor" },
                new string[] { "Sekretarka", "Sekretarka@gmail.com", "Sekretarka" },

                new string[] { "Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1" },
                new string[] { "Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2" },
                new string[] { "Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3" },

                new string[] { "Rodzic1", "Rodzic1@gmail.com", "Rodzic1" },
                new string[] { "Rodzic2", "Rodzic2@gmail.com", "Rodzic2" },
                new string[] { "Rodzic3", "Rodzic3@gmail.com", "Rodzic3" }
            };
            //lista roli

            List<List<string>> NazwyRoli = new List<List<string>>
            {
                new List<string> { "Admin" },
                new List<string> { "Dyrektor" },
                new List<string> { "Sekretarka" },
                new List<string> { "Nauczyciel","Rodzić" },
                new List<string> { "Nauczyciel" },
                new List<string> { "Nauczyciel" },
                new List<string> { "Rodzić" },
                new List<string> { "Rodzić" },
                new List<string> { "Rodzić" }
            };
            // informacje dodatkowe o użytkownikach
            List<string[]> Informacje = new List<string[]>
            {
                new string[] { "Admin", "Admin@gmail.com", "Admin", "AdminImie", "AdminNaz", "85010112345", "500100100", "Warszawa", "Marszałkowska","11"},
                new string[] { "Dyrektor", "Dyrektor@gmail.com", "Dyrektor", "DyrektorImie", "DyrektorNaz", "82020223456", "500200200", "Warszawa","Puławska","14" },
                new string[] { "Sekretarka", "Sekretarka@gmail.com", "Sekretarka", "SekretarkaImie", "SekretarkaNaz", "90030334567", "500300300", "Warszawa","Grochowska","24" },

                new string[] { "Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1", "Nauczyciel1Imie", "Nauczyciel1Naz", "88040445678", "500400400", "Warszawa", "Górczewska","1" },
                new string[] { "Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2", "Nauczyciel2Imie", "Nauczyciel2Naz", "87050556789", "500500500", "Warszawa", "Górczewska","2" },
                new string[] { "Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3", "Nauczyciel3Imie", "Nauczyciel3Naz", "86060667890", "500600600", "Warszawa", "Targowa","41" },

                new string[] { "Rodzic1", "Rodzic1@gmail.com", "Rodzic1", "Rodzic1Imie", "Rodzic1Naz", "85070778901", "500700700", "Warszawa", "Modlińska","25" },
                new string[] { "Rodzic2", "Rodzic2@gmail.com", "Rodzic2", "Rodzic2Imie", "Rodzic2Naz", "84080889012", "500800800", "Warszawa", "Wawelska","15" },
                new string[] { "Rodzic3", "Rodzic3@gmail.com", "Rodzic3", "Rodzic3Imie", "Rodzic3Naz", "83090990123", "500900900", "Warszawa", "Białobrzeska","26" }
            };
            //Dodawnie użytkownika
            foreach (var dane in loginy)
            {
                var istnieje = await _userManager.FindByNameAsync(dane[0]);

                if (istnieje == null)
                {
                    var nowyLogin = new LoginUzytkownika
                    {
                        UserName = dane[0],
                        Email = dane[1]
                    };


                    var wynik = await _userManager.CreateAsync(
                        nowyLogin,
                        dane[2]);
                }
            }

            //Pobieranie ID Roli na podstawie nazwy roli i przypisanie ich do listy IdRoli
            List<List<string>> IdRoli = new List<List<string>>();

            foreach (var PojedynczyZakresRoli in NazwyRoli)
            {
                List<string> listaId = new List<string>();

                foreach (var nazwaRoli in PojedynczyZakresRoli)
                {
                    var rola = await _roleManager.FindByNameAsync(nazwaRoli);

                    if (rola != null)
                    {
                        listaId.Add(rola.Id);
                    }
                }

                IdRoli.Add(listaId);
            }
            //przypisuje role do użytkownika
            for (var i = 0; i < NazwyRoli.Count; i++)
            {
                var nazwaRoli = NazwyRoli[i];
                var userLogin = loginy[i][0];
                var user = await _userManager.FindByNameAsync(userLogin);

                foreach (var PojedynczaRola in nazwaRoli)
                {
                    bool posiadaRole = await _userManager.IsInRoleAsync(user, PojedynczaRola);

                    if (!posiadaRole)
                    {
                        await _userManager.AddToRoleAsync(user, PojedynczaRola);
                    }
                }
            }
            //przypisuje informacje dodatkowe do użytkownika
            List<InformacjeUzytkownik> informacjeUzytkowniks = new List<InformacjeUzytkownik>();
            for (int i = 0;i<Informacje.Count;i++)
            {
                var Uzytkownik = await _userManager.FindByNameAsync(Informacje[i][0]);
                informacjeUzytkowniks.Add(Przypiszdane(Informacje[i], Uzytkownik.Id));
            }
            var noweInformacje = new List<InformacjeUzytkownik>();
            //Nie wiem dlaczego po mimo że baza ma autoincerement nadany w IdOsoby to i tak nie działa więc trzeba pobrać ostatnie id i dodać 1 do niego
            long ostatnieId = await _context.InformacjeUzytkownik
                .Select(x => (long?)x.IdOsoby)
                .MaxAsync() ?? 0;

            foreach (var informacje in informacjeUzytkowniks)
            {
                bool istnieje = await _context.InformacjeUzytkownik
                    .AnyAsync(x => x.IdUzytkownika == informacje.IdUzytkownika);

                if (!istnieje)
                {
                    ostatnieId++;
                    informacje.IdOsoby = ostatnieId;
                    noweInformacje.Add(informacje);
                }
            }

            if (noweInformacje.Any())
            {
                await _context.InformacjeUzytkownik.AddRangeAsync(noweInformacje);
                await _context.SaveChangesAsync();
            }
        }

        public InformacjeUzytkownik Przypiszdane(string[] Dane,string IDUzytkownika)
        {
            var informacje = new InformacjeUzytkownik
            {
                IdUzytkownika = IDUzytkownika,
                Imie = Dane[3],
                Nazwisko = Dane[4],
                Pesel = Dane[5],
                Telefon = Dane[6],
                Miasto = Dane[7],
                Ulica = Dane[8],
                NrMieszkania = Dane[9]
            };
            return informacje;
        }
        public async Task DodajRoleStartowe(List<string> role)
        {
            foreach (string s in role)
            {
                var istnieje = await _roleManager.RoleExistsAsync(s);

                if (!istnieje)
                {
                    var rola = new IdentityRole(s);
                    await _roleManager.CreateAsync(rola);
                }
            }
        }
    }
}
