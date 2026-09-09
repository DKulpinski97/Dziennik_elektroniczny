# Dziennik elektroniczny

🚧 **Projekt w fazie wczesnego rozwoju** — architektura, zabezpieczenia i pokrycie testami są aktywnie rozwijane.

Aplikacja webowa symulująca elektroniczny dziennik szkolny — projekt portfolio stworzony w celu przećwiczenia i zaprezentowania umiejętności fullstack .NET developera.

## O projekcie

Dziennik elektroniczny to system zarządzania użytkownikami i rolami w środowisku szkolnym, z wielopoziomową hierarchią uprawnień (Super Admin → Admin → Dyrektor → ... pozostałe role szkolne). Projekt kładzie nacisk na bezpieczeństwo dostępu do danych oraz poprawną separację odpowiedzialności między warstwami aplikacji.

## Stack technologiczny

- **Backend:** ASP.NET Core MVC (.NET 10)
- **Baza danych:** MySQL, EF Core 9 (Pomelo.EntityFrameworkCore.MySql)
- **Autoryzacja i uwierzytelnianie:** ASP.NET Core Identity
- **Architektura:** podział na warstwy Domain / Application / Infrastructure / Controllers / Views

## Jak uruchomić projekt

**Wymagania:**
- .NET 10 SDK
- MySQL Server

**Kroki:**
1. Sklonuj repozytorium
2. *(uzupełnić: konfiguracja connection string w `appsettings.json`)*
3. *(uzupełnić: zastosowanie migracji EF Core — `dotnet ef database update`?)*
4. `dotnet run`

*(sekcja do uzupełnienia dokładnymi krokami przy najbliższej okazji)*

## Kluczowe funkcjonalności

- Rejestracja i logowanie z wykorzystaniem ASP.NET Identity
- Zarządzanie użytkownikami i rolami w panelu administracyjnym
- Hierarchiczny model uprawnień — administrator może zarządzać tylko użytkownikami z ról podległych jego roli w hierarchii (a nie każdym użytkownikiem w systemie)
- Walidacja danych użytkownika (m.in. sumy kontrolnej numeru PESEL)
- Ochrona przed nieautoryzowaną zmianą uprawnień (użytkownik nie może przypisać roli, do której sam nie ma dostępu)

## Bezpieczeństwo

Jednym z celów projektu było praktyczne zastosowanie zasad bezpiecznego dostępu do danych (broken access control, OWASP):

- kontrola dostępu na poziomie zasobu (a nie tylko na poziomie roli) przy edycji i usuwaniu użytkowników
- ochrona przed privilege escalation przy przypisywaniu ról
- ochrona CSRF na akcjach zmieniających stan (logowanie, wylogowanie, edycja danych)
- przygotowana (skomentowana, do włączenia przed wdrożeniem produkcyjnym) polityka blokady konta po nieudanych próbach logowania oraz wymagań co do siły hasła

## Architektura

Projekt korzysta z podziału na warstwy (Domain, Application, Infrastructure) inspirowanego Clean Architecture, z pełnym rozdzieleniem logiki biznesowej od kontrolerów oraz wzorcem Unit of Work do zarządzania transakcjami. Niektóre uproszczenia architektoniczne zostały zastosowane świadomie, aby projekt pozostał czytelny jako materiał demonstracyjny.

## Dane startowe

Baza danych jest zasilana danymi startowymi (seed) automatycznie przy pierwszym uruchomieniu — role systemowe, przykładowa hierarchia uprawnień oraz konta testowe są tworzone bez ręcznej konfiguracji.

## Zrzuty ekranu

*(do dodania)*

## Hierarchia ról

Szczegółowy opis struktury hierarchii ról znajduje się w pliku `Hierarhia.txt` w katalogu głównym repozytorium.

## Konta demonstracyjne

*(Aktualne dane logowania. Możliwe zmiany - dane znajdują się w pliku `Dane logowania do systemu.txt`)*

## Planowane dalsze prace

**Techniczne:**
- testy jednostkowe i Moq dla kluczowych serwisów i funkcji (walidacja danych, logika hierarchii uprawnień)
- włączenie docelowej polityki siły haseł i blokady konta po nieudanych próbach logowania

**Funkcjonalne:**
- dodanie ucznia
- dodanie klas
- łączenie nauczycieli i klas
- dodanie planu zajęć

Okazjonalne code review na bieżąco rozwijanego kodu.
