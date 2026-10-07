using Dziennik_szkolny.Application.Interfejsy.Przedmioty;
using Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajPrzypisaniePrzedmioty
    {
        private readonly AppDbContext _context;
        private readonly DaneStartowe _daneStartowe;
        private readonly IPobierajPrzedmiot _pobierajPrzedmiot;
        public DodajPrzypisaniePrzedmioty(
            AppDbContext appDbContext,
            DaneStartowe daneStartowe,
            IPobierajPrzedmiot pobierajPrzedmiot)
        {
            _context = appDbContext;
            _daneStartowe = daneStartowe;
            _pobierajPrzedmiot = pobierajPrzedmiot;
        }
        public async Task<Dictionary<string, string>> PobierzIPrzygotujSlownikTlumaczenAsync()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var x in await _pobierajPrzedmiot.PobierzWszystkiePrzedmioty())
            {
                result.Add(x.NazwaPrzedmiotu, x.IdPrzedmiotu.ToString());
            }
            return result;
        }
        internal List<DTOPrzypiszPrzedmiot> PrzygotujListe(Dictionary<string, string> tlumaczenieLoginuNaId, Dictionary<string, string> tlumaczenieNazwyPrzedmiotowNaId)
        {
            List<DTOPrzypiszPrzedmiot> result = new List<DTOPrzypiszPrzedmiot>();
            foreach (var x in _daneStartowe.PrzypiszPrzedmioty)
            {
                result.Add(DTOPrzypiszPrzedmiot.Utworz(x.LoginNauczyciela, x.NazwaPrzedmiotu, tlumaczenieLoginuNaId, tlumaczenieNazwyPrzedmiotowNaId));
            }
            return result;
        }
        public async Task PrześlijDaneNaBaze(Dictionary<string, string> tlumaczenieLoginuNaId, Dictionary<string, string> tlumaczenieNazwyPrzedmiotowNaId)
        {
            List<DTOPrzypiszPrzedmiot> przypisaniaPrzedmiotow = PrzygotujListe(tlumaczenieLoginuNaId, tlumaczenieNazwyPrzedmiotowNaId);

            foreach (var przypisanie in przypisaniaPrzedmiotow)
            {
                bool istnieje = await _pobierajPrzedmiot.SprawdzCzyPrzedmiotNalezyDoNauczycielaAsync(przypisanie.IdNauczyciela, przypisanie.IdPrzedmiotu);
                if (!istnieje)
                {
                    _context.PrzypisanePrzedmioty.Add(przypisanie.DoEncja());
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
