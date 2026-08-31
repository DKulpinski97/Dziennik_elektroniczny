using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;

namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IPobierajUzytkownika
    {
        Task<LoginUzytkownika> PobierzUzytkownikaPoLoginieAsync(string login);
        Task<List<LoginUzytkownika>> PobierzWszystkichUzytkownikow();
        Task<List<InformacjeUzytkownik>> PobierzWszystkieInformacjeOUzrzytkownikach();
        Task<InformacjeUzytkownik> PobierzInformacjeUzytkownikaPoIdLoginu(string IdUzytkownika);
        Task<LoginUzytkownika> PobierzUzytkownikaPoIDAsync(string IdUzytkownika);
        Task<List<UzytkownikZRolamiDto>> PobierzUzytkownikowPoRolachAsync(List<string> role);
    }
}
