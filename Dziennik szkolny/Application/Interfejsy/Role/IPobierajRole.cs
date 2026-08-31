using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Dziennik_szkolny.Application.Interfejsy.Role
{
    public interface IPobierajRole
    {
         Task<List<DaneRoli>> PobierzRole(ClaimsPrincipal user);
        Task<bool> CzyIstniejaRoleAsync(List<string> idRoli);
        Task<IdentityRole?> PobierzRolePoIdAsync(string idRoli);
        Task<IList<string>> PobierzRoleUzytkownikaPoLoginieAsync(string login);
        Task<List<string>> PobierzNazwyRolPoIdAsync(List<string> idRol);
    }
}
