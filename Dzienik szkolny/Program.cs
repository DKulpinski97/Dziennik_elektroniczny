using Dzienik_szkolny.Data;
using Dzienik_szkolny.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();


// Baza danych MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    ));


// Identity
builder.Services.AddIdentity<LoginUzytkownika, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


// Konfiguracja przekierowania niezalogowanego użytkownika
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Logowanie/Login";
    options.AccessDeniedPath = "/Logowanie/BrakDostepu";

    // Automatyczne wylogowanie po bezczynności
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    // Odświeżaj czas wygaśnięcia przy aktywności użytkownika
    options.SlidingExpiration = true;
});


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// Kolejność jest poprawna
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Logowanie}/{action=Login}/{id?}");


app.Run();