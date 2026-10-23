using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Dziennik_szkolny.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dziennik_szkolny.Controllers.APIRole
{
    [ApiController]
    [Route("api/role")]
    [Authorize(Roles = NazwyRoli.Admin)]
    public class RoleApiController : ControllerBase
    {
        private readonly IPobierajRole _pobierajRole;
        private readonly IPobierajUprawnieniaRoli _pobierajUprawnieniaRoli;
        private readonly IZarzadzajRolami _zarzadzajRolami;

        public RoleApiController(IPobierajRole pobierajRole, IPobierajUprawnieniaRoli pobierajUprawnieniaRoli, IZarzadzajRolami zarzadzajRolami)
        {
            _pobierajRole = pobierajRole;
            _pobierajUprawnieniaRoli = pobierajUprawnieniaRoli;
            _zarzadzajRolami = zarzadzajRolami;
        }

        [HttpGet]
        public async Task<ActionResult<List<DaneRoli>>> PobierzZarzadzaneRole()
        {
            var role = await _pobierajRole.PobierzRole(User);
            return Ok(role);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DaneRoli>> PobierzRole(string id)
        {
            var uprawnienie = await _pobierajUprawnieniaRoli.CzyMozeZarzadzacRolaAsync(User, id);
            if (!uprawnienie.CzyMoze)
            {
                return NotFound();
            }

            var rola = await _pobierajRole.PobierzRolePoIdAsync(id);
            if (rola == null)
            {
                return NotFound();
            }

            var daneRoli = new DaneRoli
            {
                Id = rola.Id,
                Nazwa = rola.Name
            };
            return Ok(daneRoli);
        }
        [HttpPost]

        public async Task<ActionResult<DaneRoli>> UtworzRole([FromBody] NoweDaneRoli noweDaneRoli)
        {
            string idAdmina = await _pobierajRole.PobierzIdRoliPoNazwieAsync(NazwyRoli.Admin);
            if (string.IsNullOrEmpty(idAdmina))
            {
                return Problem(detail: "Nie udało się odczytać roli Admin.", statusCode: StatusCodes.Status500InternalServerError);
            }
            var wynik = await _zarzadzajRolami.KontrolaDodawaRoliAsync(idAdmina, noweDaneRoli.Nazwa);
            if (!wynik.Sukces)
            {
                return Problem(detail: wynik.Komunikat, statusCode: StatusCodes.Status400BadRequest);
            }
            var rola = await _pobierajRole.PobierzRolePoNazwieAsync(noweDaneRoli.Nazwa);
            if (rola == null)
            {
                return Problem(detail: "Nie udało się odczytać utworzonej roli.", statusCode: StatusCodes.Status500InternalServerError);
            }
            var daneRoli = new DaneRoli
            {
                Id = rola.Id,
                Nazwa = rola.Name
            };
            return CreatedAtAction(nameof(PobierzRole), new { id = rola.Id }, daneRoli);
        }
    }
}