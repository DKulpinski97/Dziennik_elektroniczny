# Dziennik elektroniczny

Aplikacja webowa symulująca elektroniczny dziennik szkolny. Projekt portfolio, w którym ćwiczę i prezentuję umiejętności fullstack .NET developera. Gotowy jest panel administracyjny (użytkownicy, role, hierarchia uprawnień); moduły ucznia, klas i planu zajęć mają na razie tylko dane startowe i strukturę bazy.

## O projekcie

System zarządzania użytkownikami i rolami w środowisku szkolnym z wielopoziomową hierarchią uprawnień (SuperAdmin, Admin, Dyrektor, ViceDyrektor, Sekretarka, Nauczyciel, Rodzic, Uczeń). Projekt kładzie nacisk na bezpieczeństwo dostępu do danych i na rozdzielenie odpowiedzialności między warstwami aplikacji.

## Stack technologiczny

- **Backend:** ASP.NET Core MVC (.NET 10)
- **Baza danych:** MySQL, EF Core 9 (Pomelo.EntityFrameworkCore.MySql)
- **Uwierzytelnianie i autoryzacja:** ASP.NET Core Identity
- **Testy:** xUnit, Moq
- **Architektura:** podział na warstwy Domain / Application / Infrastructure / Controllers / Views

## Jak uruchomić projekt

**Wymagania:** .NET 10 SDK, MySQL Server, narzędzie `dotnet-ef`.

1. Sklonuj repozytorium.
2. W `Dziennik szkolny/appsettings.json` ustaw `ConnectionStrings:DefaultConnection`. Wartość domyślna wskazuje lokalny serwer (`server=localhost;port=3306;database=dziennik;user=root;password=`) i jest przeznaczona wyłącznie do środowiska deweloperskiego.
3. Utwórz schemat bazy z migracji: `dotnet ef database update --project "Dziennik szkolny"`.
4. Uruchom aplikację: `dotnet run --project "Dziennik szkolny"`. Przy pierwszym starcie na pustej bazie aplikacja zasila ją danymi startowymi; przy kolejnych startach dane startowe nie są ponownie dodawane.
5. Zaloguj się jednym z kont demonstracyjnych (niżej).

## Konta demonstracyjne

Konta tworzone przez seed: `SuperAdmin`, `Admin`, `Dyrektor`, `ViceDyrektor`, `Sekretarka`, `Nauczyciel1`, `Nauczyciel2`, `Nauczyciel3`, `Rodzic1`, `Rodzic2`, `Rodzic3`. Wszystkie mają to samo hasło demonstracyjne (stała w `DaneStartowe.cs`, spełnia reguły siły hasła); aktualne dane są w pliku `Dane logowania do systemu.txt`. To konta wyłącznie pokazowe; w prawdziwym systemie konta administracyjne powinny być imienne (zob. niżej).

Konto `Nauczyciel1` ma role Nauczyciel i Rodzic, jest wychowawcą klas A i B, prowadzi polski i matematykę oraz jest opiekunem ucznia, więc na tym koncie widać wszystkie blokady zmiany ról i usuwania.

## Funkcjonalności

- Logowanie z przekierowaniem do strony, o którą użytkownik pierwotnie prosił (`ReturnUrl`, tylko adresy lokalne).
- Blokada konta po 5 nieudanych próbach na 5 minut; komunikat podaje godzinę końca blokady w czasie polskim.
- Panel administracyjny: dodawanie, edycja i usuwanie użytkowników oraz dodawanie, zmiana nazwy i usuwanie ról. Panel użytkowników jest dostępny dla SuperAdmina, Admina, Dyrektora i ViceDyrektora, a zarządzanie rolami tylko dla Admina.
- Hierarchiczny model uprawnień: użytkownik widzi i zarządza tylko osobami, których role są mu podporządkowane w hierarchii (widoczność: co najmniej jedna rola podległa, zarządzanie: wszystkie role podległe).
- Walidacja danych użytkownika: m.in. suma kontrolna numeru PESEL, telefon, unikalność loginu, e-maila i numeru PESEL.
- Reguły haseł: co najmniej 8 znaków, cyfra, mała i wielka litera oraz znak specjalny. Reguły są zapisane w jednym miejscu (`Domain/reguly/OgraniczeniaHasla.cs`) i używane przez konfigurację Identity, walidację i widoki.
- Reguły nazw ról: po przycięciu 2–50 znaków, słowa łączone bez spacji i pisane wielką literą (np. „it specjalista” daje „ItSpecjalista”), duplikaty odrzucane. Role systemowe z danych startowych nie mogą być usunięte ani przemianowane. Nie można też usunąć roli, która uczestniczy w hierarchii poza relacją z rolą Admin, ani roli będącej jedyną rolą jakiegoś użytkownika.
- Reguła ostatniego administratora: nie można usunąć jedynego użytkownika z rolą Admin.
- Ochrona powiązań użytkownika: nie można usunąć użytkownika, który jest opiekunem ucznia, prowadzi przedmiot albo jest wychowawcą klasy. W edycji nie można odebrać roli Rodzic opiekunowi ucznia ani roli Nauczyciel nauczycielowi, który prowadzi przedmiot lub jest wychowawcą. Zmiana samych danych i dodawanie ról są dozwolone.

