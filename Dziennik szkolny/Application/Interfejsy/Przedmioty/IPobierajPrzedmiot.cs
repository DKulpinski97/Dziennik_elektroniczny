namespace Dziennik_szkolny.Application.Interfejsy.Przedmioty
{
    public interface IPobierajPrzedmiot
    {
        public Task<bool> SprawdzCzyPrzedmiotIstniejePoNazwieAsync(string nazwaPrzedmiotu);
    }
}
