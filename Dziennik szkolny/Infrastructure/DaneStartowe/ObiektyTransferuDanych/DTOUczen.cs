using Dziennik_szkolny.Domain.Entities;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych
{
    public class DTOUczen
    {

        public string Pesel { get; }

        public string Imie { get; }

        public string Nazwisko { get; }

        public DateOnly DataUrodzenia { get; }

        public int IdKlasy { get; }

        public string IdOpiekun1 { get; }
        public string? IdOpiekun2 { get; }

        private DTOUczen(string pesel, string imie, string nazwisko, DateOnly dataUrodzenia, int idKlasy, string idOpiekun1, string? idOpiekun2)
        {
            Pesel = pesel;
            Imie = imie;
            Nazwisko = nazwisko;
            DataUrodzenia = dataUrodzenia;
            IdKlasy = idKlasy;
            IdOpiekun1 = idOpiekun1;
            IdOpiekun2 = idOpiekun2;
        }
        public static DTOUczen Utworz(string pesel, string imie, string nazwisko, DateOnly dataUrodzenia, string loginOpiekun1, string? loginOpiekun2, string oznaczenieKlasy, string rokRozpoczeciaKlasy, Dictionary<string, string> tlumaczenieLoginuNaId, Dictionary<(string, string), string> tlumaczeniaKlasyNaIdKlasy)
        {
            if (!tlumaczenieLoginuNaId.TryGetValue(loginOpiekun1, out string idOpiekun1))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora opiekuna1 dla loginu {loginOpiekun1}.");
            }
            string? idOpiekun2 = null;
            if (!string.IsNullOrEmpty(loginOpiekun2) && !tlumaczenieLoginuNaId.TryGetValue(loginOpiekun2, out idOpiekun2))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora opiekuna2 dla loginu {loginOpiekun2}.");
            }
            if (!tlumaczeniaKlasyNaIdKlasy.TryGetValue((oznaczenieKlasy, rokRozpoczeciaKlasy), out string idKlasy))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora klasy dla oznaczenia {oznaczenieKlasy} i roku rozpoczęcia {rokRozpoczeciaKlasy}.");
            }
            return new DTOUczen(pesel, imie, nazwisko, dataUrodzenia, Convert.ToInt32(idKlasy), idOpiekun1, idOpiekun2);
        }
        public Uczen DoEncja()
        {
            return new Uczen
            {
                Pesel = this.Pesel,
                Imie = this.Imie,
                Nazwisko = this.Nazwisko,
                DataUrodzenia = this.DataUrodzenia,
                IdKlasy = this.IdKlasy,
                IdOpiekun1 = this.IdOpiekun1,
                IdOpiekun2 = this.IdOpiekun2
            };
        }
    }
}
