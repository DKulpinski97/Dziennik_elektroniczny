using System.Security.Claims;

namespace Dziennik_szkolny.Application.Interfejsy.Role
{
    public interface IPobierajUprawnieniaRoli
    {
        Task<List<string>> PobierzRoleKtorymiMozeZarzadzacAsync(List<string> roleZarzadzajace);
    }
}
