using Dziennik_szkolny.Application.Interfejsy.DaneStartowe;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.Serwisy
{
    public class DodajDaneStartowe : IDodajDaneStartowe
    {
        private readonly DodajRoleStartowe _dodajRoleStartowe;
        private readonly DodajLoginyStartowe _dodajLoginyStartowe;
        private readonly PrzypiszRoleStartowe _przypiszRoleStartowe;
        private readonly PrzypiszInformacjeStartowe _przypiszInformacjeStartowe;
        private readonly DodajUprawnieniaZarzadzaniaRoli _przypiszUprawnieniaZarzadzaniaRoli;
        private readonly DodajKlasyStartowe _dodajKlasyStartowe;
        private readonly DodajUczniowStartowych _dodajUczniowStartowych;
        private readonly DodajPrzedmiotyStartowe _dodajPrzedmiotyStartowe;
        private readonly DodajPrzypisaniePrzedmioty _dodajPrzypisaniePrzedmiot;

        public DodajDaneStartowe(
            DodajRoleStartowe dodajRoleStartowe,
            DodajLoginyStartowe dodajLoginyStartowe,
            PrzypiszRoleStartowe przypiszRoleStartowe,
            PrzypiszInformacjeStartowe przypiszInformacjeStartowe,
            DodajUprawnieniaZarzadzaniaRoli przypiszUprawnieniaZarzadzaniaRoli,
            DodajKlasyStartowe dodajKlasyStartowe,
            DodajUczniowStartowych dodajUczniowStartowych,
            DodajPrzedmiotyStartowe dodajPrzedmiotyStartowe,
            DodajPrzypisaniePrzedmioty dodajPrzypisaniePrzedmiot)
        {
            _dodajRoleStartowe = dodajRoleStartowe;
            _dodajLoginyStartowe = dodajLoginyStartowe;
            _przypiszRoleStartowe = przypiszRoleStartowe;
            _przypiszInformacjeStartowe = przypiszInformacjeStartowe;
            _przypiszUprawnieniaZarzadzaniaRoli = przypiszUprawnieniaZarzadzaniaRoli;
            _dodajKlasyStartowe = dodajKlasyStartowe;
            _dodajUczniowStartowych = dodajUczniowStartowych;
            _dodajPrzedmiotyStartowe = dodajPrzedmiotyStartowe;
            _dodajPrzypisaniePrzedmiot = dodajPrzypisaniePrzedmiot;
        }

        public async Task DodajDaneStartoweAsync()
        {
            try
            {
                await _dodajRoleStartowe.DodajRoleStartoweAsync();

                await _dodajLoginyStartowe.DodajLoginyStartoweAsync();

                await _przypiszRoleStartowe.PrzypiszRoleStartoweAsync();

                var tlumaczeniaLoginyNaIdUrzytkownika = await _przypiszInformacjeStartowe.PobierzIPrzygotujSlownikTlumaczenAsync();
                await _przypiszInformacjeStartowe.PrześlijDaneNaBaze(tlumaczeniaLoginyNaIdUrzytkownika);

                await _przypiszUprawnieniaZarzadzaniaRoli.DodajUprawnieniaZarzadzaniaRoliAsync();


                await _dodajKlasyStartowe.PrześlijDaneNaBaze(tlumaczeniaLoginyNaIdUrzytkownika);
                await _dodajUczniowStartowych.PrzeslijUczniowStartowych(tlumaczeniaLoginyNaIdUrzytkownika);

                await _dodajPrzedmiotyStartowe.DodajPrzedmiotyStartoweAsync();

                var tlumaczeniaPrzedmiotNaIdPrzedmiotu = await _dodajPrzypisaniePrzedmiot.PobierzIPrzygotujSlownikTlumaczenAsync();
                await _dodajPrzypisaniePrzedmiot.PrześlijDaneNaBaze(tlumaczeniaLoginyNaIdUrzytkownika, tlumaczeniaPrzedmiotNaIdPrzedmiotu);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Wystąpił błąd podczas dodawania danych startowych: {ex.Message}",
                    ex);
            }
        }
    }
}
