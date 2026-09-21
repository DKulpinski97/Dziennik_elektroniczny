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

        public DodajDaneStartowe(
            DodajRoleStartowe dodajRoleStartowe,
            DodajLoginyStartowe dodajLoginyStartowe,
            PrzypiszRoleStartowe przypiszRoleStartowe,
            PrzypiszInformacjeStartowe przypiszInformacjeStartowe,
            DodajUprawnieniaZarzadzaniaRoli przypiszUprawnieniaZarzadzaniaRoli,
            DodajKlasyStartowe dodajKlasyStartowe)
        {
            _dodajRoleStartowe = dodajRoleStartowe;
            _dodajLoginyStartowe = dodajLoginyStartowe;
            _przypiszRoleStartowe = przypiszRoleStartowe;
            _przypiszInformacjeStartowe = przypiszInformacjeStartowe;
            _przypiszUprawnieniaZarzadzaniaRoli = przypiszUprawnieniaZarzadzaniaRoli;
            _dodajKlasyStartowe = dodajKlasyStartowe;
        }

        public async Task DodajDaneStartoweAsync()
        {
            try
            {
                await _dodajRoleStartowe.DodajRoleStartoweAsync();

                await _dodajLoginyStartowe.DodajLoginyStartoweAsync();

                await _przypiszRoleStartowe.PrzypiszRoleStartoweAsync();

                await _przypiszInformacjeStartowe.PrzypiszInformacjeDodatkoweAsync();

                await _przypiszUprawnieniaZarzadzaniaRoli.DodajUprawnieniaZarzadzaniaRoliAsync();

               var tlumaczenia= await _dodajKlasyStartowe.PobierzIPrzygotujSlownikTlumaczenAsync();

                await _dodajKlasyStartowe.PrześlijDaneNaBaze(tlumaczenia);
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
