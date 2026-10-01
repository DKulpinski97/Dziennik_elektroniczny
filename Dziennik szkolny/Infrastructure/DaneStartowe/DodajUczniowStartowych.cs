using Dziennik_szkolny.Application.Interfejsy.Klasa;
using Dziennik_szkolny.Application.Interfejsy.Uczen;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
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
        internal async Task<List<DTOUczen>> PrzygotujListe(Dictionary<string, string> tlumaczenieLoginuNaID)
        {
            var klasy = await _pobierajKlase.PobierzWszystkieKlasy();
            List<DTOUczen> przygotowanaLista = new List<DTOUczen>();
            foreach (var x in _daneStartowe.Uczen)
            {
                int idKlasy = klasy.FirstOrDefault(k => k.Oznaczenie == x.OznaczenieKlasy && k.RokRozpoczecia == Convert.ToInt32(x.RokRozpoczeciaKlasy))?.IdKlasy ?? 0;
                if (idKlasy != 0)
                {
                    var dtoUczen = DTOUczen.Utworz(x.Pesel, x.Imie, x.Nazwisko, DateOnly.Parse(x.DataUrodzenia), idKlasy, x.Opiekun1Login, x.Opiekun2Login, tlumaczenieLoginuNaID);
                    przygotowanaLista.Add(dtoUczen);
                }
                else
                {
                    throw new InvalidOperationException($"Nie można znaleźć klasy dla ucznia {x.Imie} {x.Nazwisko} z oznaczeniem klasy {x.OznaczenieKlasy} i rokiem rozpoczęcia {x.RokRozpoczeciaKlasy}.");
                }
            }
            return przygotowanaLista;
        }
        public async Task PrzeslijUczniowStartowych(Dictionary<string, string> tlumaczenieLoginuNaID)
        {
            var przygotowanaLista = await PrzygotujListe(tlumaczenieLoginuNaID);
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
