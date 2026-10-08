using Dziennik_szkolny.Domain.Entities;

namespace Dziennik_szkolny.Application.Interfejsy.Plany
{
    public interface IPobierajWpisPlanu
    {
        public Task<bool> SprawdzCzyIstniejeWpisPlanuAsync(int IdKlasy, byte lekcja, DzienTygodnia Dzien);
    }
}
