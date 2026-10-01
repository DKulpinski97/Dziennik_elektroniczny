using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych
{
    public class DTOInformacjaUzytkownika
    {

        public string Imie { get; }
        public string Nazwisko { get; }
        public string Pesel { get; }
        public string Telefon { get; }
        public string Miasto { get; }
        public string Ulica { get; }
        public string NrMieszkania { get; }
        public string IdUzytkownika { get; }


        private DTOInformacjaUzytkownika(string imie, string nazwisko, string pesel, string telefon, string miasto, string ulica, string nrMieszkania, string idUzytkownika)
        {
            Imie = imie;
            Nazwisko = nazwisko;
            Pesel = pesel;
            Telefon = telefon;
            Miasto = miasto;
            Ulica = ulica;
            NrMieszkania = nrMieszkania;
            IdUzytkownika = idUzytkownika;
        }
        public static DTOInformacjaUzytkownika Utworz(string Login, string imie, string nazwisko, string pesel, string telefon, string miasto, string ulica, string nrMieszkania, Dictionary<string, string> tlumaczenieLoginuNaId)
        {
            if (!tlumaczenieLoginuNaId.TryGetValue(Login, out string IdUzytkownika))
            {
                throw new InvalidOperationException($"Nie można znaleźć uzytkownika dla loginu {Login}.");
            }
           
            return new DTOInformacjaUzytkownika (imie, nazwisko, pesel, telefon, miasto, ulica, nrMieszkania, IdUzytkownika);
        }
        public InformacjeUzytkownik DoEncja()
        {
            return new InformacjeUzytkownik
            {
                Imie = this.Imie,
                Nazwisko = this.Nazwisko,
                Pesel = this.Pesel,
                Telefon = this.Telefon,
                Miasto = this.Miasto,
                Ulica = this.Ulica,
                NrMieszkania = this.NrMieszkania,
                IdUzytkownika = this.IdUzytkownika
            };
        }

    }
}
