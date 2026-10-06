using Dziennik_szkolny.Application.Interfejsy.Przedmioty;
using Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajPrzedmiotyStartowe
    {
        private readonly DaneStartowe _daneStartowe;
        private readonly AppDbContext _context;
        private readonly IPobierajPrzedmiot _pobierajPrzedmiot;
        public DodajPrzedmiotyStartowe(DaneStartowe daneStartowe, AppDbContext context, IPobierajPrzedmiot pobierajPrzedmiot)
        {
            _daneStartowe = daneStartowe;
            _context = context;
            _pobierajPrzedmiot = pobierajPrzedmiot;
        }
        public async Task DodajPrzedmiotyStartoweAsync()
        {

            foreach (var dane in _daneStartowe.Przedmioty)
            {
                var istnieje = await _pobierajPrzedmiot.SprawdzCzyPrzedmiotIstniejePoNazwieAsync(dane);
                if (!istnieje)
                {
                    var dto = new DTOPrzedmiot(dane);
                    _context.Przedmioty.Add(dto.DoEncja());

                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
