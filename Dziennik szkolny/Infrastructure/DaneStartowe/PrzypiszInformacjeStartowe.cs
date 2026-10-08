using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class PrzypiszInformacjeStartowe
    {
        private readonly AppDbContext _context;
        private readonly DaneStartowe _daneStartowe;
        private readonly IPobierajUzytkownika _pobieranieUzytkownika;
        public PrzypiszInformacjeStartowe(
            AppDbContext appDbContext,
            DaneStartowe daneStartowe,
            IPobierajUzytkownika pobieranieUzytkownika)
        {
            _context = appDbContext;
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

                bool istnieje = await _pobieranieUzytkownika.SprawdzCzyInformacjeUzytkownikaIstniejaPoIdUzytkownikaAsync(informacjeUzytkownika.IdUzytkownika);
                if (!istnieje)
                {
                    _context.InformacjeUzytkownik.Add(informacjeUzytkownika.DoEncja());
                }
            }

            await _context.SaveChangesAsync();
        }


    }
}