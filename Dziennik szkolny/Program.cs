using Dziennik_szkolny.Application.Interfejsy.DaneStartowe;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Serwisy.Uzytkownik;
using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Infrastructure.DaneStartowe.Serwisy;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.Infrastructure.Serwisy.Role;
using Dziennik_szkolny.Infrastructure.Serwisy.Uzytkownik;
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
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


// Własne serwisy
builder.Services.AddScoped<IZarzadzajRolami, ZarzadzajRolamiService>();
builder.Services.AddScoped<IZarzadzajUzytkownikem, ZarzadzajUzytkownikemService>();
builder.Services.AddScoped<IDodajDaneStartowe, DodajDaneStartowe>();
builder.Services.AddScoped<IWeryfikacjaDanychLogowania, WeryfikacjaDanychLogowaniaService>();
builder.Services.AddScoped<IPobierajRole, PobierajRoleService>();
builder.Services.AddScoped<IPobierajUzytkownika, PobierajUzytkownikaService>();


// Konfiguracja ciasteczka Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Logowanie/Login";
    options.AccessDeniedPath = "/Logowanie/BrakDostepu";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider
            .GetRequiredService<IDodajDaneStartowe>();

        await seeder.DodajDaneStartoweAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"BŁĄD DANYCH STARTOWYCH: {ex.Message}");
        throw;
    }
}


// Obsługa błędów
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// Identity
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Logowanie}/{action=Login}/{id?}"
);


app.Run();