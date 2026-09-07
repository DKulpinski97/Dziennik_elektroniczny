using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Dziennik_szkolny.Application.Interfejsy.Role
{
    public interface IPobierajRole
    {
         Task<List<DaneRoli>> PobierzRole(ClaimsPrincipal user);
        Task<List<string>> PobierzRoleZalogowanegoUzytkownikaAsync(ClaimsPrincipal user);
        Task<bool> CzyIstniejaRoleAsync(List<string> idRoli);
        Task<IdentityRole?> PobierzRolePoIdAsync(string idRoli);
        Task<IdentityRole?> PobierzRolePoNazwie(string nazwa);
        Task<IList<string>> PobierzRoleUzytkownikaPoLoginieAsync(string login);
        Task<List<string>> PobierzNazwyRolPoIdAsync(List<string> idRol);
        Task<string?> PobierzIdRoliPoNazwieAsync(string nazwaRoli);
        Task<List<string>> PobierzRoleUzytkownikaPoIdAsync(string idUzytkownika);
    }
}
