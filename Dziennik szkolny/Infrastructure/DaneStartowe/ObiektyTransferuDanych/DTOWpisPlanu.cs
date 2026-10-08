using Dziennik_szkolny.Domain.Entities;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych
{
    public class DTOWpisPlanu
    {
        public byte Lekcja { get; }
        public DzienTygodnia Dzien { get; }
        public int IdKlasy { get;  }
        public int IdPrzedmiotu { get; }
        public string IdNauczyciela { get; }
        private DTOWpisPlanu(byte lekcja, DzienTygodnia dzien, int idKlasy, int idPrzedmiotu, string idNauczyciela)
        {
            Lekcja = lekcja;
            Dzien = dzien;
            IdKlasy = idKlasy;
            IdPrzedmiotu = idPrzedmiotu;
            IdNauczyciela = idNauczyciela;
        }
        public static DTOWpisPlanu Utworz(string nrLekcja, string dzien, string oznaczenieKlasy, string rokRozpoczeciaKlasy, 
            string nazwaPrzedmiotu, string loginNauczyciela, Dictionary<string, string> tlumaczeniaLoginyNaIdUrzytkownika,
            Dictionary<(string OznaczenieKlasy, string RokRozpoczecia), string> tlumaczeniaKlasyNaIdKlasy, Dictionary<string, string> tlumaczeniaPrzedmiotNaIdPrzedmiotu)
        {
            if (!byte.TryParse(nrLekcja, out byte numerLekcji) || numerLekcji < 1 || numerLekcji > 8)
            {
                throw new InvalidOperationException($"Nieprawidłowy numer lekcji. Zakres to od 1 do 8. Twoja wartość: {nrLekcja}.");
            }
            if (!Enum.TryParse<DzienTygodnia>(dzien, out var dzienTygodnia) || !Enum.IsDefined(dzienTygodnia))
            {
                throw new InvalidOperationException($"Nieprawidłowy dzień tygodnia. Dozwolone wartości: {string.Join(", ", Enum.GetNames<DzienTygodnia>())}. Twoja wartość: {dzien}.");
            }
            if (!tlumaczeniaLoginyNaIdUrzytkownika.TryGetValue(loginNauczyciela, out string idNauczyciela))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora nauczyciela dla loginu {loginNauczyciela}.");
            }
            if (!tlumaczeniaKlasyNaIdKlasy.TryGetValue((oznaczenieKlasy, rokRozpoczeciaKlasy), out string idKlasy))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora klasy dla oznaczenia {oznaczenieKlasy} i roku rozpoczęcia {rokRozpoczeciaKlasy}.");
            }
            if (!tlumaczeniaPrzedmiotNaIdPrzedmiotu.TryGetValue(nazwaPrzedmiotu, out string idPrzedmiotu))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora przedmiotu dla nazwy {nazwaPrzedmiotu}.");
            }
            return new DTOWpisPlanu(numerLekcji, dzienTygodnia, Convert.ToInt32(idKlasy), Convert.ToInt32(idPrzedmiotu), idNauczyciela);
        }
        public WpisPlanu DoEncja()
        {
            return new WpisPlanu
            {
                Lekcja = this.Lekcja,
                Dzien = this.Dzien,
                IdKlasy = this.IdKlasy,
                IdPrzedmiotu = this.IdPrzedmiotu,
                IdNauczyciela = this.IdNauczyciela
            };
        }
    }
}
