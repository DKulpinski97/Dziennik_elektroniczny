# Code review — Dziennik elektroniczny

Pełny skan projektu (72 pliki źródłowe, świeża kopia z Twojego komputera — nie tylko dzisiejsze zmiany), wg ustalonego planu. Poniżej wszystko, co znalazłem, od najważniejszego.

---

## 🔴 Krytyczne

### 1. Stare migracje nie zostały usunięte — historia migracji jest teraz wewnętrznie sprzeczna

W folderze `Migrations` nadal leżą wszystkie pięć starych plików (`Pierwsza migracja`, `Poprawa bazy kasowanie...`, `PoprawionoTabeleUzytkownicy`, `DodanokluczGlowny`, `NaprawaAutoIncrementIdOsoby`) **obok** nowej `20260912163618_PoczatkowaStrukturaBazy.cs`, która sama od zera tworzy wszystkie 9 tabel (`CreateTable` x9).

Efekt: na czystej, pustej bazie `dotnet ef database update` wykona najpierw starą `Pierwsza migracja` (tworzy `InformacjeUzytkownik` itd.), a potem nowa `PoczatkowaStrukturaBazy` spróbuje utworzyć te same tabele jeszcze raz → błąd „table already exists". Innymi słowy: **projekt w obecnym stanie nie da się postawić od zera na nowym środowisku** (nowy komputer, kolega z zespołu, CI/CD). U Ciebie działa tylko dlatego, że baza była ręcznie zarządzana w trakcie tej sesji (drop przez phpMyAdmin, częściowe `dotnet ef database update`).

**Do zrobienia:** usuń fizycznie 5 starych plików migracji z folderu `Migrations` (zostaw tylko `20260912163618_PoczatkowaStrukturaBazy.cs` + `.Designer.cs` + `AppDbContextModelSnapshot.cs`), i jeśli lokalna baza ma jeszcze stare wpisy w `__EFMigrationsHistory`, wyczyść tabelę i wstaw tylko wpis dla `PoczatkowaStrukturaBazy` (albo po prostu drop+recreate bazy od zera, żeby mieć pewność).

---

## 🟠 Ważne

### 2. `AccessDeniedPath` wskazuje na nieistniejącą akcję

`Program.cs`, linia 93:
```csharp
options.AccessDeniedPath = "/Logowanie/BrakDostepu";
```
W `LogowanieController` nie ma żadnej akcji `BrakDostepu` — kontroler ma tylko `Login()` i `Wyloguj()`. Efekt: gdy zalogowany użytkownik bez odpowiedniej roli trafi na akcję z `[Authorize(Roles=...)]`, zamiast strony „brak dostępu" dostanie **404**, co jest mylące i niespójne z resztą UX-u.

**Do zrobienia:** dodaj akcję `BrakDostepu` w `LogowanieController` (prosty widok z komunikatem) albo zmień ścieżkę na istniejącą.

### 3. Wyciek szczegółów wyjątku do użytkownika końcowego

`ObslugaUzytkownika.cs`, linia 136:
```csharp
return ($"Błąd: {ex.Message} Inner: {ex.InnerException?.Message}", model, false);
```
Ten komunikat trafia bezpośrednio do widoku (formularz dodawania/edycji użytkownika). Surowe komunikaty wyjątków (mogą zawierać nazwy tabel, kolumn, czasem fragmenty connection stringa przy błędach EF/MySQL) nie powinny trafiać do użytkownika końcowego — to klasyczny information disclosure z listy OWASP, o której README explicite wspomina jako o celu projektu.

