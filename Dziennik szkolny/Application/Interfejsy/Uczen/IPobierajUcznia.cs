namespace Dziennik_szkolny.Application.Interfejsy.Uczen
{
    public interface IPobierajUcznia
    {
        public Task<bool> SprawdzCzyUczenIstniejePoPeselAsync(string pesel);
    }
}
