using Dziennik_szkolny.Application.Interfejsy.Plany;
using Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajWpisPlanu 
    {
        private readonly AppDbContext _context;
        private readonly DaneStartowe _daneStartowe;
        private readonly IPobierajWpisPlanu _pobierajWpisPlanu;

        public DodajWpisPlanu(AppDbContext context, DaneStartowe daneStartowe, IPobierajWpisPlanu pobierajWpisPlanu)
        {
            _context = context;
            _daneStartowe = daneStartowe;
            _pobierajWpisPlanu = pobierajWpisPlanu;
        }
        internal List<DTOWpisPlanu> PrzygotujListe(Dictionary<string, string> tlumaczeniaLoginyNaIdUrzytkownika,
            Dictionary<(string OznaczenieKlasy, string RokRozpoczecia), string> tlumaczeniaKlasyNaIdKlasy, Dictionary<string, string> tlumaczeniaPrzedmiotNaIdPrzedmiotu)
        {
            List<DTOWpisPlanu> result = new List<DTOWpisPlanu>();
            foreach (var x in _daneStartowe.WpisyPlanu)
            {
                result.Add(DTOWpisPlanu.Utworz(x.NrLekcja, x.Dzien, x.Oznaczenie, x.RokRozpoczecia, x.NazwaPrzedmiotu, x.LoginNauczyciela, tlumaczeniaLoginyNaIdUrzytkownika, tlumaczeniaKlasyNaIdKlasy, tlumaczeniaPrzedmiotNaIdPrzedmiotu));
            }
            return result;
        }
        public async Task PrześlijDaneNaBaze(Dictionary<string, string> tlumaczeniaLoginyNaIdUrzytkownika,
            Dictionary<(string OznaczenieKlasy, string RokRozpoczecia), string> tlumaczeniaKlasyNaIdKlasy, Dictionary<string, string> tlumaczeniaPrzedmiotNaIdPrzedmiotu)
        {
            var przygotowanaLista = PrzygotujListe(tlumaczeniaLoginyNaIdUrzytkownika, tlumaczeniaKlasyNaIdKlasy, tlumaczeniaPrzedmiotNaIdPrzedmiotu);
            foreach (var x in przygotowanaLista)
            {
                if (!await _pobierajWpisPlanu.SprawdzCzyIstniejeWpisPlanuAsync(x.IdKlasy,x.Lekcja, x.Dzien))
                {
                     _context.WpisyPlanu.Add(x.DoEncja());
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
