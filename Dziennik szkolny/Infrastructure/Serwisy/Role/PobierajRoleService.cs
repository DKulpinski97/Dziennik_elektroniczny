using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Modele;
using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dziennik_szkolny.Infrastructure.Serwisy.Role
{
    public class PobierajRoleService : IPobierajRole

    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<LoginUzytkownika> _userManager;
        private readonly AppDbContext _context;
        public PobierajRoleService(RoleManager<IdentityRole> roleManager, UserManager<LoginUzytkownika> userManager, AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task<List<DaneRoli>> PobierzRole(ClaimsPrincipal user)
        {
            var roleZalogowanego = await PobierzRoleZalogowanegoUzytkownikaAsync(user);

            var idRolZalogowanego = await _roleManager.Roles
                .Where(x => roleZalogowanego.Contains(x.Name))
                .Select(x => x.Id)
                .ToListAsync();

            var idRolDoZarzadzania = await _context.UprawnieniaZarzadzaniaRola
                .Where(x => idRolZalogowanego.Contains(x.RolaZarzadzajacaId))
                .Select(x => x.RolaZarzadzanaId)
                .Distinct()
                .ToListAsync();

            return await _roleManager.Roles
                .Where(x => idRolDoZarzadzania.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new DaneRoli
                {
                    Id = x.Id,
                    Nazwa = x.Name
                })
                .ToListAsync();
        }
        public async Task<bool> CzyIstniejaRoleAsync(List<string> idRoli)
        {
            if (idRoli == null || !idRoli.Any())
            {
                return true;
            }


            foreach (var idRoliItem in idRoli)
            {
                var rola = await _roleManager.FindByIdAsync(idRoliItem);


                if (rola == null)
                {
                    return false;
                }
            }


            return true;
        }
        public async Task<IdentityRole?> PobierzRolePoIdAsync(string idRoli)
        {
            return await _roleManager.FindByIdAsync(idRoli);
        }
        public async Task<IdentityRole?> PobierzRolePoNazwie(string nazwa)
        {
            return await _roleManager.FindByNameAsync(nazwa);
        }
        public async Task<IList<string>> PobierzRoleUzytkownikaPoLoginieAsync(string login)
        {
            var uzytkownik = await _userManager.FindByNameAsync(login);

            if (uzytkownik == null)
            {
                return new List<string>();
            }

            return await _userManager.GetRolesAsync(uzytkownik);
        }
        public async Task<List<string>> PobierzNazwyRolPoIdAsync(List<string> idRol)
        {
            if (idRol == null || idRol.Count == 0)
            {
                return new List<string>();
            }

            return await _roleManager.Roles.Where(x => idRol.Contains(x.Id)).Select(x => x.Name).ToListAsync();
        }

        public async Task<List<string>> PobierzRoleZalogowanegoUzytkownikaAsync(ClaimsPrincipal user)
        {
            var uzytkownik = await _userManager.GetUserAsync(user);

            if (uzytkownik == null)
            {
                return [];
            }

            return (await _userManager.GetRolesAsync(uzytkownik)).ToList();
        }
        public async Task<string?> PobierzIdRoliPoNazwieAsync(string nazwaRoli)
        {
            var rola = await _roleManager.FindByNameAsync(nazwaRoli);

            return rola?.Id;
        }
        public async Task<List<string>> PobierzRoleUzytkownikaPoIdAsync(string idUzytkownika)
        {
            var uzytkownik = await _userManager.FindByIdAsync(idUzytkownika);

            if (uzytkownik == null)
            {
                return new List<string>();
            }

            return (await _userManager.GetRolesAsync(uzytkownik)).ToList();
        }

    }

}
