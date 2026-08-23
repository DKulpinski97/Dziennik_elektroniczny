using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Mapery;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Dziennik_szkolny.Controllers
{
    public class AdminController : Controller
    {
        private readonly IZarzadzajRolami _roleService;
        private readonly IZarzadzajUzytkownikem _uzytkownikService;
        private readonly MapowanieRoli _mapowanieRoli;
        private readonly MapowanieUzytkownika _mapowanieUzytkownika;
        private readonly IPobierajRole _pobierajRole;
        private readonly IObslugaUzytkownika _obslugaUzytkownika;
        private readonly IPobierajUzytkownika _pobierajUzytkownika;


        public AdminController(IZarzadzajRolami roleService, AppDbContext appDbContext, IZarzadzajUzytkownikem uzytkownikService,
            MapowanieRoli mapowanieRoli, MapowanieUzytkownika mapowanieUzytkownika, IPobierajRole pobierajRole, 
            IObslugaUzytkownika obslugaUzytkownika, IPobierajUzytkownika pobierajUzytkownika)
        {
            _roleService = roleService;
            _uzytkownikService = uzytkownikService;
            _mapowanieRoli = mapowanieRoli;
            _pobierajRole = pobierajRole;
            _mapowanieUzytkownika = mapowanieUzytkownika;
            _obslugaUzytkownika = obslugaUzytkownika;
            _pobierajUzytkownika = pobierajUzytkownika;
        }
        /*=================Zarządzanie Rolami==================*/

        [HttpGet]
        public async Task<IActionResult> ZarzadzajRolami()
        {
            var role = await _pobierajRole.PobierzRole();

            UzytkownikaViewModel uzytkownikaViewModel = _mapowanieUzytkownika.MapujDostepneRoleVievModel(role);

            return View(uzytkownikaViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajRole(string nazwaRoli)
        {
            if (string.IsNullOrWhiteSpace(nazwaRoli))
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nazwa roli nie może być pusta.";

                var role = await _pobierajRole.PobierzRole();

                return View("ZarzadzajRolami", role);
            }


            var wynik = await _roleService.DodajRole(nazwaRoli.Trim());


            if (!wynik)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Taka rola już istnieje.";

                var role = await _pobierajRole.PobierzRole();

                return View("ZarzadzajRolami", role);
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Rola dodana pomyślnie.";
            return RedirectToAction(nameof(ZarzadzajRolami));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ZmienNazweRoli(string RoleId, string NowaNazwaRoli, string StaraNazwaRoli)
        {
            if (string.IsNullOrWhiteSpace(RoleId))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano roli.";

                var role = await _pobierajRole.PobierzRole();

                return View("ZarzadzajRolami", role);
            }
            if (NowaNazwaRoli == StaraNazwaRoli)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nowa nazwa nie może być taka sama jak stara.";

                var role = await _pobierajRole.PobierzRole();

                return View("ZarzadzajRolami", role);
            }


            var wynik = await _roleService.ZmienNazweRoli(RoleId, NowaNazwaRoli.Trim(), StaraNazwaRoli);


            if (!wynik)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nie udało się zmienić nazwy roli.";

                var role = await _pobierajRole.PobierzRole();

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

                var role = await _pobierajRole.PobierzRole();

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
            var role = await _pobierajRole.PobierzRole();

            UzytkownikaViewModel uzytkownikaViewModel = _mapowanieUzytkownika.MapujDostepneRoleVievModel(role);
            ViewBag.TrybDodawania = true;
            return View("DaneUżytkonikaKontrola", uzytkownikaViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajUzytkownika(UzytkownikaViewModel uzytkownikaViewModel)
        {
            ViewBag.TrybDodawania = true;
            if (uzytkownikaViewModel.WybraneRole == null || uzytkownikaViewModel.WybraneRole.Count == 0)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Musisz wybrać przynajmniej jedną rolę.";
                var role = await _pobierajRole.PobierzRole();
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(uzytkownikaViewModel, role);
                return View("DaneUżytkonikaKontrola", uzytkownikaViewModel);
            }
            if (!ModelState.IsValid)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Przynajmiej jedno pole jest nie uzupełnione lub zawiera wadliwe informacjie";
                var role = await _pobierajRole.PobierzRole();
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(uzytkownikaViewModel, role);
                return View("DaneUżytkonikaKontrola", uzytkownikaViewModel);
            }

            var wynik = await _obslugaUzytkownika.DodajUzytkownikaAsync(uzytkownikaViewModel);

            if (wynik.CzyUdane)
            {
                TempData["TypWiadomosci"] = "success";
                TempData["Wiadomosc"] = wynik.Komunikat;

                return RedirectToAction(nameof(DodajUzytkownika));
            }
            else
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = wynik.Komunikat;
                var role = await _pobierajRole.PobierzRole();
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(uzytkownikaViewModel, role);
                return View("DaneUżytkonikaKontrola", uzytkownikaViewModel);
            }
        }


        /*=================Wybór Użytkowników==================*/


        [HttpGet]
        public async Task<IActionResult> ZarządzajUżytkownikem()
        {
            var wynik = await _obslugaUzytkownika.PodzielUzytkownikowNaRodzicowIPracownikowAsync();

            ViewBag.Rodzice = wynik.Rodzice;
            ViewBag.Pracownicy = wynik.Pracownicy;

            return View();
        }




        /*=================Przygotowanie danych do edycji==================*/


        [HttpGet]
        public async Task<IActionResult> PrzygotujUzytkownikaDoEdycji(string idUzytkownika)
        {
            if (string.IsNullOrEmpty(idUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }



            LoginUzytkownika uzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoIDAsync(idUzytkownika);


            if (uzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie znaleziono użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }



            InformacjeUzytkownik informacjeUzytkownik = await _pobierajUzytkownika.PobierzInformacjeUzytkownikaPoIdLoginu(idUzytkownika);



            if (informacjeUzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Brak danych osobowych użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }
            var dostempneRole = _mapowanieRoli.MapujRoleNaSelectList(await _pobierajRole.PobierzRole());
            var przypisaneRole =await  _pobierajRole.PobierzRoleUzytkownikaPoLoginieAsync(uzytkownik.UserName);
            var viewModelUzytkownika =  _mapowanieUzytkownika.MapujNaUzytkownikaViewModel(informacjeUzytkownik, uzytkownik, przypisaneRole.ToList(), dostempneRole);
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
            _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(viewModelUzytkownika, await _pobierajRole.PobierzRole());
            var wynik = await _obslugaUzytkownika.EdytujUzytkownikaAsync(viewModelUzytkownika);


            TempData["TypWiadomosci"] = wynik.CzyUdane ? "success" : "danger";
            TempData["Wiadomosc"] = wynik.Komunikat;

            if (wynik.CzyUdane)
            {
                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }

            return View("DaneUżytkonikaKontrola", wynik.Uzytkownik);
        }

        /*=================Usuwanie użytkownika==================*/


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunUzytkownika(string IdUzytkownika)
        {
            if (string.IsNullOrEmpty(IdUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano użytkownika.";

                return RedirectToAction(nameof(ZarządzajUżytkownikem));
            }


            var wynik = await _uzytkownikService.UsunUzytkownika(IdUzytkownika);


            if (!wynik)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nie udało się usunąć użytkownika.";

                return RedirectToAction(
                    nameof(ZarządzajUżytkownikem));
            }



            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Użytkownik został usunięty.";


            return RedirectToAction(nameof(ZarządzajUżytkownikem));
        }
    }
}