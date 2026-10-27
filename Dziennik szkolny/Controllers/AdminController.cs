using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Mapery;
using Dziennik_szkolny.Domain;
using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.DaneStartowe;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.Infrastructure.Serwisy.Role;
using Dziennik_szkolny.ViewModel.Rola;
using Dziennik_szkolny.ViewModel.Uzytkownik;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> KontrolerZarzadzajRolami()
        {

            var role = await _pobierajRole.PobierzRole(User);
            if (role == null || !role.Any())
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Krytyczny błąd: Brak dostępnych ról do zarządzania.Prawdopodonie jest problem z bazą danych.";
                return RedirectToAction("Index", "Home");
            }
            var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
            WidokRoliViewModel widokRoliViewModel = new WidokRoliViewModel
            {
                RoleSystemowe = tmp.RoleSystemowe,
                PozostaleRole = tmp.PozostaleRole
            };

            return View("MenuRol", widokRoliViewModel);
        }
        [HttpGet]
        [Authorize(Roles = NazwyRoli.Admin)]
        public async Task<IActionResult> ZarzadzajRolami()
        {
            var role = await _pobierajRole.PobierzRole(User);
            if (role == null || !role.Any())
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Krytyczny błąd: Brak dostępnych ról. Prawdopodonie jest problem z bazą danych.";
                return RedirectToAction(nameof(KontrolerZarzadzajRolami));
            }
            var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
            return View(tmp.PozostaleRole);
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
                var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
                return View("ZarzadzajRolami", tmp.PozostaleRole);
            }
            string idZarzadzajacejRoli = await _pobierajRole.PobierzIdRoliPoNazwieAsync(NazwyRoli.Admin);
            var wynik = await _roleService.KontrolaDodawaRoliAsync(idZarzadzajacejRoli, nazwaRoli.Trim());


            if (!wynik.Sukces)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = wynik.Komunikat;

                var role = await _pobierajRole.PobierzRole(User);
                var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
                return View("ZarzadzajRolami", tmp.PozostaleRole);
            }

            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = wynik.Komunikat;
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
                var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
                return View("ZarzadzajRolami", tmp.PozostaleRole);
            }
            if (NowaNazwaRoli == StaraNazwaRoli)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Nowa nazwa nie może być taka sama jak stara.";
                var role = await _pobierajRole.PobierzRole(User);
                var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
                return View("ZarzadzajRolami", tmp.PozostaleRole);
            }
            var Weryfikacja = await _pobierajUprawnieniaRoli.CzyMozeZarzadzacRolaAsync(User, RoleId);
            if (Weryfikacja.CzyMoze == false)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = Weryfikacja.Komunikat;
                var role = await _pobierajRole.PobierzRole(User);
                var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
                return View("ZarzadzajRolami", tmp.PozostaleRole);
            }

            var wynik = await _roleService.ZmienNazweRoli(RoleId, NowaNazwaRoli.Trim(), StaraNazwaRoli);

            if (!wynik.Sukces)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = wynik.Komunikat;

                var role = await _pobierajRole.PobierzRole(User);
                var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
                ViewBag.RolaId = RoleId;
                return View("ZarzadzajRolami", tmp.PozostaleRole);
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
                var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
                return View("ZarzadzajRolami", tmp.PozostaleRole);
            }
            string RolaAdminId = await _pobierajRole.PobierzIdRoliPoNazwieAsync(NazwyRoli.Admin);
            var wynik = await _roleService.UsunRole(RoleId, RolaAdminId);


            if (!wynik.Sukces)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = wynik.Komunikat;

                var role = await _pobierajRole.PobierzRole(User);
                var tmp = _roleService.PodzierRoleNaSystemoweIPozostale(User, role);
                return View("ZarzadzajRolami", tmp.PozostaleRole);
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

            DodajUzytkownikaViewModel dodajUzytkownikaViewModel = _mapowanieUzytkownika.MapujDostepneRoleVievModel(role);
            return View("DodajUzytkownika", dodajUzytkownikaViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = NazwyRoli.SuperAdminAdminDyrektor)]
        public async Task<IActionResult> DodajUzytkownika(DodajUzytkownikaViewModel dodajUzytkownikaViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Przynajmiej jedno pole jest nie uzupełnione lub zawiera wadliwe informacjie";
                var role = await _pobierajRole.PobierzRole(User);
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(dodajUzytkownikaViewModel, role);
                return View("DodajUzytkownika", dodajUzytkownikaViewModel);
            }

            var wynik = await _obslugaUzytkownika.DodajUzytkownikaAsync(dodajUzytkownikaViewModel, User);

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
                _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(dodajUzytkownikaViewModel, role);
                return View("DodajUzytkownika", dodajUzytkownikaViewModel);
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
            var mozeZarzadzac = await _pobierajUprawnieniaRoli.CzyMozeZarzadzacUzytkownikiemAsync(User, idUzytkownika);

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
            var ListaIdRolUzytkownika = await _pobierajRole.PobierzIdRolUzytkownikaAsync(idUzytkownika);
            var dostempneRole = _mapowanieRoli.MapujRoleNaSelectList(await _pobierajRole.PobierzRole(User));
            var viewModelUzytkownika = _mapowanieUzytkownika.MapujNaUzytkownikaViewModel(informacjeUzytkownik, uzytkownik, ListaIdRolUzytkownika.ToList(), dostempneRole);
            return View("EdytujUzytkownika", viewModelUzytkownika);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = NazwyRoli.SuperAdminAdminDyrektor)]
        public async Task<IActionResult> EdytujDaneUzytkownika(EdytujUzytkownikaViewModel edytujUzytkownikaViewModel)
        {

            if (edytujUzytkownikaViewModel == null)
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Błąd podczas próby edycji danych użytkownika.";
                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }
            var mozeZarzadzac = await _pobierajUprawnieniaRoli.CzyMozeZarzadzacUzytkownikiemAsync(User, edytujUzytkownikaViewModel.IdUzytkownika);
            if (!mozeZarzadzac)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = "Przynajmiej jedno pole jest nie uzupełnione lub zawiera wadliwe informacjie";
                edytujUzytkownikaViewModel.DostepneRole = _mapowanieRoli.MapujRoleNaSelectList(await _pobierajRole.PobierzRole(User));
                edytujUzytkownikaViewModel.WybraneRole = await _pobierajRole.PobierzIdRolUzytkownikaAsync(edytujUzytkownikaViewModel.IdUzytkownika);
                return View("EdytujUzytkownika", edytujUzytkownikaViewModel);
            }
            _mapowanieUzytkownika.MapujDostepneRoleDoIStniejacegoVievModel(edytujUzytkownikaViewModel, await _pobierajRole.PobierzRole(User));
            var wynik = await _obslugaUzytkownika.EdytujUzytkownikaAsync(edytujUzytkownikaViewModel, User);

            TempData["TypWiadomosci"] = wynik.CzyUdane ? "success" : "danger";
            TempData["Wiadomosc"] = wynik.Komunikat;

            if (wynik.CzyUdane)
            {
                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }

            return View("EdytujUzytkownika", edytujUzytkownikaViewModel);
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

            var mozeZarzadzac = await _pobierajUprawnieniaRoli.CzyMozeZarzadzacUzytkownikiemAsync(User, IdUzytkownika);

            if (!mozeZarzadzac)
            {
                return Forbid();
            }
            if (await _pobierajUzytkownika.CzyUzytkownikJestOpiekunemAsync(IdUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie można usunąć użytkownika, który jest opiekunem ucznia.";
                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }
            else if (await _pobierajUzytkownika.CzyUzytkownikProwadziPrzedmiotAsync(IdUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie można usunąć użytkownika, który prowadzi przedmiot.";
                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }
            else if (await _pobierajUzytkownika.CzyUzytkownikJestWychowawcaAsync(IdUzytkownika))
            {
                TempData["TypWiadomosci"] = "info";
                TempData["Wiadomosc"] = "Nie można usunąć użytkownika, który jest wychowawcą klasy.";
                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }
            var wynik = await _uzytkownikService.UsunUzytkownika(IdUzytkownika);


            if (!wynik.Stan)
            {
                TempData["TypWiadomosci"] = "danger";
                TempData["Wiadomosc"] = wynik.komunikat;

                return RedirectToAction(nameof(ZarzadzajUzytkownikem));
            }



            TempData["TypWiadomosci"] = "success";
            TempData["Wiadomosc"] = "Użytkownik został usunięty.";


            return RedirectToAction(nameof(ZarzadzajUzytkownikem));
        }
    }
}
