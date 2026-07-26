using Dzienik_szkolny.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dzienik_szkolny.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRoleService _roleService;

        public AdminController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        [HttpGet]
        public async Task<IActionResult> ZarzadzajRolami()
        {
            var role = await _roleService.PobierzRole();

            return View(role);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajRole(string nazwaRoli)
        {
            if (string.IsNullOrWhiteSpace(nazwaRoli))
            {
                ViewBag.Komunikat = "Nazwa roli nie może być pusta.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }


            var wynik = await _roleService.DodajRole(nazwaRoli.Trim());


            if (!wynik)
            {
                ViewBag.Komunikat = "Taka rola już istnieje.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }


            return RedirectToAction(nameof(ZarzadzajRolami));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ZmienNazweRoli(string RoleId, string NowaNazwaRoli,string StaraNazwaRoli)
        {
            if (string.IsNullOrWhiteSpace(NowaNazwaRoli))
            {
                ViewBag.Komunikat = "Nowa nazwa roli nie może być pusta.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }


            var wynik = await _roleService.ZmienNazweRoli(
                RoleId,
                NowaNazwaRoli.Trim(),
                StaraNazwaRoli
            );


            if (!wynik)
            {
                ViewBag.Komunikat = "Nie udało się zmienić nazwy roli.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }


            return RedirectToAction(nameof(ZarzadzajRolami));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunRole(string RoleId)
        {
            var wynik = await _roleService.UsunRole(RoleId);

            if (!wynik)
            {
                ViewBag.Komunikat = "Nie udało się usunąć roli.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }

            return RedirectToAction(nameof(ZarzadzajRolami));
        }
    }
}