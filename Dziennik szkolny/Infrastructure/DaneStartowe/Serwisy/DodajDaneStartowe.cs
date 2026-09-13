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

        public DodajDaneStartowe(
            DodajRoleStartowe dodajRoleStartowe,
            DodajLoginyStartowe dodajLoginyStartowe,
            PrzypiszRoleStartowe przypiszRoleStartowe,
            PrzypiszInformacjeStartowe przypiszInformacjeStartowe,
            DodajUprawnieniaZarzadzaniaRoli przypiszUprawnieniaZarzadzaniaRoli)
        {
            _dodajRoleStartowe = dodajRoleStartowe;
            _dodajLoginyStartowe = dodajLoginyStartowe;
            _przypiszRoleStartowe = przypiszRoleStartowe;
            _przypiszInformacjeStartowe = przypiszInformacjeStartowe;
            _przypiszUprawnieniaZarzadzaniaRoli = przypiszUprawnieniaZarzadzaniaRoli;
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
