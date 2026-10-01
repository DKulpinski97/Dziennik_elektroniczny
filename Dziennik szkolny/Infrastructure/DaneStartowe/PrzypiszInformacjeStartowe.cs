using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych;
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
        private readonly IPobierajUzytkownika _pobieranieUzytkownika;
        public PrzypiszInformacjeStartowe(
            AppDbContext appDbContext,
            UserManager<LoginUzytkownika> userManager,
            DaneStartowe daneStartowe,
            IPobierajUzytkownika pobieranieUzytkownika)
        {
            _context = appDbContext;
            _userManager = userManager;
            _daneStartowe = daneStartowe;
            _pobieranieUzytkownika = pobieranieUzytkownika;
        }
        public async Task<Dictionary<string, string>> PobierzIPrzygotujSlownikTlumaczenAsync()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var x in await _pobieranieUzytkownika.PobierzWszystkichUzytkownikow())
            {
                result.Add(x.UserName, x.Id);
            }
            return result;
        }
        internal List<DTOInformacjaUzytkownika> PrzygotujListe(Dictionary<string, string> tlumaczenieLoginuNaID)
        {
            List<DTOInformacjaUzytkownika> przygotowanaLista = new List<DTOInformacjaUzytkownika>();
            foreach (var x in _daneStartowe.Informacje)
            {
                przygotowanaLista.Add(DTOInformacjaUzytkownika.Utworz(x.Login, x.Imie, x.Nazwisko, x.Pesel, x.Telefon, x.Miasto, x.Ulica, x.NrMieszkania, tlumaczenieLoginuNaID));
            }
            return przygotowanaLista;
        }
        public async Task PrześlijDaneNaBaze(Dictionary<string, string> tlumaczenieLoginuNaID)
        {
            List<DTOInformacjaUzytkownika> informacjeUzytkownikow = PrzygotujListe(tlumaczenieLoginuNaID);

            foreach (var informacjeUzytkownika in informacjeUzytkownikow)
            {
                //sprawdzenie istnienia nie ma funkcji async w linq, więc trzeba zrobić to w pętli  

                /*if (!istnieje)
                {
                    //dodanie do listy wysłania
                }*/
            }

            //await _context.SaveChangesAsync();
        }

        
    }
}