using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }


    [HttpGet]
    public async Task<IActionResult> ZarzadzajRolami()
    {
        var role = await _roleManager.Roles.ToListAsync();
        return View(role);
    }


    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> DodajRole(string nazwaRoli)
    {
        if (string.IsNullOrWhiteSpace(nazwaRoli))
        {
            ViewBag.Komunikat = "Nazwa roli nie może być pusta.";
            ViewBag.NazwaRoli = nazwaRoli;
            return View("ZarzadzajRolami", await _roleManager.Roles.ToListAsync());
        }

        nazwaRoli = nazwaRoli.Trim();
        var role = await _roleManager.Roles.ToListAsync();
        if (await _roleManager.RoleExistsAsync(nazwaRoli))
        {
            ViewBag.Komunikat = "Taka rola już istnieje.";
            ViewBag.NazwaRoli = nazwaRoli;
            return View("ZarzadzajRolami", await _roleManager.Roles.ToListAsync());
        }

        await _roleManager.CreateAsync(new IdentityRole(nazwaRoli));

        return RedirectToAction("ZarzadzajRolami");
    }
}