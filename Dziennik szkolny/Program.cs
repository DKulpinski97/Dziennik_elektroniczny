using Dziennik_szkolny.Data;
using Dziennik_szkolny.Models;
using Dziennik_szkolny.Services;
using Dziennik_szkolny.Services.Interfaces;
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
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUzytkownikService, UzytkownikService>();
builder.Services.AddScoped<IDodajDaneStartowe, DodajDaneStartowe>();


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
    var seeder = scope.ServiceProvider.GetRequiredService<IDodajDaneStartowe>();
    await seeder.DodajDaneStartoweAsync();
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