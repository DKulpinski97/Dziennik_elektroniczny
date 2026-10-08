using Dziennik_szkolny.Domain.Entities;

namespace Dziennik_szkolny.Application.Interfejsy.Przedmioty
{
    public interface IPobierajPrzedmiot
    {
        public Task<bool> SprawdzCzyPrzedmiotIstniejePoNazwieAsync(string nazwaPrzedmiotu);
        public Task<List<Przedmiot>> PobierzWszystkiePrzedmioty();
        public Task<bool> SprawdzCzyPrzedmiotNalezyDoNauczycielaAsync(string IdNauczyciela, int IdPrzedmiotu);
    }
}
