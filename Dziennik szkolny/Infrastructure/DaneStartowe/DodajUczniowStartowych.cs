using Dziennik_szkolny.Application.Interfejsy.Klasa;
using Dziennik_szkolny.Application.Interfejsy.Uczen;
using Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajUczniowStartowych
    {
        private readonly DaneStartowe _daneStartowe;
        private readonly AppDbContext _context;
        private readonly IPobierajKlase _pobierajKlase;
        private readonly IPobierajUcznia _pobierajUczen;

        public DodajUczniowStartowych(DaneStartowe daneStartowe,
            AppDbContext context, IPobierajKlase pobierajKlase, IPobierajUcznia pobierajUczen)
        {
            _daneStartowe = daneStartowe;
            _context = context;
            _pobierajKlase = pobierajKlase;
            _pobierajUczen = pobierajUczen;
        }
        public async Task<Dictionary<(string OznaczenieKlasy, string RokRozpoczecia), string>> PobierzIPrzygotujSlownikTlumaczenAsync()
        {
            var result = new Dictionary<(string OznaczenieKlasy, string RokRozpoczecia), string>();
            foreach (var x in await _pobierajKlase.PobierzWszystkieKlasy())
            {
                result.Add((x.Oznaczenie, x.RokRozpoczecia.ToString()), x.IdKlasy.ToString());
            }
            return result;
        }
        internal  List<DTOUczen> PrzygotujListe(Dictionary<string, string> tlumaczenieLoginuNaID, Dictionary<(string, string), string> tlumaczeniaKlasyNaIdKlasy)
        {
            List<DTOUczen> przygotowanaLista = new List<DTOUczen>();
            foreach (var x in _daneStartowe.Uczniowie)
            {
                przygotowanaLista.Add(DTOUczen.Utworz(x.Pesel, x.Imie, x.Nazwisko, DateOnly.Parse(x.DataUrodzenia), x.Opiekun1Login, x.Opiekun2Login, x.OznaczenieKlasy, x.RokRozpoczeciaKlasy, tlumaczenieLoginuNaID, tlumaczeniaKlasyNaIdKlasy));
            }
            return przygotowanaLista;
        }
        public async Task PrzeslijUczniowStartowych(Dictionary<string, string> tlumaczenieLoginuNaID, Dictionary<(string, string), string> tlumaczeniaKlasyNaIdKlasy)
        {
            var przygotowanaLista =  PrzygotujListe(tlumaczenieLoginuNaID, tlumaczeniaKlasyNaIdKlasy);
            foreach (var x in przygotowanaLista)
            {
                if (!await _pobierajUczen.SprawdzCzyUczenIstniejePoPeselAsync(x.Pesel))
                {
                    await _context.Uczniowie.AddAsync(x.DoEncja());
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
