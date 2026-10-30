using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Modele;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dziennik_szkolny.Controllers
{
    public class LogowanieController : Controller
    {
        private readonly SignInManager<LoginUzytkownika> _signInManager;
        private readonly IWeryfikacjaDanychLogowania _weryfikacjaDanychLogowania;
        public LogowanieController(SignInManager<LoginUzytkownika> signInManager, IWeryfikacjaDanychLogowania weryfikacjaDanychLogowania)
        {
            _signInManager = signInManager;
            _weryfikacjaDanychLogowania = weryfikacjaDanychLogowania;
        }
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login Login, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(Login);
            }

            var result = await _signInManager.PasswordSignInAsync(
                Login.LoginUzytkownika,
                Login.HasloUzytkownika,
                false,
                true
            );

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                var doKiedyBlokada = await _weryfikacjaDanychLogowania.PobranieDoKiedyBlokadaAsync(Login.LoginUzytkownika);
                ModelState.AddModelError("", $"Blokada konta do {doKiedyBlokada.ToString("yyyy-MM-dd HH:mm:ss")}");
            }
            else
            {
                ModelState.AddModelError("", "Nieprawidłowy login lub hasło");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(Login);
        }
        [HttpPost]
        public async Task<IActionResult> Wyloguj()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult BrakDostepu()
        {
            TempData["TypWiadomosci"] = "danger";
            TempData["Wiadomosc"] = "Nie masz uprawnień do wykonania tej akcji.";

            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }
    }
}
