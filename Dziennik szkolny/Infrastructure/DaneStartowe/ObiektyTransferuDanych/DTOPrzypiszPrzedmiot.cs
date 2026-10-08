using Dziennik_szkolny.Domain.Entities;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych
{
    public class DTOPrzypiszPrzedmiot
    {
        public string IdNauczyciela { get; }
        public int IdPrzedmiotu { get; }

        private DTOPrzypiszPrzedmiot(string idNauczyciela, int idPrzedmiotu)
        {
            IdNauczyciela = idNauczyciela;
            IdPrzedmiotu = idPrzedmiotu;
        }
       
        public static DTOPrzypiszPrzedmiot Utworz(string loginNauczyciela, string nazwaPrzedmiotu, Dictionary<string, string> tlumaczenieLoginuNaId, Dictionary<string, string> tlumaczenieNazwyPrzedmiotowNaId)
        {
            if (!tlumaczenieLoginuNaId.TryGetValue(loginNauczyciela, out string idNauczyciela))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora nauczyciela dla loginu {loginNauczyciela}.");
            }
            if (!tlumaczenieNazwyPrzedmiotowNaId.TryGetValue(nazwaPrzedmiotu, out string idPrzedmiotu))
            {
                throw new InvalidOperationException($"Nie można znaleźć identyfikatora przedmiotu dla nazwy {nazwaPrzedmiotu}.");
            }
            return new DTOPrzypiszPrzedmiot(idNauczyciela, int.Parse(idPrzedmiotu));
        }
        public PrzypisaniePrzedmiotu DoEncja()
        {
            return new PrzypisaniePrzedmiotu
            {
                IdNauczyciela = this.IdNauczyciela,
                IdPrzedmiotu = this.IdPrzedmiotu
            };
        }
    }
}
