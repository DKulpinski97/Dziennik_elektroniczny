using Dziennik_szkolny.Application.Interfejsy;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.Serwisy
{
    public class DodajDaneStartowe : IDodajDaneStartowe
    {
        private readonly DodajRoleStartowe _dodajRoleStartowe;
        private readonly DodajLoginyStartowe _dodajLoginyStartowe;
        private readonly PrzypiszRoleStartowe _przypiszRoleStartowe;
        private readonly PrzypiszInformacjeStartowe _przypiszInformacjeStartowe;

        public DodajDaneStartowe(
            DodajRoleStartowe dodajRoleStartowe,
            DodajLoginyStartowe dodajLoginyStartowe,
            PrzypiszRoleStartowe przypiszRoleStartowe,
            PrzypiszInformacjeStartowe przypiszInformacjeStartowe)
        {
            _dodajRoleStartowe = dodajRoleStartowe;
            _dodajLoginyStartowe = dodajLoginyStartowe;
            _przypiszRoleStartowe = przypiszRoleStartowe;
            _przypiszInformacjeStartowe = przypiszInformacjeStartowe;
        }

        public async Task DodajDaneStartoweAsync()
        {
            try
            {
                await _dodajRoleStartowe.DodajRoleStartoweAsync();

                await _dodajLoginyStartowe
                    .DodajLoginyStartoweAsync();

                await _przypiszRoleStartowe
                    .PrzypiszRoleStartoweAsync();

                await _przypiszInformacjeStartowe
                    .PrzypiszInformacjeDodatkoweAsync();
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