**Do zrobienia:** loguj pełny wyjątek po stronie serwera (`ILogger`), a użytkownikowi zwracaj generyczny komunikat („Wystąpił błąd podczas zapisu danych").

---

## 🟡 Średnie

### 4. Literówki i niespójności w nazwach plików/tekstach

| Miejsce | Problem |
|---|---|
| `README.md`, linia 63 | Odwołanie do pliku `Hierarhia.txt` — plik w repo nazywa się `Hierarchia.txt` (brakuje „c"). Link/odwołanie jest technicznie błędny. |
| `Views/Admin/DaneUżytkonikaKontrola.cshtml` | „Użytkonika" zamiast „Użytkownika" (brak „w"). |
| `Views/Shared/_FormulazDodajUzytkonika.cshtml` | „Formulaz" zamiast prawdopodobnie „Formularz", plus ten sam brak „w" w „Uzytkonika". |
| Nazewnictwo plików `.cshtml` w `Views/Admin/` | Niespójne użycie polskich znaków: `ZarzadzajRolami.cshtml` (bez diakrytyków) vs `ZarządzajUżytkownikem.cshtml` (z diakrytykami) — ten sam czasownik „zarządzaj" zapisany dwa razy inaczej w tym samym folderze. |
| Stare migracje (już i tak do usunięcia, patrz pkt 1) | `namespace Dzienik_szkolny.Migrations` — brakuje drugiego „n" w „Dziennik" (nowe pliki mają to poprawnie: `Dziennik_szkolny.Migrations`). |
| `Infrastructure/DaneStartowe/Serwisy/DodajDaneStartowe .cs` | Spacja w nazwie pliku przed rozszerzeniem `.cs` — czysta niedbałość, warto poprawić. |
| `DaneStartowe.cs`, linia 47 | Komentarz `// Dyrektor` nad blokiem `("ViceDyrektor", ...)` — powinno być `// ViceDyrektor`, to kopiuj-wklej z bloku wyżej. |

### 5. Nieaktualny komentarz TODO — koliduje z tym, co już jest w kodzie

`LogowanieController.cs`, linia 24:
```csharp
// TODO: Przed wdrożeniem dodać ochronę CSRF na wylogowanie.
[HttpPost]
[ValidateAntiForgeryToken]
```
`[ValidateAntiForgeryToken]` już tam jest, więc ten TODO jest nieaktualny/mylący — ktoś czytający kod może pomyśleć, że ochrony brakuje, choć jest. To osobna sprawa od „świadomego TODO przy hasłach" (Program.cs / ObslugaUzytkownika.cs), który jest poprawnie opisany i spójny z README — ten jeden komentarz przy CSRF to zwykły dług informacyjny do usunięcia.

### 6. Martwy kod

- `ObslugaUzytkownika.cs`, linia 233: `//var obecneRole = await _pobierajRole.PobierzRoleUzytkownikaPoLoginieAsync(...)` — zakomentowana, nieużywana linia, niezwiązana z TODO haseł.
- `PrzypiszRoleStartowe.cs`, linie 26-43: budowana jest lista `IdRoli` (`List<List<string>>`), ale nigdzie dalej w metodzie nie jest używana — cały ten blok jest martwy.

### 7. Potencjalny NullReferenceException w seederze

`PrzypiszRoleStartowe.cs`, linia 49:
```csharp
var user = await _userManager.FindByNameAsync(userLogin);
...
bool posiadaRole = await _userManager.IsInRoleAsync(user, PojedynczaRola);
```
Brak sprawdzenia `user == null` — jeśli login z `DaneStartowe.Loginy` nie istnieje jeszcze w bazie (np. zmieniona kolejność seedowania, literówka w danych startowych), `IsInRoleAsync` dostanie `null` i rzuci wyjątkiem zamiast czytelnego błędu. Dla porównania, `DodajUprawnieniaZarzadzaniaRoli.cs` ma poprawnie taki check (`if (rolaZarzadzajaca == null || rolaZarzadzana == null) continue;`).

---

## 🟢 Niskie / do rozważenia

### 8. Duplikacja nazw ról jako magic strings

69 wystąpień twardo zakodowanych nazw ról (`"Admin"`, `"SuperAdmin"`, `"Dyrektor"` itd.) w 7 różnych plikach, m.in. identyczny string `"SuperAdmin,Admin,Dyrektor"` powtórzony w 4 różnych atrybutach `[Authorize(Roles = ...)]` w `AdminController.cs`. Literówka w jednym z tych miejsc (np. spacja albo zła wielkość litery) nie da błędu kompilacji — po prostu cicho przepuści/zablokuje dostęp. Warto rozważyć stałe (`static class Role { public const string Admin = "Admin"; ... }`) i budowanie list ról z tych stałych.

### 9. Connection string z `user=root;password=;`

`appsettings.json` ma zapisane wprost dane dostępowe do MySQL (puste hasło roota — typowy default XAMPP, więc nie jest to wyciek realnego sekretu, ale to nawyk wart poprawienia nawet w projekcie demo — np. przez `dotnet user-secrets` albo zmienne środowiskowe, żeby to nie wchodziło do repo).

---

## ✅ Co jest zrobione dobrze (potwierdzone podczas skanu)

- `[Authorize]` i `[ValidateAntiForgeryToken]` są konsekwentnie na wszystkich akcjach POST w `AdminController` i `LogowanieController`.
- Zero wystąpień blokujących `.Result` / `.Wait()` w całym kodzie — async jest konsekwentny.
- Zero nadużyć null-forgiving operatora (`!`).
- Wszystkie klasy seedujące (`DodajRoleStartowe`, `DodajLoginyStartowe`, `DodajUprawnieniaZarzadzaniaRoli`, `PrzypiszInformacjeStartowe`) poprawnie sprawdzają istnienie rekordu przed wstawieniem — seed jest idempotentny (poza drobnym ryzykiem z pkt 7).
- TODO dot. polityki haseł i blokady konta (`Program.cs`, `ObslugaUzytkownika.cs`, częściowo `LogowanieController.cs`) jest spójne, dobrze opisane i zgodne z tym, co deklaruje `README.md` — świadomy, udokumentowany dług, nie przypadkowy bałagan.

---

## Podsumowanie priorytetów

1. Usuń stare pliki migracji (krytyczne — projekt inaczej nie da się postawić od zera).
2. Dodaj brakującą akcję `BrakDostepu` albo popraw `AccessDeniedPath`.
3. Przestań zwracać `ex.Message`/`ex.InnerException.Message` do użytkownika.
4. Reszta (literówki, martwy kod, magic stringi) — porządki jakościowe, można robić stopniowo.
