using Dziennik_szkolny.Application.Interfejsy;
using Dziennik_szkolny.Application.Interfejsy.DaneStartowe;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Application.Mapery;
using Dziennik_szkolny.Application.Serwisy.Uzytkownik;
using Dziennik_szkolny.Application.Walidacja.Uzytkownik;
using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Infrastructure.DaneStartowe;
using Dziennik_szkolny.Infrastructure.DaneStartowe.Serwisy;
using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Dziennik_szkolny.Infrastructure.Serwisy;
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
// TODO: Przed wdrożeniem włączyć wymagania silnego hasła.
builder.Services.AddIdentity<LoginUzytkownika, IdentityRole>(options =>
{
    /*
    options.Password.RequiredLength = 8;          // Minimalna długość hasła
    options.Password.RequireDigit = true;         // Wymagana cyfra
    options.Password.RequireLowercase = true;     // Wymagana mała litera
    options.Password.RequireUppercase = true;     // Wymagana wielka litera
    options.Password.RequireNonAlphanumeric = true; // Wymagany znak specjalny
     * */
    options.Password.RequiredLength = 1;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();


// Własne serwisy
//Generowanie danych startowych
builder.Services.AddSingleton<DaneStartowe>();
builder.Services.AddScoped<IDodajDaneStartowe, DodajDaneStartowe>();
builder.Services.AddScoped<DodajRoleStartowe>();
builder.Services.AddScoped<DodajLoginyStartowe>();
builder.Services.AddScoped<PrzypiszRoleStartowe>();
builder.Services.AddScoped<PrzypiszInformacjeStartowe>();
builder.Services.AddScoped<DodajUprawnieniaZarzadzaniaRoli>();

//zażądzanie użytkownikami
builder.Services.AddScoped<IZarzadzajRolami, ZarzadzajRolamiService>();
builder.Services.AddScoped<IZarzadzajUzytkownikem, ZarzadzajUzytkownikemService>();
builder.Services.AddScoped<IObslugaUzytkownika, ObslugaUzytkownika>();

//sewisy weryfikujące dane
builder.Services.AddScoped<IWeryfikacjaDanychLogowania, WeryfikacjaDanychLogowaniaService>();
builder.Services.AddScoped<WalidacjaDanychUzytkownika>();

//Serwisy infrastruktury
builder.Services.AddScoped<IPobierajRole, PobierajRoleService>();
builder.Services.AddScoped<IPobierajUzytkownika, PobierajUzytkownikaService>();
builder.Services.AddScoped<IJednostkaPracy, JednostkaPracy>();
builder.Services.AddScoped<IPobierajUprawnieniaRoli, PobierajUprawnieniaRoli>();



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