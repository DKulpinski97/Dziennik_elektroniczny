using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Mapery;
using Dziennik_szkolny.Models;
using Dziennik_szkolny.ViewModel.Uzytkownik;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Dziennik_szkolny.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly MapowanieUzytkownika _mapowanieUzytkownika = new MapowanieUzytkownika();
        private readonly IPobierajUzytkownika _pobierajUzytkownika;
        public HomeController( IPobierajUzytkownika pobierajUzytkownika)
        {
            _pobierajUzytkownika = pobierajUzytkownika;
        }
        public async Task<IActionResult> Index()
        {
            PrzywitanieUzytkownikaViewModel przywitanieUzytkownikaViewModel = new PrzywitanieUzytkownikaViewModel();
            var (imie, nazwisko) = await _pobierajUzytkownika.PobierzImieINazwiskoPoIdUzytkownikaAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(imie == null || nazwisko == null)
            {
                przywitanieUzytkownikaViewModel.Imie = "";
                przywitanieUzytkownikaViewModel.Nazwisko = "";
                return View(przywitanieUzytkownikaViewModel);
            }
            przywitanieUzytkownikaViewModel = _mapowanieUzytkownika.MapowanieDanePodstawowe(imie, nazwisko);
            return View(przywitanieUzytkownikaViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
