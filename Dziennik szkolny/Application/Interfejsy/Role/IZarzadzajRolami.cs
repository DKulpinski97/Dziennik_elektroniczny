using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dziennik_szkolny.Application.Interfejsy.Role
{
    public interface IZarzadzajRolami
    {
       
        Task<bool> ZmienNazweRoli(string roleId, string nowaNazwa, string staraNazwa);

        Task<bool> DodajRole(string nazwaRoli);

        Task<bool> UsunRole(string roleId);
    }
}