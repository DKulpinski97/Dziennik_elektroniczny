using Dzienik_szkolny.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dzienik_szkolny.Controllers
{
    public class LogowanieController : Controller
    {
      private readonly UserManager<LoginUzytkownika> _userManager;
      private readonly SignInManager<LoginUzytkownika> _signInManager;
        public LogowanieController(UserManager<LoginUzytkownika> userManager, SignInManager<LoginUzytkownika> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
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
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(LoginUzytkownika loginUzytkownika)
        {
            var result = await _userManager.CreateAsync(
        loginUzytkownika,
        loginUzytkownika.PasswordHash
    );

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(loginUzytkownika);
            }
           

        }
        [HttpGet]
        public async Task<IActionResult> Wyloguj()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}
