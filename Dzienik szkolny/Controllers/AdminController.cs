using Dzienik_szkolny.Data;
using Dzienik_szkolny.Models;
using Dzienik_szkolny.Services.Interfaces;
using Dzienik_szkolny.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Dzienik_szkolny.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IUzytkownikService _uzytkownikService;
        private readonly AppDbContext _context;
        private readonly UserManager<LoginUzytkownika> _userManager;

        public AdminController(IRoleService roleService, AppDbContext appDbContext, UserManager<LoginUzytkownika> userManager, IUzytkownikService uzytkownikService)
        {
            _roleService = roleService;
            _context = appDbContext;
            _userManager = userManager;
            _uzytkownikService = uzytkownikService;
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

        /*=================Dodawanie Użytkownikami==================*/
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
            TempData["Wiadomosc"] = _uzytkownikService.DodajUzytkownikaAsync(model).Result.Komunikat;
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
        public async Task<IActionResult> PriperStartUzytkownika(DodajUzytkownikaViewModel model)
        {

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
        /*=================Wybór Użytkownikami==================*/
        [HttpGet]
        public async Task<IActionResult> EdytujDaneUzytkownika()
        {
            DodajUzytkownikaViewModel dodajUzytkownikaViewModel = null; 
            var uzytkownicy = await _userManager.Users.ToListAsync();

            var informacje = await _context.InformacjeUzytkownik
                .ToListAsync();


            List<SelectListItem> rodzice = new();
            List<SelectListItem> pracownicy = new();


            // Role, które traktujemy jako pracowników szkoły
            var rolePracownikow = new List<string>
    {
        "Admin",
        "Nauczyciel",
        "Dyrektor",
        "ViceDyrektor",
        "Sekretarka"
    };


            foreach (var uzytkownik in uzytkownicy)
            {
                var roleUzytkownika = await _userManager.GetRolesAsync(uzytkownik);


                var dane = informacje
                    .FirstOrDefault(x => x.IdUzytkownika == uzytkownik.Id);


                if (dane == null)
                {
                    continue;
                }


                var element = new SelectListItem
                {
                    Value = uzytkownik.Id,
                    Text = $"{dane.Imie} {dane.Nazwisko}"
                };


                bool jestPracownikiem = roleUzytkownika.Any(r => rolePracownikow.Contains(r));


                bool jestRodzicem = roleUzytkownika.Any(r => r == "Rodzić");


                // Pracownik jest piorytetem
                if (jestPracownikiem)
                {
                    pracownicy.Add(element);
                }
                else if (jestRodzicem)
                {
                    rodzice.Add(element);
                }
            }


            ViewBag.Rodzice = rodzice;
            ViewBag.Pracownicy = pracownicy;


            return View();
        }
        /*=================Przygotuj dane do edycji==================*/
        [HttpGet]
        public async Task<IActionResult> PrzygotujUżytkonikaDoEdycji(string idUzytkownika)
        {
            if (string.IsNullOrEmpty(idUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano użytkownika.";
                return RedirectToAction(nameof(EdytujDaneUzytkownika));
            }


            // Pobranie użytkownika z Identity
            var uzytkownik = await _userManager.FindByIdAsync(idUzytkownika);


            if (uzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie znaleziono użytkownika.";
                return RedirectToAction(nameof(EdytujDaneUzytkownika));
            }


            // Pobranie danych osobowych
            var informacjeUzytkownik = await _context.InformacjeUzytkownik.FirstOrDefaultAsync(x => x.IdUzytkownika == idUzytkownika);


            if (informacjeUzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Brak danych osobowych użytkownika.";
                return RedirectToAction(nameof(EdytujDaneUzytkownika));
            }


            // Przygotowanie danych do formularza edycji
            var viewModel = await _uzytkownikService.PrzygotujDaneDoEdycjiAsync(informacjeUzytkownik, idUzytkownika);
            var role = await _roleService.PobierzRole();
            
            return View("WidokEdycjiUzytkownika", viewModel);
        }
        /*=================usuwanie konkretnego Użytkownika==================*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunUzytkownika(string IdUzytkownika)
        {
            if (string.IsNullOrEmpty(IdUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano użytkownika.";
                return RedirectToAction(nameof(EdytujDaneUzytkownika));
            }


            // Pobranie użytkownika Identity
            var uzytkownik = await _userManager.FindByIdAsync(IdUzytkownika);


            if (uzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie znaleziono użytkownika.";
                return RedirectToAction(nameof(EdytujDaneUzytkownika));
            }


            // Usunięcie danych osobowych
            var informacje = await _context.InformacjeUzytkownik
                .FirstOrDefaultAsync(x => x.IdUzytkownika == IdUzytkownika);


            if (informacje != null)
            {
                _context.InformacjeUzytkownik.Remove(informacje);
                await _context.SaveChangesAsync();
            }


            // Usunięcie konta Identity
            var wynik = await _userManager.DeleteAsync(uzytkownik);


            if (!wynik.Succeeded)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nie udało się usunąć użytkownika.";

                return RedirectToAction(nameof(EdytujDaneUzytkownika));
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Użytkownik został usunięty.";


            return RedirectToAction(nameof(EdytujDaneUzytkownika));
        }
    }
}