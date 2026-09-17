using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Domain;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Mapery;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.Infrastructure.Serwisy.Role;
using Dziennik_szkolny.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dziennik_szkolny.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IZarzadzajRolami _roleService;
        private readonly IZarzadzajUzytkownikem _uzytkownikService;
        private readonly MapowanieRoli _mapowanieRoli;
        private readonly MapowanieUzytkownika _mapowanieUzytkownika;
        private readonly IPobierajRole _pobierajRole;
        private readonly IObslugaUzytkownika _obslugaUzytkownika;
        private readonly IPobierajUzytkownika _pobierajUzytkownika;
        private readonly IPobierajUprawnieniaRoli _pobierajUprawnieniaRoli;


        public AdminController(IZarzadzajRolami roleService, IZarzadzajUzytkownikem uzytkownikService, IPobierajRole pobierajRole,
            IObslugaUzytkownika obslugaUzytkownika, IPobierajUzytkownika pobierajUzytkownika, IPobierajUprawnieniaRoli pobierajUprawnieniaRoli)
        {
            _roleService = roleService;
            _uzytkownikService = uzytkownikService;
            _mapowanieRoli = new MapowanieRoli(); ;
            _pobierajRole = pobierajRole;
            _mapowanieUzytkownika = new MapowanieUzytkownika();
            _obslugaUzytkownika = obslugaUzytkownika;
            _pobierajUzytkownika = pobierajUzytkownika;
            _pobierajUprawnieniaRoli = pobierajUprawnieniaRoli;
        }
        /*=================Zarządzanie Rolami==================*/

        [HttpGet]
        [Authorize(Roles = NazwyRoli.Admin)]
        public async Task<IActionResult> ZarzadzajRolami()
        {
            var role = await _pobierajRole.PobierzRole(User);

            return View(role);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = NazwyRoli.Admin)]
        public async Task<IActionResult> DodajRole(string nazwaRoli)
        {
            if (string.IsNullOrWhiteSpace(nazwaRoli))
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nazwa roli nie może być pusta.";

                var role = await _pobierajRole.PobierzRole(User);

                return View("ZarzadzajRolami", role);
            }


            var wynik = await _roleService.DodajRole(nazwaRoli.Trim());


            if (!wynik)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Taka rola już istnieje.";

                var role = await _pobierajRole.PobierzRole(User);

                return View("ZarzadzajRolami", role);
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Rola dodana pomyślnie.";
            return RedirectToAction(nameof(ZarzadzajRolami));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = NazwyRoli.Admin)]
        public async Task<IActionResult> ZmienNazweRoli(string RoleId, string NowaNazwaRoli, string StaraNazwaRoli)
        {
            if (string.IsNullOrWhiteSpace(RoleId))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano roli.";

                var role = await _pobierajRole.PobierzRole(User);

                return View("ZarzadzajRolami", role);
            }
            if (NowaNazwaRoli == StaraNazwaRoli)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nowa nazwa nie może być taka sama jak stara.";

                var role = await _pobierajRole.PobierzRole(User);

                return View("ZarzadzajRolami", role);
            }


            var wynik = await _roleService.ZmienNazweRoli(RoleId, NowaNazwaRoli.Trim(), StaraNazwaRoli);

            if (!wynik.Sukces)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = wynik.Komunikat;

                var role = await _pobierajRole.PobierzRole(User);

                return View("ZarzadzajRolami", role);
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = wynik.Komunikat;

            return RedirectToAction(nameof(ZarzadzajRolami));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = NazwyRoli.Admin)]
        public async Task<IActionResult> UsunRole(string RoleId)
        {
            if (string.IsNullOrWhiteSpace(RoleId))
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Należy wybrać rolę do usunięcia.";

                var role = await _pobierajRole.PobierzRole(User);
                return View("ZarzadzajRolami", role);
            }

            var wynik = await _roleService.UsunRole(RoleId);


            if (!wynik.Sukces)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = wynik.Komunikat;

                var role = await _pobierajRole.PobierzRole(User);

                return View("ZarzadzajRolami", role);
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = wynik.Komunikat;

            return RedirectToAction(nameof(ZarzadzajRolami));
        }



        /*=================Dodawanie Użytkownika==================*/


        [HttpGet]
        [Authorize(Roles = NazwyRoli.SuperAdminAdminDyrektor)]
        public async Task<IActionResult> DodajUzytkownika()
        {
            var role = await _pobierajRole.PobierzRole(User);

            UzytkownikaViewModel uzytkownikaViewModel = _mapowanieUzytkownika.MapujDostepneRoleVievModel(role);
            ViewBag.TrybDodawania = true;
            return View("DaneUżytkownikaKontrola", uzytkownikaViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = NazwyRoli.SuperAdminAdminDyrektor)]
        public async Task<IActionResult> DodajUzytkownika(UzytkownikaViewModel uzytkownikaViewModel)
        {
            ViewBag.TrybDodawania = true;
            if (uzytkownikaViewModel.WybraneRole == null || uzytkownikaViewModel.WybraneRole.Count == 0)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Musisz wybrać przynajmniej jedną rolę.";
                var role = await _pobierajRole.PobierzRole(User);
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(uzytkownikaViewModel, role);
                return View("DaneUżytkownikaKontrola", uzytkownikaViewModel);
            }
            if (string.IsNullOrWhiteSpace(uzytkownikaViewModel.Haslo))
            {
                ModelState.AddModelError(
                    nameof(uzytkownikaViewModel.Haslo),
                    "Hasło jest nie prawidłowe.");

                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Hasło jest nie prawidłowe.";
                var role = await _pobierajRole.PobierzRole(User);
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(uzytkownikaViewModel, role);
                return View("DaneUżytkownikaKontrola", uzytkownikaViewModel);
            }
            if (!ModelState.IsValid)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Przynajmiej jedno pole jest nie uzupełnione lub zawiera wadliwe informacjie";
                var role = await _pobierajRole.PobierzRole(User);
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(uzytkownikaViewModel, role);
                return View("DaneUżytkownikaKontrola", uzytkownikaViewModel);
            }

            var wynik = await _obslugaUzytkownika.DodajUzytkownikaAsync(uzytkownikaViewModel, User);

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
                var role = await _pobierajRole.PobierzRole(User);
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(uzytkownikaViewModel, role);
                return View("DaneUżytkownikaKontrola", uzytkownikaViewModel);
            }
        }


        /*=================Wybór Użytkowników==================*/


        [HttpGet]
        [Authorize(Roles = NazwyRoli.SuperAdminAdminDyrektor)]
        public async Task<IActionResult> ZarzadzajUzytkownikem()
        {
            var wynik = await _obslugaUzytkownika.PodzielUzytkownikowNaRodzicowIPracownikowAsync(User);

            ViewBag.Rodzice = wynik.Rodzice;
            ViewBag.Pracownicy = wynik.Pracownicy;

            return View();
        }




        /*=================Przygotowanie danych do edycji==================*/


        [HttpGet]
        [Authorize(Roles = NazwyRoli.SuperAdminAdminDyrektor)]
        public async Task<IActionResult> PrzygotujUzytkownikaDoEdycji(string idUzytkownika)
        {
            if (string.IsNullOrEmpty(idUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano użytkownika.";

                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }
            var mozeZarzadzac = await _pobierajUprawnieniaRoli.CzyMozeZarzadzacUzytkownikiemAsync(User,idUzytkownika);

            if (!mozeZarzadzac)
            {
                return Forbid();
            }


            LoginUzytkownika uzytkownik = await _pobierajUzytkownika.PobierzUzytkownikaPoIDAsync(idUzytkownika);


            if (uzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie znaleziono użytkownika.";

                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }



            InformacjeUzytkownik informacjeUzytkownik = await _pobierajUzytkownika.PobierzInformacjeUzytkownikaPoIdLoginu(idUzytkownika);



            if (informacjeUzytkownik == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Brak danych osobowych użytkownika.";

                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }
            var dostempneRole = _mapowanieRoli.MapujRoleNaSelectList(await _pobierajRole.PobierzRole(User));
            var przypisaneRole = await _pobierajRole.PobierzRoleUzytkownikaPoLoginieAsync(uzytkownik.UserName);
            var viewModelUzytkownika = _mapowanieUzytkownika.MapujNaUzytkownikaViewModel(informacjeUzytkownik, uzytkownik, przypisaneRole.ToList(), dostempneRole);
            ViewBag.TrybDodawania = false;
            return View("DaneUżytkownikaKontrola", viewModelUzytkownika);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = NazwyRoli.SuperAdminAdminDyrektor)]
        public async Task<IActionResult> EdytujDaneUzytkownika(UzytkownikaViewModel viewModelUzytkownika)
        {
            if (viewModelUzytkownika == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Błąd podczas próby edycji danych użytkownika.";
                return RedirectToAction(nameof(ZarzadzajUzytkownikem));

            }

            var mozeZarzadzac = await _pobierajUprawnieniaRoli
                .CzyMozeZarzadzacUzytkownikiemAsync(User, viewModelUzytkownika.idUzytkownika);

            if (!mozeZarzadzac)
            {
                return Forbid();
            }

            _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(viewModelUzytkownika, await _pobierajRole.PobierzRole(User));
            var wynik = await _obslugaUzytkownika.EdytujUzytkownikaAsync(viewModelUzytkownika, User);


            TempData["TypWiadomosci"] = wynik.CzyUdane ? "success" : "danger";
            TempData["Wiadomosc"] = wynik.Komunikat;

            if (wynik.CzyUdane)
            {
                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }

            return View("DaneUżytkownikaKontrola", wynik.Uzytkownik);
        }

        /*=================Usuwanie użytkownika==================*/


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = NazwyRoli.SuperAdminAdminDyrektor)]
        public async Task<IActionResult> UsunUzytkownika(string IdUzytkownika)
        {
            if (string.IsNullOrEmpty(IdUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie wybrano użytkownika.";

                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }

            var mozeZarzadzac = await _pobierajUprawnieniaRoli
                .CzyMozeZarzadzacUzytkownikiemAsync(User, IdUzytkownika);

            if (!mozeZarzadzac)
            {
                return Forbid();
            }

            var wynik = await _uzytkownikService.UsunUzytkownika(IdUzytkownika);


            if (!wynik)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nie udało się usunąć użytkownika.";

                return RedirectToAction(
                    nameof(ZarzadzajUzytkownikem));
            }



            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Użytkownik został usunięty.";


            return RedirectToAction(nameof(ZarzadzajUzytkownikem));
        }
    }
}
