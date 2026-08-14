using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Microsoft.AspNetCore.Identity;

namespace Dziennik_szkolny.Application.Interfejsy.Role
{
    public interface IPobierajRole
    {
         Task<List<DaneRoli>> PobierzRole();
        Task<bool> CzyIstniejaRoleAsync(List<string> idRoli);
        Task<IdentityRole?> PobierzRolePoIdAsync(string idRoli);
    }
}
