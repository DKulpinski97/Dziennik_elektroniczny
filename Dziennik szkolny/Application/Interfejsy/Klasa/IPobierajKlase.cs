namespace Dziennik_szkolny.Application.Interfejsy.Klasa
{
    public interface IPobierajKlase
    {
        Task<bool> SprawdzCzyKlasaIstniejePoDacieIOznaczeniuAsync(string oznaczenie, int rokRozpoczecia);
        Task<int> PobieranieIdKlasyPoDacieIOznaczeniuAsync(string oznaczenie, int rokRozpoczecia);
         Task<List<Dziennik_szkolny.Domain.Entities.Klasa>> PobierzWszystkieKlasy();
    }
}