## Bezpieczeństwo

Celem było praktyczne zastosowanie zasad bezpiecznego dostępu do danych (broken access control, OWASP):

- kontrola dostępu na poziomie zasobu, nie tylko roli, przy podglądzie, edycji i usuwaniu użytkowników
- ochrona przed podniesieniem uprawnień: użytkownik nie przypisze roli spoza swojego zakresu
- token anty-CSRF na logowaniu i na akcjach zmieniających dane w panelu administracyjnym
- ochrona przed open redirect (`Url.IsLocalUrl` + `LocalRedirect`)
- blokada konta i wymagania siły hasła włączone w konfiguracji Identity

**Świadome kompromisy:**

- Komunikat o blokadzie konta ujawnia, że takie konto istnieje, i pozwala celowo zablokować cudze konto. To kompromis na rzecz czytelności dla użytkownika.
- Konto `SuperAdmin` jest chronione przez hierarchię: żadna rola nim nie zarządza, a rolą `Admin` zarządza tylko `SuperAdmin`.
- Akcja wylogowania nie wymaga tokenu anty-CSRF. Skutkiem takiego ataku byłoby wyłącznie wymuszone wylogowanie użytkownika, bez dostępu do konta lub danych.
- Ochrona przed bezpośrednią edycją bazy danych jest poza zakresem aplikacji (wymaga dostępu do całego systemu); właściwą odpowiedzią są logi działań administratorów.

**Zasada kont administracyjnych.** Konta administracyjne powinny być oddzielone od kont codziennych, imienne (a nie wspólne), z rozdzieleniem obowiązków (`SuperAdmin` zarządza administratorami, `Admin` rolami szkolnymi) i z niezależnością awaryjną (reguła ostatniego administratora). Wspólne konta `Admin` i `SuperAdmin` w danych startowych są wyjątkiem demonstracyjnym.

## Architektura

Podział na warstwy (Domain, Application, Infrastructure) inspirowany Clean Architecture, z rozdzieleniem logiki biznesowej od kontrolerów oraz wzorcem Unit of Work do zarządzania transakcjami. Niektóre uproszczenia zastosowano świadomie, aby projekt pozostał czytelny jako materiał demonstracyjny. Największe z nich: ASP.NET Core Identity (`LoginUzytkownika`) jest używane bezpośrednio w Domain i Application zamiast własnego modelu użytkownika mapowanego w Infrastructure, więc zależności nie są w pełni odwrócone; w jednym projekcie, z Identity jako podstawą logowania, uznałem to za rozsądny kompromis.

## Dane startowe

Przy starcie aplikacja sprawdza, czy w bazie są użytkownicy. Jeśli ich nie ma, w jednej transakcji zasila bazę rolami systemowymi, hierarchią uprawnień, kontami demonstracyjnymi, klasami, uczniami, przedmiotami i planem zajęć; błąd w trakcie cofa całość, więc kolejny start spróbuje ponownie. Jeśli użytkownicy już istnieją, dane startowe nie są dodawane, więc zmiany wprowadzone w panelu (usunięte konta, zmienione role) nie są cofane przy restarcie. Skutek: nowe dane startowe dodane w kodzie trafiają tylko do świeżej bazy (`dotnet ef database drop`, potem `update`).

## Hierarchia ról

Kto kim zarządza (relacje przechowywane w bazie jako para rola zarządzająca → rola zarządzana):

| Rola | Może zarządzać rolami |
|---|---|
| SuperAdmin | Admin |
| Admin | Dyrektor, ViceDyrektor, Sekretarka, Nauczyciel, Rodzic |
| Dyrektor | Sekretarka, Nauczyciel, Rodzic |
| ViceDyrektor | Sekretarka, Nauczyciel, Rodzic |
| Sekretarka | Uczeń, Rodzic |
| Nauczyciel, Rodzic, Uczeń | nikim |

Szczegółowy opis jest w pliku `Hierarchia.txt` w katalogu głównym repozytorium.

## Testy

