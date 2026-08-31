using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DodajUprawnieniaZarzadzaniaRoli
    {
        private readonly AppDbContext _context;
        private readonly DaneStartowe _daneStartowe;
        private readonly IPobierajRole _pobierajRole;
        public DodajUprawnieniaZarzadzaniaRoli(AppDbContext context, DaneStartowe daneStartowe, IPobierajRole pobierajRole)
        {
            _context = context;
            _daneStartowe = daneStartowe;
            _pobierajRole = pobierajRole;
        }
        public async Task DodajUprawnieniaZarzadzaniaRoliAsync()
        {
            foreach (var uprawnienie in _daneStartowe.UprawnieniaRoli)
            {
                var rolaZarzadzajaca =
                    await _pobierajRole.PobierzRolePoNazwie(uprawnienie.RolaZarzadzajaca);

                var rolaZarzadzana =
                    await _pobierajRole.PobierzRolePoNazwie(uprawnienie.RolaZarzadzana);

                if (rolaZarzadzajaca == null || rolaZarzadzana == null)
                {
                    continue;
                }

                var istnieje = await _context.UprawnieniaZarzadzaniaRola.AnyAsync(x =>
                        x.RolaZarzadzajacaId == rolaZarzadzajaca.Id &&
                        x.RolaZarzadzanaId == rolaZarzadzana.Id);

                if (!istnieje)
                {
                    _context.UprawnieniaZarzadzaniaRola.Add(new UprawnienieZarzadzaniaRola
                        {
                            RolaZarzadzajacaId = rolaZarzadzajaca.Id,
                            RolaZarzadzanaId = rolaZarzadzana.Id
                        });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
