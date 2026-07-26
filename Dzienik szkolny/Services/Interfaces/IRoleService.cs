using Dzienik_szkolny.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Dzienik_szkolny.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<IdentityRole>> PobierzRole();

        Task<bool> ZmienNazweRoli(string roleId, string nowaNazwa, string staraNazwa);

        Task<bool> DodajRole(string nazwaRoli);

        Task<bool> UsunRole(string roleId);
    }
}