using Dziennik_szkolny.Application.Interfejsy.Klasa;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Domain;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.Infrastructure.Serwisy.Klasa;
using Dziennik_szkolny.Infrastructure.Serwisy.Uzytkownik;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajKlasyStartowe
    {
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly DaneStartowe _daneStartowe;
        private readonly IPobierajUzytkownika _pobieranieUzytkownika;
        private readonly AppDbContext _context;
        private readonly IPobierajKlase _pobierajKlase;

        public DodajKlasyStartowe(UserManager<LoginUzytkownika> userManager, DaneStartowe daneStartowe,
            IPobierajUzytkownika pobierajUzytkownika, AppDbContext context, IPobierajKlase pobierajKlase)
        {
            _userManager = userManager;
            _daneStartowe = daneStartowe;
            _pobieranieUzytkownika = pobierajUzytkownika;
            _context = context;
            _pobierajKlase = pobierajKlase;
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
        public List<string> PrzetlumaczNazwyNaId(Dictionary<string, string> TlumaczenieLoginuNaID)
        {
            List<string> result = new List<string>();
            foreach (var x in _daneStartowe.Klasy)
            {
                string rokZakonczenia = string.IsNullOrEmpty(x[3]) ? "" : x[3];
                string idNauczyciela = TlumaczenieLoginuNaID[x[4]];

                result.Add($"{x[0]},{x[1]},{x[2]},{rokZakonczenia},{idNauczyciela}");
            }
            return result;
        }
        public async Task PrześlijDaneNaBaze(Dictionary<string, string> TlumaczenieLoginuNaID)
        {

            List<string> przygotowanaLista = PrzetlumaczNazwyNaId(TlumaczenieLoginuNaID);
            foreach (var x in przygotowanaLista)
            {
                Klasa klasa = new Klasa();
                string[] podzielonyNapis = x.Split(',');
                if (string.IsNullOrEmpty(podzielonyNapis[3]))
                {
                    klasa.Oznaczenie = podzielonyNapis[0];
                    klasa.RokNauki = Convert.ToByte(podzielonyNapis[1]);
                    klasa.RokRozpoczecia = Convert.ToInt32(podzielonyNapis[2]);
                    klasa.RokZakonczenia = null;
                    klasa.IdWychowawcy = podzielonyNapis[4];
                }
                else
                {
                    klasa.Oznaczenie = podzielonyNapis[0];
                    klasa.RokNauki = Convert.ToByte(podzielonyNapis[1]);
                    klasa.RokRozpoczecia = Convert.ToInt32(podzielonyNapis[2]);
                    klasa.RokZakonczenia = Convert.ToInt32(podzielonyNapis[3]);
                    klasa.IdWychowawcy = podzielonyNapis[4];
                }
                var istnieje = await _pobierajKlase.SprawdzCzyKlasaIstniejePoDacieIOznaczeniuAsync(klasa.Oznaczenie,klasa.RokRozpoczecia);

                if (!istnieje)
                {
                _context.Klasa.Add(klasa);
                }
            }
            await _context.SaveChangesAsync();

        }
    }
}
