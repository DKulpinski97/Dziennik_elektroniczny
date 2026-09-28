using Dziennik_szkolny.Application.Interfejsy.Klasa;
using Dziennik_szkolny.Application.Interfejsy.Uczen;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajUczniowStartowych
    {
        private readonly DaneStartowe _daneStartowe;
        private readonly AppDbContext _context;
        private readonly IPobierajKlase _pobierajKlase;
        private readonly IPobierajUcznia _pobierajUczen;

        public DodajUczniowStartowych(DaneStartowe daneStartowe,IPobierajUzytkownika pobierajUzytkownika, 
            AppDbContext context, IPobierajKlase pobierajKlase, IPobierajUcznia pobierajUczen)
        {
            _daneStartowe = daneStartowe;
            _context = context;
            _pobierajKlase = pobierajKlase;
            _pobierajUczen = pobierajUczen;
        }
        public List<string> PrzygotujListeDoPrzesłania(List<Dziennik_szkolny.Domain.Entities.Klasa> klasy, Dictionary<string, string> tlumaczenieLoginuNaID)
        {
            List<string> result = new List<string>();
            foreach (var x in _daneStartowe.Uczen)
            {
                var klasa = klasy.Where(k => k.Oznaczenie == x[4] && k.RokRozpoczecia == Convert.ToInt32(x[5])).FirstOrDefault();
                if(klasa == null)
                {
                    throw new InvalidOperationException($"Nie można znaleźć klasy o oznaczeniu {x[4]} i roku rozpoczęcia {x[5]}.");
                }
                string opiekun1Id = tlumaczenieLoginuNaID.TryGetValue(x[6], out var id1) ? id1 : null;
                string opiekun2Id = tlumaczenieLoginuNaID.TryGetValue(x[7], out var id2) ? id2 : null;
                if(opiekun1Id == null)
                {
                    throw new InvalidOperationException($"Nie można znaleźć identyfikatora opiekuna 1 dla ucznia {x[0]} {x[1]}.");
                }
                result.Add($"{x[0]},{x[1]},{x[2]},{x[3]},{klasa.IdKlasy},{opiekun1Id},{opiekun2Id}");
            }
            return result;
        }
        public async Task PrzeslijUczniowStartowych(Dictionary<string, string> tlumaczenieLoginuNaID)
        {
            Uczen uczen;
            var klasy = await _pobierajKlase.PobierzWszystkieKlasy();
            var przygotowanaLista = PrzygotujListeDoPrzesłania(klasy, tlumaczenieLoginuNaID);
            foreach (var x in przygotowanaLista)
            {
               string[] tmp = x.Split(',');
                uczen = new Uczen();
                uczen.Pesel = tmp[0];
                uczen.Imie = tmp[1];
                uczen.Nazwisko = tmp[2];
                uczen.DataUrodzenia = DateOnly.Parse(tmp[3]);
                uczen.IdKlasy = Convert.ToInt32(tmp[4]);
                uczen.IdOpiekun1 = tmp[5];
                uczen.IdOpiekun2 = string.IsNullOrEmpty(tmp[6]) ? null : tmp[6];

                if (!await _pobierajUczen.SprawdzCzyUczenIstniejePoPeselAsync(uczen.Pesel))
                {
                    await _context.Uczniowie.AddAsync(uczen);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
