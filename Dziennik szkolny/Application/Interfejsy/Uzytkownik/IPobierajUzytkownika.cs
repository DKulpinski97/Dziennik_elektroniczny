using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IPobierajUzytkownika
    {
        Task<LoginUzytkownika> PobierzUzytkownikaPoLoginieAsync(string login);
        Task<string> PobierzIdUzytkownikaPoLoginieAsync(string login);
        Task<List<LoginUzytkownika>> PobierzWszystkichUzytkownikow();
        Task<List<InformacjeUzytkownik>> PobierzWszystkieInformacjeOUzrzytkownikach();
        Task<InformacjeUzytkownik> PobierzInformacjeUzytkownikaPoIdLoginu(string IdUzytkownika);
        Task<LoginUzytkownika> PobierzUzytkownikaPoIDAsync(string IdUzytkownika);
        Task<List<UzytkownikZRolamiDto>> PobierzUzytkownikowPoRolachAsync(List<string> role);
        Task<bool> SprawdzCzyInformacjeUzytkownikaIstniejaPoIdUzytkownikaAsync(string idUzytkownika);
        Task<string> PobierzEmailUzytkownikaAsync(string idUzytkownika);
        Task<bool> CzyPeselIstniejeAsync(string pesel);
        Task<bool> CzyIstniejeInnyUzytkownikOTymPeseluAsync(string pesel, string idUzytkownika);
        Task<bool> CzyUzytkownikJestOpiekunemAsync(string idUzytkownika);
        Task<bool> CzyUzytkownikProwadziPrzedmiotAsync(string idUzytkownika);
        Task<bool> CzyUzytkownikJestWychowawcaAsync(string idUzytkownika);

    }
}
