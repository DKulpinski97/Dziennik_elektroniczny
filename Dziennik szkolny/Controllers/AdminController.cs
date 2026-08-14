using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Controllers
{
    public class AdminController : Controller
    {
        private readonly IZarzadzajRolami _roleService;
        private readonly IZarzadzajUzytkownikem _uzytkownikService;
        private readonly AppDbContext _context;
        private readonly UserManager<LoginUzytkownika> _userManager;


        public AdminController(IZarzadzajRolami roleService,AppDbContext appDbContext,UserManager<LoginUzytkownika> userManager,IZarzadzajUzytkownikem uzytkownikService)
        {
            _roleService = roleService;
            _context = appDbContext;
            _userManager = userManager;
            _uzytkownikService = uzytkownikService;
        }


        private async Task UzupelnijRole(UzytkownikaViewModel model)
        {
            var role = await _roleService.PobierzRole();

            model.DostepneRole = role.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.Nazwa
            }).ToList();
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
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nazwa roli nie może być pusta.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }


            var wynik = await _roleService.DodajRole(nazwaRoli.Trim());


            if (!wynik)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Taka rola już istnieje.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Rola dodana pomyślnie.";
            return RedirectToAction(nameof(ZarzadzajRolami));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ZmienNazweRoli(string RoleId,string NowaNazwaRoli,string StaraNazwaRoli)
        {
            if (string.IsNullOrWhiteSpace(RoleId))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano roli.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }
            if (NowaNazwaRoli == StaraNazwaRoli)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nowa nazwa nie może być taka sama jak stara.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }


            var wynik = await _roleService.ZmienNazweRoli(RoleId,NowaNazwaRoli.Trim(),StaraNazwaRoli);


            if (!wynik)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nie udało się zmienić nazwy roli.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Nazwa roli zmieniona pomyślnie.";
            return RedirectToAction(nameof(ZarzadzajRolami));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunRole(string RoleId)
        {
            var wynik = await _roleService.UsunRole(RoleId);


            if (!wynik)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nie udało się usunąć roli.";

                var role = await _roleService.PobierzRole();

                return View("ZarzadzajRolami", role);
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Rola usunięta pomyślnie.";
            return RedirectToAction(nameof(ZarzadzajRolami));
        }



        /*=================Dodawanie Użytkownika==================*/


        [HttpGet]
        public async Task<IActionResult> DodajUzytkownika()
        {
            var model = new UzytkownikaViewModel();

            await UzupelnijRole(model);
            ViewBag.TrybDodawania = true;
            return View("DaneUżytkonikaKontrola", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajUzytkownika(UzytkownikaViewModel model)
        {
            ViewBag.TrybDodawania = true;
            if (model.WybraneRole == null || model.WybraneRole.Count == 0)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Musisz wybrać przynajmniej jedną rolę.";

                await UzupelnijRole(model);

                return View("DaneUżytkonikaKontrola", model);
            }
            if (!ModelState.IsValid)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Przynajmiej jedno pole jest nie uzupełnione lub zawiera wadliwe informacjie";
                await UzupelnijRole(model);

                return View("DaneUżytkonikaKontrola", model);
            }

            var wynik =await _uzytkownikService.DodajUzytkownikaAsync(model);

            TempData["TypWiadomosci"] = "info";
            TempData["Wiadomosc"] = wynik.Komunikat;


            if (wynik.CzyUdane == true)
            {
                TempData["TypWiadomosci"] = "success";
                return RedirectToAction(nameof(DodajUzytkownika));
            }


            await UzupelnijRole(model);

            return View("DaneUżytkonikaKontrola", model);
        }
        public async Task<IActionResult> PriperStartUzytkownika(
            UzytkownikaViewModel model)
        {
            if (TempData["Wiadomosc"]?.ToString() == "Użytkownik dodany")
            {
                return RedirectToAction(nameof(DodajUzytkownika));
            }


            await UzupelnijRole(model);

            return View(model);
        }



        /*=================Wybór Użytkowników==================*/


        [HttpGet]
        public async Task<IActionResult> ZarządzajUżytkownikem()
        {
            var uzytkownicy = await _userManager.Users.ToListAsync();

            var informacje = await _context.InformacjeUzytkownik
                .ToListAsync();


            List<SelectListItem> rodzice = new();

            List<SelectListItem> pracownicy = new();


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
                var roleUzytkownika =await _userManager.GetRolesAsync(uzytkownik);


                var dane = informacje.FirstOrDefault(x =>x.IdUzytkownika == uzytkownik.Id);


                if (dane == null)
                {
                    continue;
                }


                var element = new SelectListItem
                {
                    Value = uzytkownik.Id,
                    Text = $"{dane.Imie} {dane.Nazwisko}"
                };


                bool jestPracownikiem =roleUzytkownika.Any(r =>rolePracownikow.Contains(r));


                bool jestRodzicem = roleUzytkownika.Any(r =>r == "Rodzic");


                // pracownik ma pierwszeństwo
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




        /*=================Przygotowanie danych do edycji==================*/


        [HttpGet]
        public async Task<IActionResult> PrzygotujUzytkownikaDoEdycji(string idUzytkownika)
        {
            if (string.IsNullOrEmpty(idUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] ="Nie wybrano użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }



            var uzytkownik =
                await _userManager.FindByIdAsync(idUzytkownika);


            if (uzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] ="Nie znaleziono użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }



            var informacjeUzytkownik =await _context.InformacjeUzytkownik.FirstOrDefaultAsync(x => x.IdUzytkownika == idUzytkownika);



            if (informacjeUzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] ="Brak danych osobowych użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }

            var viewModelUzytkownika = await _uzytkownikService.PrzygotujDaneDoEdycjiAsync(informacjeUzytkownik,idUzytkownika);

            ViewBag.TrybDodawania = false;
            return View("DaneUżytkonikaKontrola", viewModelUzytkownika);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EdytujDaneUzytkownika(UzytkownikaViewModel viewModelUzytkownika)
        {
            if (viewModelUzytkownika == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Błąd podczas próby edycji danych użytkownika.";
                return RedirectToAction(nameof(ZarządzajUżytkownikem));
               
            }
            UzupelnijRole(viewModelUzytkownika);
            var wynik = await _uzytkownikService.EdytujUzytkownikaAsync(viewModelUzytkownika);

            TempData["TypWiadomosci"] = wynik.Sukces ? "success" : "danger";
            TempData["Wiadomosc"] = wynik.Komunikat;
            if (wynik.Sukces)
            {
                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }
            else
            {
                return View("DaneUżytkonikaKontrola", viewModelUzytkownika);
            }
        }

        /*=================Usuwanie użytkownika==================*/


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunUzytkownika(
            string IdUzytkownika)
        {
            if (string.IsNullOrEmpty(IdUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] ="Nie wybrano użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }



            var uzytkownik =
                await _userManager.FindByIdAsync(IdUzytkownika);



            if (uzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] ="Nie znaleziono użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }



            var informacje =await _context.InformacjeUzytkownik.FirstOrDefaultAsync(x => x.IdUzytkownika == IdUzytkownika);



            if (informacje != null)
            {
                _context.InformacjeUzytkownik.Remove(informacje);

                await _context.SaveChangesAsync();
            }



            var wynik =
                await _userManager.DeleteAsync(uzytkownik);



            if (!wynik.Succeeded)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] ="Nie udało się usunąć użytkownika.";

                return RedirectToAction(
                    nameof(ZarządzajUżytkownikem));
            }



            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] ="Użytkownik został usunięty.";


            return RedirectToAction(nameof(ZarządzajUżytkownikem));
        }
    }
}