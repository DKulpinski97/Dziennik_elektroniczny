# Case study: zniknięcie AUTO_INCREMENT po migracji EF Core (RenameColumn)

**Projekt:** Dziennik elektroniczny (ASP.NET Core, EF Core 9, Pomelo.EntityFrameworkCore.MySql, MySQL/MariaDB)
**Obszar:** migracje bazy danych, Entity Framework Core, diagnostyka na styku model↔baza

---

## 1. Objaw

Kolumna `IdOsoby` (klucz główny tabeli `InformacjeUzytkownik`) przestała samodzielnie generować wartości przy nowych rekordach. W kodzie aplikacji istniały już dwa niezależne obejścia tego problemu:

- w seederze danych startowych (`PrzypiszInformacjeStartowe.cs`) — ręczne liczenie `MaxAsync()` i inkrementacja ID,
- w realnej ścieżce dodawania użytkownika (`ZarzadzajUzytkownikemService.DodajInformacjeUzytkownikaAsync`) — dokładnie ten sam wzorzec.

Obecność takiego kodu (plus komentarz w stylu „Tymczasowo ręczne nadawanie ID, ponieważ AUTO_INCREMENT nie działa poprawnie") to typowy sygnał: ktoś już wcześniej zauważył symptom, ale załatał go w warstwie aplikacji zamiast naprawić w warstwie bazy danych.

## 2. Metoda diagnozy

Zamiast zgadywać, prześledzono **całą historię migracji** pliku po pliku, linia po linii, szukając każdego miejsca, które w ogóle dotyka kolumny `IdOsoby`/`IdRodzica`:

| Migracja | Co robi z kolumną |
|---|---|
| `Pierwsza migracja` | Tworzy `IdRodzica bigint` z `.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)` — AUTO_INCREMENT obecny od początku. |
| `Poprawa bazy kasowanie rekurencyjnych tabel` | Brak jakiegokolwiek dotknięcia tej kolumny/tabeli. |
| **`PoprawionoTabeleUzytkownicy`** | **`RenameColumn(IdRodzica → IdOsoby)` — i nic więcej.** |
| `DodanokluczGlowny` | Dotyczy tylko `IdUzytkownika`, nie `IdOsoby`. |
| `ŁączenieRoli` | Tworzy zupełnie inną, niepowiązaną tabelę. |

**Wniosek strukturalny:** w całej historii istnieje dokładnie jedno miejsce, w którym atrybut mógł zostać utracony — `RenameColumn`.

## 3. Przyczyna źródłowa

`RenameColumnOperation` w EF Core jest z założenia „chudą" operacją — niesie tylko nazwę starą i nową, **żadnych informacji o typie, nullowalności ani strategii generowania wartości** (czyli m.in. o AUTO_INCREMENT). To, jaki SQL z tego powstanie, zależy od silnika bazy wykrytego w danym momencie przez `ServerVersion.AutoDetect(...)`:

- na MySQL 8.0.13+ / MariaDB 10.5.2+ dostawca (Pomelo) może wygenerować natywne `ALTER TABLE ... RENAME COLUMN old TO new` — to zachowuje AUTO_INCREMENT,
- na starszych silnikach dostawca może spaść do `CHANGE COLUMN old new <pełna redefinicja typu>` — a ta redefinicja, bez jawnej adnotacji identity, **gubi AUTO_INCREMENT**.

Kluczowa lekcja: **EF Core nigdy nie weryfikuje zgodności modelu z rzeczywistą bazą danych.** `dotnet ef migrations add` porównuje aktualny model C# wyłącznie z ostatnim zapisanym `ModelSnapshot`, nie z żywą bazą. Skoro model (`[DatabaseGenerated(DatabaseGeneratedOption.Identity)]` na encji) nigdy się nie zmienił, EF nigdy „nie zauważy", że baza faktycznie odjechała od tego, co powinna mieć — więc żadna kolejna migracja nie naprawi tego sama z siebie.

## 4. Naprawa

Rozważono dwie ścieżki:

1. **Migracja naprawcza (patch)** — dopisanie nowej migracji z jawnym `AlterColumn` + `.Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)`. Bezpieczne na istniejącej bazie z danymi, nie wymaga resetu.
2. **Reset migracji (wybrana opcja)** — skasowanie całej historii migracji i bazy, wygenerowanie jednej świeżej migracji (`dotnet ef migrations add`) z aktualnego modelu. Czystsze rozwiązanie dla projektu bez produkcyjnych danych — nowa migracja od razu poprawnie tworzy tabelę z AUTO_INCREMENT, bez żadnego `RenameColumn` w historii.

Potwierdzenie w bazie:
```sql
SHOW COLUMNS FROM InformacjeUzytkownik WHERE Field = 'IdOsoby';
-- Extra: auto_increment  ✅
```

Dodatkowo usunięto oba obejścia w kodzie aplikacji (ręczne liczenie `MaxAsync()` + ręczne przypisanie `IdOsoby`) — zarówno w seederze, jak i w `ZarzadzajUzytkownikemService`, przywracając poleganie na bazie danych jako jedynym źródle generowania ID.

## 5. Lekcje na przyszłość

- **`RenameColumn` ≠ bezpieczna operacja przy kolumnach Identity/AUTO_INCREMENT.** Przy zmianie nazwy takiej kolumny warto od razu dopisać jawny `AlterColumn` z odtworzeniem strategii generowania wartości, zamiast ufać samemu rename.
- **Objaw w kodzie aplikacji (ręczne liczenie ID, `MaxAsync()`, komentarze „tymczasowo") to czerwona flaga** — sygnał, że gdzieś w warstwie bazy danych jest niezgodność ze stanem oczekiwanym przez model. Warto szukać przyczyny w migracjach, zamiast zostawiać obejście na stałe.
- **EF Core migracje = diff modelu vs. snapshot, nigdy vs. żywej bazy.** Rozjazd między tym, co baza faktycznie zawiera, a tym, co EF „myśli", że zawiera, nie zostanie sam wykryty ani naprawiony przez `dotnet ef migrations add`.
- Zawsze warto zweryfikować rzeczywisty stan bazy (`SHOW CREATE TABLE` / `SHOW COLUMNS ... `) zamiast polegać wyłącznie na tym, co pokazuje `ModelSnapshot.cs` — to dwa niezależne źródła prawdy, które mogą się rozjechać.
- Na wczesnym etapie projektu (bez środowiska produkcyjnego) reset historii migracji do jednej, czystej migracji początkowej bywa prostszym i solidniejszym rozwiązaniem niż nawarstwianie migracji naprawczych.
