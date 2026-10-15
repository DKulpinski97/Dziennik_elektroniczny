using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using System.Security.Claims;

namespace Dziennik_szkolny.Application.Interfejsy.Role
{
    public interface IZarzadzajRolami
    {

        Task<(bool Sukces, string Komunikat)> ZmienNazweRoli(string roleId, string nowaNazwa, string staraNazwa);

        Task<(bool Sukces, string Komunikat)> DodajRole(string nazwaRoli);

        Task<(bool Sukces, string Komunikat)> UsunRole(string roleId);
        Task<bool> PolaczRoleZarzadzanaZZarzadzajacaAsync(string zarzadzajacaRola, string zarzadzanaRola);
        Task<(bool Sukces, string Komunikat)> KontrolaDodawaRoliAsync(string zarzadzajacaRola, string NazwaNowejRoli);
        (List<DaneRoli> RoleSystemowe, List<DaneRoli> PozostaleRole) PodzierRoleNaSystemoweIPozostale(ClaimsPrincipal user,List<DaneRoli> wszystkieDostempneRole);


    }
}