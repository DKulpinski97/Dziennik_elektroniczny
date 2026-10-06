using Dziennik_szkolny.Domain.Entities;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych
{
    public class DTOKlasa
    {
        public string Oznaczenie { get; }
        public byte RokNauki { get; }
        public int RokRozpoczecia { get; }
        public int? RokZakonczenia { get; }
        public string IdWychowawcy { get; }
        private DTOKlasa(string oznaczenie, byte rokNauki, int rokRozpoczecia, int? rokZakonczenia, string idWychowawcy)
        {
            Oznaczenie = oznaczenie;
            RokNauki = rokNauki;
            RokRozpoczecia = rokRozpoczecia;
            RokZakonczenia = rokZakonczenia;
            IdWychowawcy = idWychowawcy;
        }
        public static DTOKlasa Utworz(string oznaczenie, byte rokNauki, int rokRozpoczecia, int? rokZakonczenia, string LoginWychowawcy, Dictionary<string, string> tlumaczenieLoginuNaId)
        {
            if (!tlumaczenieLoginuNaId.TryGetValue(LoginWychowawcy, out string wychowawcaId))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora wychowawcy dla loginu {LoginWychowawcy}.");
            }
            return new DTOKlasa(oznaczenie, rokNauki, rokRozpoczecia, rokZakonczenia, wychowawcaId);
        }
        public Klasa DoEncja()
        {
            return new Klasa
            {
                Oznaczenie = this.Oznaczenie,
                RokNauki = this.RokNauki,
                RokRozpoczecia = this.RokRozpoczecia,
                RokZakonczenia = this.RokZakonczenia,
                IdWychowawcy = this.IdWychowawcy
            };
        }
    }
}
