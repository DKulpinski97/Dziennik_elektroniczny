# Temat do nauki: magiczne stringi i wzorzec "jedno miejsce definicji"

Kontekst: znaleziony i omówiony podczas dalszej pracy nad code review projektu Dziennik_elektroniczny (opcjonalny punkt z listy do poprawy). Temat nowy, nieprzerabiany wcześniej w trybie nauki.

## Problem: magic string (magiczny napis)

Magic string to literał tekstowy używany bezpośrednio w logice programu, zamiast nazwanej, jednej wspólnej definicji. W projekcie Dziennik_elektroniczny nazwa roli `"Admin"` (i inne nazwy ról) występowała jako gołe stringi w wielu, niezależnych od siebie miejscach:

- `DaneStartowe.cs` — w listach `Role`, `RolePracownika`, `PrzypisanieRoli`, `Loginy`, `Informacje`
- `AdminController.cs` — czterokrotnie w `[Authorize(Roles = "Admin")]`
- kilka widoków `.cshtml` w `Views/Admin`

Sam napis `"Admin"` nie jest błędem — problem to **duplikacja tego samego faktu w wielu miejscach naraz**, bez jednego wspólnego źródła.

## Dlaczego to realny problem, nie tylko kosmetyka

Kompilator C# nie wie, że `"Admin"` w pliku A i `"Admin"` w pliku B to "ten sam byt" — to dla niego po prostu dwa niezależne stringi, które przypadkiem mają taką samą treść. Konsekwencje:

- literówka w jednym z wielu miejsc (`"Admn"`) nie jest błędem kompilacji — program się zbuduje, a błąd ujawni się dopiero w runtime, cicho (np. `[Authorize(Roles = "Admn")]` nie przepuści nikogo, kto naprawdę ma rolę `"Admin"`)
- zmiana nazwy roli wymaga ręcznego znalezienia i poprawienia każdego wystąpienia z osobna — łatwo coś pominąć
- brak jednego miejsca, które jednoznacznie odpowiada na pytanie "jakie role istnieją w systemie i jak dokładnie się nazywają"

## Rozwiązanie: klasa ze stałymi (`const string`)

```csharp
public static class NazwyRoli
{
    public const string Admin = "Admin";
    public const string SuperAdmin = "SuperAdmin";
    public const string Dyrektor = "Dyrektor";
    public const string ViceDyrektor = "ViceDyrektor";
    public const string Nauczyciel = "Nauczyciel";
    public const string Sekretarka = "Sekretarka";
    public const string Rodzic = "Rodzic";
    public const string Uczen = "Uczen";
}
```

Wszędzie, gdzie wcześniej był literał `"Admin"`, używamy `NazwyRoli.Admin`. Kluczowa właściwość: `const string` jest **stałą znaną w czasie kompilacji**, dzięki czemu można jej użyć nawet wewnątrz atrybutu — `[Authorize(Roles = NazwyRoli.Admin)]` kompiluje się bez problemu (atrybuty w C# akceptują tylko wyrażenia stałe, nie każdą zmienną).

Efekt: literówka w nazwie stałej (`NazwyRoli.Admni`) jest teraz błędem **kompilacji**, a nie cichym błędem w runtime — bo `NazwyRoli.Admni` po prostu nie istnieje jako identyfikator. To przesuwa wykrycie błędu z "ktoś kiedyś zgłosi, że nie działa" na "IDE podkreśli to na czerwono, zanim jeszcze uruchomisz program".

## Alternatywa (poznana przy okazji, ale niewybrana w tym projekcie): enum + `nameof()`

Rozważaliśmy też podejście przez `enum`:

```csharp
public enum Role { Admin, SuperAdmin, Dyrektor }
```

Problem: ASP.NET Core Identity (`RoleManager`, `[Authorize(Roles=...)]`) operuje wyłącznie na `string` — nazwa roli w bazie danych (`AspNetRoles.Name`) to zwykły tekst, nie typ `enum`. Żeby użyć enuma, i tak trzeba go zamienić na string w każdym miejscu użycia: `nameof(Role.Admin)` zwraca `"Admin"` i — podobnie jak `const string` — jest stałą kompilacji, więc też działa w atrybucie. Dodatkowa zaleta: zmiana nazwy przez "Rename Symbol" w IDE automatycznie aktualizuje wszystkie `nameof(...)`.

Wybrane podejście dla tego projektu: prostsza klasa ze stałymi (`NazwyRoli`), bo nie wymaga żadnych dodatkowych sztuczek (`nameof`) i jest wystarczająca — enum dodaje realną wartość głównie wtedy, gdy potrzeba np. `Enum.GetValues()` do iterowania po wszystkich wariantach, czego tu nie potrzebujemy.

## Ważne rozróżnienie: `NazwyRoli` a `DaneStartowe`

`NazwyRoli` **nie zastępuje** `DaneStartowe.cs` i nie "decyduje", jakie role istnieją w systemie. To nie jest obiekt, który się gdziekolwiek przekazuje (nie ma instancji, nie leci przez konstruktor ani parametr metody) — to statyczny "słownik nazw", do którego kod odwołuje się bezpośrednio w miejscu użycia.

Rzeczywiste źródło prawdy o tym, jakie role istnieją, to nadal:
1. `DaneStartowe.cs` — konfiguracja seeda: które role są rolami pracowniczymi, pełna lista ról, hierarchia zarządzania, przypisania ról do użytkowników startowych,
2. baza danych — tabela `AspNetRoles`, na której faktycznie operuje `[Authorize]` i `IsInRoleAsync`.

`NazwyRoli` tylko eliminuje duplikację **tekstu** w kodzie C# — `DaneStartowe` i tak pozostaje odpowiedzialna za strukturę i logikę seeda, tylko czerpie teksty ról z jednego wspólnego źródła zamiast wpisywać je ręcznie osobno.

## Do zapamiętania jako ogólna zasada (nie tylko role)

Ten sam wzorzec (jedna nazwana stała/wspólne źródło zamiast powtarzanego literału) stosuje się szerzej — do dowolnego tekstu lub liczby, która ma ustalone, powtarzające się znaczenie w wielu miejscach kodu (nazwy claimów, klucze konfiguracji, kody błędów, limity biznesowe). Ogólna zasada DRY (Don't Repeat Yourself) w praktyce: jeśli fakt pojawia się w kodzie więcej niż raz jako literał, prawdopodobnie powinien być jedną nazwaną stałą.
