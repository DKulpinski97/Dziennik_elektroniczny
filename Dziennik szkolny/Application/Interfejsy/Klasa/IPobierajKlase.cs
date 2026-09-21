namespace Dziennik_szkolny.Application.Interfejsy.Klasa
{
    public interface IPobierajKlase
    {
        Task<bool> SprawdzCzyKlasaIstniejePoDacieIOznaczeniuAsync(string oznaczenie, int rokRozpoczecia);
    }
}
