using System.Security.Claims;

namespace Dziennik_szkolny.Application.Interfejsy.Role
{
    public interface IPobierajUprawnieniaRoli
    {
        Task<List<string>> PobierzNazwyRolKtorymiMozeZarzadzacAsync(List<string> roleZarzadzajace);
        Task<bool> CzyMozeZarzadzacUzytkownikiemAsync(ClaimsPrincipal user, string idUzytkownika);
        Task<bool> CzyRolaJestUzywanaWHierarchiiAsync(string idRoli);
        Task<List<string>> PobierzIdRolKtorymiMozeZarzadzacAsync(List<string> roleZarzadzajace);
        Task<(bool CzyMoze, string Komunikat)> CzyMozeZarzadzacRolaAsync(ClaimsPrincipal user, string idRoli);
        Task<bool> CzyRolaMaPowiazaniaPozaAdminemAsync(string idRoli, string idAdmina);
    }
}
