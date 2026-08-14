using Dziennik_szkolny.Application.Modele;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dziennik_szkolny.Controllers
{
    public class LogowanieController : Controller
    {
        private readonly SignInManager<LoginUzytkownika> _signInManager;
        public LogowanieController(SignInManager<LoginUzytkownika> signInManager)
        {
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login Login)
        {
            var result = await _signInManager.PasswordSignInAsync(
       Login.LoginUzytkownika,
       Login.HasloUzytkownika,
       false,
       false
   );

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Nieprawidłowy login lub hasło");

            return View(Login);

        }
 
        [HttpPost]
        public async Task<IActionResult> Wyloguj()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}