Testy jednostkowe (xUnit, Moq) obejmują walidację danych użytkownika (w tym siłę hasła i PESEL) oraz serwis zarządzania rolami. Dwa testy usuwania roli są pominięte, bo wymagają bazy w pamięci.

## Znane ograniczenia

- Reguły ochrony powiązań użytkownika (opiekun, przedmiot, wychowawca) oraz reguła ostatniego administratora są zapisane w kontrolerze i nie mają testów jednostkowych.
- Brak edycji własnego hasła i podglądu własnych danych (planowane).
- Rola Sekretarka nie ma jeszcze dostępu do panelu użytkowników; jej panel (zarządzanie Uczniem i Rodzicem) jest planowany, na stronie głównej widzi na razie pusty panel.
- Logowanie błędów (`ILogger`) jest tylko w obsłudze użytkownika; brak logów działań administratorów i odmów.
- Brak testów dla reguł widoczności i zarządzania (Any/All) oraz dla usuwania ról powiązanych z hierarchią.
- Panel nie pozwala tworzyć relacji hierarchii między rolami; powstają one tylko z danych startowych.
- Dane startowe, w tym konta demonstracyjne ze wspólnym hasłem, są dodawane do każdej pustej bazy, w każdym środowisku (także produkcyjnym); aktualizacja bazy po zmianie danych startowych wymaga jej odtworzenia.

## Planowane dalsze prace

**Techniczne:**
- testy Any/All i usuwania ról z bazą w pamięci
- logi działań administratorów i odmów dostępu
- ujednolicenie sprawdzania powiązań użytkownika (opiekun, przedmiot, wychowawca) w jednym miejscu dla edycji i usuwania oraz przeniesienie tych reguł do serwisu z testami jednostkowymi
- Web API ról (osobna gałąź)

**Funkcjonalne:**
- edycja własnego hasła i podgląd własnych danych
- panel Sekretarki: zarządzanie Uczniem i Rodzicem
- dodanie ucznia, klas i łączenie nauczycieli z klasami
- plan zajęć

## Zrzuty ekranu

Wszystkie zrzuty pochodzą z danych demonstracyjnych.

### Logowanie i strony główne według ról

![Ekran logowania](docs/zrzuty/ekran-logowania.png)
*Ekran logowania.*

![Strona główna Admina](docs/zrzuty/index-admin.png)
*Strona główna Admina: panel administratora z zarządzaniem rolami i użytkownikami.*

![Strona główna Dyrektora](docs/zrzuty/index-dyrektor-vice-dyrektor.png)
*Strona główna Dyrektora (identycznie ViceDyrektora): zarządzanie użytkownikami bez zarządzania rolami.*

![Strona główna użytkownika z rolami Nauczyciel i Rodzic](docs/zrzuty/index-nauczyciel-rodzic.png)
*Użytkownik z dwiema rolami widzi panele obu.*

![Strona główna Nauczyciela](docs/zrzuty/index-nauczyciel.png)
*Strona główna Nauczyciela.*

![Strona główna Rodzica](docs/zrzuty/index-rodzic.png)
*Strona główna Rodzica.*

### Zarządzanie rolami (Admin)

![Menu ról](docs/zrzuty/menu-roli.png)
*Lista ról systemowych i pozostałych.*

![Zarządzanie rolami](docs/zrzuty/menu-zarzadzania-rolami.png)
*Dodawanie, zmiana nazwy i usuwanie ról spoza systemowych, z podpowiedzią reguł nazw.*

### Zarządzanie użytkownikami

![Lista użytkowników widziana przez Admina](docs/zrzuty/menu-zarzadzania-uzytkownikem-admin.png)
*Widok Admina: wśród pracowników widoczni są także Dyrektor i ViceDyrektor.*

![Lista użytkowników widziana przez Dyrektora](docs/zrzuty/menu-zarzadzania-uzytkownikem-dyrektor.png)
*Widok Dyrektora: tylko role mu podległe, bez Dyrektora, ViceDyrektora i Admina.*

![Dodawanie użytkownika](docs/zrzuty/dodawanie-uzytkownika.png)
*Dodawanie użytkownika; lista ról zawiera tylko role, którymi zalogowany użytkownik może zarządzać.*

![Edycja użytkownika](docs/zrzuty/edycja-uzytkownika.png)
*Edycja użytkownika z opcjonalną zmianą hasła i zaznaczonymi rolami.*

![Blokada zmiany roli](docs/zrzuty/przykladowy-blad-edycji-uzytkownika.png)
*Ochrona powiązań: nie można odebrać roli Rodzic opiekunowi ucznia. Formularz zostaje na ekranie, a zaznaczenie ról jest zachowane.*
