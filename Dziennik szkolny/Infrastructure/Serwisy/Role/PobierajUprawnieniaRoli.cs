using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


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

        public async Task<List<string>> PobierzNazwyRolKtorymiMozeZarzadzacAsync(List<string> roleZarzadzajace)
        {
            var wynik = new List<string>();

            foreach (var rola in roleZarzadzajace)
            {
                var idRoli = await _pobierajRole.PobierzIdRoliPoNazwieAsync(rola);

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

        public async Task<List<string>> PobierzIdRolKtorymiMozeZarzadzacAsync(List<string> roleZarzadzajace)
        {
            var wynik = new List<string>();

            foreach (var rola in roleZarzadzajace)
            {
                var idRoli = await _pobierajRole.PobierzIdRoliPoNazwieAsync(rola);

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

            return wynik.Distinct().ToList();
        }

        public async Task<bool> CzyMozeZarzadzacUzytkownikiemAsync(ClaimsPrincipal user, string idUzytkownika)
        {
            var (roleDoZarzadzania, roleUzytkownika) = await PobierzRoleUzytkownikaIZarzadzane(user, idUzytkownika);
            if (roleUzytkownika == null || roleUzytkownika.Count == 0)
            {
                return false;
            }
            return roleUzytkownika.All(x => roleDoZarzadzania.Contains(x));
        }

        public async Task<bool> CzyMozeWidziecUzytkownikaAsync(ClaimsPrincipal user, string idUzytkownika)
        {
            var (roleDoZarzadzania, roleUzytkownika) = await PobierzRoleUzytkownikaIZarzadzane(user, idUzytkownika);
            if (roleUzytkownika == null || roleUzytkownika.Count == 0)
            {
                return (false);
            }
            return roleUzytkownika.Any(x => roleDoZarzadzania.Contains(x));
        }

        public async Task<(bool CzyMoze, string Komunikat)> CzyMozeZarzadzacRolaAsync(ClaimsPrincipal user, string idRoli)
        {
            var roleUzytkownika = await _pobierajRole.PobierzRoleUzytkownikaPoIdUzytkownikaAsync(user.FindFirstValue(ClaimTypes.NameIdentifier));
            List<string> listaZarzadzanychRol = await PobierzIdRolKtorymiMozeZarzadzacAsync(roleUzytkownika);
            if (listaZarzadzanychRol.Count == 0)
            {
                return (false, "Brak zarzadzanych rol");
            }
            return listaZarzadzanychRol.Contains(idRoli) ? (true, "Może zarządzać") : (false, "Nie może zarządzać");
        }
        

        public async Task<bool> CzyRolaJestUzywanaWHierarchiiAsync(string idRoli)
        {
            return await _context.UprawnieniaZarzadzaniaRola.AnyAsync(x => x.RolaZarzadzajacaId == idRoli || x.RolaZarzadzanaId == idRoli);
        }

        public async Task<bool> CzyRolaMaPowiazaniaPozaAdminemAsync(string idRoli, string idAdmina)
        {
            return await _context.UprawnieniaZarzadzaniaRola.AnyAsync(x =>
                x.RolaZarzadzajacaId == idRoli ||
                (x.RolaZarzadzanaId == idRoli && x.RolaZarzadzajacaId != idAdmina));
        }

        private async Task<(List<string> roleDoZarzadzania, List<string> roleUzytkownika)> PobierzRoleUzytkownikaIZarzadzane(ClaimsPrincipal user, string idUzytkownika)
        {
            var roleZalogowanego = await _pobierajRole.PobierzRoleZalogowanegoUzytkownikaAsync(user);

            var roleUzytkownika = await _pobierajRole.PobierzRoleUzytkownikaPoIdUzytkownikaAsync(idUzytkownika);

            var roleDoZarzadzania = await PobierzNazwyRolKtorymiMozeZarzadzacAsync(roleZalogowanego);

            return (roleDoZarzadzania, roleUzytkownika);
        }
    }
}
