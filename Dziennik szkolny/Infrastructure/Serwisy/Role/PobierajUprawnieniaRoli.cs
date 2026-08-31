using Dziennik_szkolny.Application.Interfejsy.Role;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Role
{
    public class PobierajUprawnieniaRoli : IPobierajUprawnieniaRoli
    {
        private readonly AppDbContext _context;
        private readonly IPobierajRole _pobierajRole;

        public PobierajUprawnieniaRoli(
            AppDbContext context,
            IPobierajRole pobierajRole)
        {
            _context = context;
            _pobierajRole = pobierajRole;
        }

        public async Task<List<string>> PobierzRoleKtorymiMozeZarzadzacAsync(List<string> roleZarzadzajace)
        {
            var wynik = new List<string>();

            foreach (var rola in roleZarzadzajace)
            {
                var idRoli = await _pobierajRole
                    .PobierzIdRoliPoNazwieAsync(rola);

                if (idRoli == null)
                {
                    continue;
                }

                var roleDoZarzadzania =
                    await _context.UprawnieniaZarzadzaniaRola
                        .Where(x => x.RolaZarzadzajacaId == idRoli)
                        .Select(x => x.RolaZarzadzanaId)
                        .ToListAsync();

                wynik.AddRange(roleDoZarzadzania);
            }

            wynik = wynik.Distinct().ToList();

            return await _pobierajRole.PobierzNazwyRolPoIdAsync(wynik);
        }
    }
}
