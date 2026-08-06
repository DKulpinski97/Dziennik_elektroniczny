using Dziennik_szkolny.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dziennik_szkolny.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDto>> PobierzRole();

        Task<bool> ZmienNazweRoli(string roleId, string nowaNazwa, string staraNazwa);

        Task<bool> DodajRole(string nazwaRoli);

        Task<bool> UsunRole(string roleId);
    }
}