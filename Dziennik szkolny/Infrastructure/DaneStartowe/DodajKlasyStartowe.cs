using Dziennik_szkolny.Application.Interfejsy.Klasa;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Domain;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.Infrastructure.Serwisy.Klasa;
using Dziennik_szkolny.Infrastructure.Serwisy.Uzytkownik;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajKlasyStartowe
    {
        private readonly DaneStartowe _daneStartowe;
        private readonly AppDbContext _context;
        private readonly IPobierajKlase _pobierajKlase;

        public DodajKlasyStartowe(DaneStartowe daneStartowe,
            IPobierajUzytkownika pobierajUzytkownika, AppDbContext context, IPobierajKlase pobierajKlase)
        {
            _daneStartowe = daneStartowe;
            _context = context;
            _pobierajKlase = pobierajKlase;
        }
        
        internal List<DTOKlasa> PrzygotujListe(Dictionary<string, string> tlumaczenieLoginuNaID)
        {
            List<DTOKlasa> przygotowanaLista = new List<DTOKlasa>();
            foreach (var x in _daneStartowe.Klasy)
            {
                przygotowanaLista.Add(DTOKlasa.Utworz(x.Oznaczenie, Convert.ToByte(x.RokNauki), Convert.ToInt32(x.RokRozpoczecia), string.IsNullOrEmpty(x.RokZakonczenia) ? (int?)null : Convert.ToInt32(x.RokZakonczenia), x.LoginWychowawcy, tlumaczenieLoginuNaID));
            }
            return przygotowanaLista;
        }
        public async Task PrześlijDaneNaBaze(Dictionary<string, string> tlumaczenieLoginuNaID)
        {

            List<DTOKlasa> przygotowanaLista = PrzygotujListe(tlumaczenieLoginuNaID);

            foreach (var x in przygotowanaLista)
            {

                var istnieje = await _pobierajKlase.SprawdzCzyKlasaIstniejePoDacieIOznaczeniuAsync(x.Oznaczenie, x.RokRozpoczecia);

                if (!istnieje)
                {
                    _context.Klasa.Add(x.DoEncja());
                }
            }
            await _context.SaveChangesAsync();

        }
    }
}
