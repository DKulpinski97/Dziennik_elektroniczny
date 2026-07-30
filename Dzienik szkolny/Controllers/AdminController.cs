using Dzienik_szkolny.Data;
using Dzienik_szkolny.Models;
using Dzienik_szkolny.Services;
using Dzienik_szkolny.Services.Interfaces;
using Dzienik_szkolny.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Dzienik_szkolny.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IUzytkownikService _UzytkownikService;
        private readonly AppDbContext _context;
        private readonly UserManager<LoginUzytkownika> _userManager;

        public AdminController(IRoleService roleService, AppDbContext appDbContext, UserManager<LoginUzytkownika> userManager, IUzytkownikService uzytkownikService )
        {
            _roleService = roleService;
            _context = appDbContext;
            _userManager = userManager;
            _UzytkownikService = uzytkownikService;
        }

        /*=================Zarządzanie Rolami==================*/
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
        public async Task<IActionResult> ZmienNazweRoli(string RoleId, string NowaNazwaRoli, string StaraNazwaRoli)
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

        /*=================Zarządzanie Użytkownikami==================*/
        [HttpGet]
        public async Task<IActionResult> DodajUzytkownika()
        {
            var model = new DodajUzytkownikaViewModel
            {
                Role = await _roleService.PobierzRole()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajUzytkownika(DodajUzytkownikaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Role = await _roleService.PobierzRole();
                return View(model);
            }
            if (model.IdRoli.Count == 0)
            {
                TempData["Wiadomosc"] = "Musisz wybrać przynajmiej jedną role";
                model.Role = await _roleService.PobierzRole();
                return View(model);
            }
            TempData["Wiadomosc"] = _UzytkownikService.DodajUzytkownikaAsync(model).Result.Komunikat;
            if (TempData["Wiadomosc"].ToString() == "Użytkownik dodany")
            {
                return RedirectToAction("DodajUzytkownika");
            }
            else
            {
                model.Role = await _roleService.PobierzRole();
                return View(model);
            }
        }
    }
}