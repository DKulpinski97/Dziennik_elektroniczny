using Dziennik_szkolny.Domain;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{

    public class DaneStartowe
    {
        //Lista ról pracowników w systemie
        public List<string> RolePracownika { get; } =
        [
        NazwyRoli.Admin,
        NazwyRoli.SuperAdmin,
        NazwyRoli.Nauczyciel,
        NazwyRoli.Dyrektor,
        NazwyRoli.ViceDyrektor,
        NazwyRoli.Sekretarka,

        ];
        //Lista ról w systemie
        public List<string> Role { get; } =
        [
        NazwyRoli.Admin,
        NazwyRoli.SuperAdmin,
        NazwyRoli.Nauczyciel,
        NazwyRoli.Uczen,
        NazwyRoli.BrakRoli,
        NazwyRoli.Dyrektor,
        NazwyRoli.ViceDyrektor,
        NazwyRoli.Sekretarka,
        NazwyRoli.Rodzic

        ];
        //Połączenia w rolach w systemie
        public List<(string RolaZarzadzajaca, string RolaZarzadzana)> UprawnieniaRoli { get; } =
[
    // SuperAdmin
    (NazwyRoli.SuperAdmin, NazwyRoli.Admin),

    // Admin
    (NazwyRoli.Admin, NazwyRoli.Dyrektor),
    (NazwyRoli.Admin, NazwyRoli.ViceDyrektor),
    (NazwyRoli.Admin, NazwyRoli.Sekretarka),
    (NazwyRoli.Admin, NazwyRoli.Nauczyciel),
    (NazwyRoli.Admin, NazwyRoli.Rodzic),

    // Dyrektor
    (NazwyRoli.Dyrektor, NazwyRoli.Sekretarka),
    (NazwyRoli.Dyrektor, NazwyRoli.Nauczyciel),
    (NazwyRoli.Dyrektor, NazwyRoli.Rodzic),
    // ViceDyrektor
    (NazwyRoli.ViceDyrektor, NazwyRoli.Sekretarka),
    (NazwyRoli.ViceDyrektor, NazwyRoli.Nauczyciel),
    (NazwyRoli.ViceDyrektor, NazwyRoli.Rodzic),

    // Sekretarka
    (NazwyRoli.Sekretarka, NazwyRoli.Uczen),
    (NazwyRoli.Sekretarka, NazwyRoli.Rodzic)
];
        //Lista ról przypisanych do użytkowników w systemie
        public List<List<string>> PrzypisanieRoli { get; } =
   [
        [NazwyRoli.Admin],
        [NazwyRoli.SuperAdmin],
        [NazwyRoli.Dyrektor],
        [NazwyRoli.Sekretarka],
        [NazwyRoli.Nauczyciel, NazwyRoli.Rodzic],
        [NazwyRoli.Nauczyciel],
        [NazwyRoli.Nauczyciel],
        [NazwyRoli.Rodzic],
        [NazwyRoli.Rodzic],
        [NazwyRoli.Rodzic]
   ];
        //Informacje do tworzenia Loginów
        public List<(string Login, string Email, string Haslo)> Loginy { get; } =
         [
             ("Admin", "Admin@gmail.com", "Admin"),
            ("SuperAdmin", "SuperAdmin@gmail.com", "SuperAdmin"),
            ("Dyrektor", "Dyrektor@gmail.com", "Dyrektor"),
            ("Sekretarka", "Sekretarka@gmail.com", "Sekretarka"),

            ("Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1"),
            ("Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2"),
            ("Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3"),

            ("Rodzic1", "Rodzic1@gmail.com", "Rodzic1"),
            ("Rodzic2", "Rodzic2@gmail.com", "Rodzic2"),
            ("Rodzic3", "Rodzic3@gmail.com", "Rodzic3"),
        ];
        //Dane do utworzenia informacji o użytkowniku.
        //Hasło i e-mail są dodane dla czytelności i nie są używane przy tworzeniu encji InformacjeUzytkownik,
        //ponieważ znajdują się już w liście Loginy.
        //Login służy do odnalezienia identyfikatora użytkownika w słowniku tłumaczeń (login na Id).
        public List<(string Login, string Email, string Haslo, string Imie, string Nazwisko, string Pesel, string Telefon, string Miasto, string Ulica, string NrMieszkania)> Informacje { get; } =
         [
             ("Admin", "Admin@gmail.com", "Admin", "AdminImie", "AdminNaz", "85010112345", "500100100", "Warszawa", "Marszałkowska", "11"),
            ("SuperAdmin", "SuperAdmin@gmail.com", "SuperAdmin", "SuperAdminImie", "SuperAdminNaz", "92031512342", "500100100", "Warszawa", "Marszałkowska", "11"),
            ("Dyrektor", "Dyrektor@gmail.com", "Dyrektor", "DyrektorImie", "DyrektorNaz", "82020223452", "500200200", "Warszawa", "Puławska", "14"),
            ("Sekretarka", "Sekretarka@gmail.com", "Sekretarka", "SekretarkaImie", "SekretarkaNaz", "90030334565", "500300300", "Warszawa", "Grochowska", "24"),

            ("Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1", "Nauczyciel1Imie", "Nauczyciel1Naz", "88040445670", "500400400", "Warszawa", "Górczewska", "1"),
            ("Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2", "Nauczyciel2Imie", "Nauczyciel2Naz", "87050556781", "500500500", "Warszawa", "Górczewska", "2"),
            ("Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3", "Nauczyciel3Imie", "Nauczyciel3Naz", "86060667892", "500600600", "Warszawa", "Targowa", "41"),

            ("Rodzic1", "Rodzic1@gmail.com", "Rodzic1", "Rodzic1Imie", "Rodzic1Naz", "85070778903", "500700700", "Warszawa", "Modlińska", "25"),
            ("Rodzic2", "Rodzic2@gmail.com", "Rodzic2", "Rodzic2Imie", "Rodzic2Naz", "84080889014", "500800800", "Warszawa", "Wawelska", "15"),
            ("Rodzic3", "Rodzic3@gmail.com", "Rodzic3", "Rodzic3Imie", "Rodzic3Naz", "83090990125", "500900900", "Warszawa", "Białobrzeska", "26"),
        ];
        //Informacje niezbędne do utworzenia klasy
        public List<(string Oznaczenie, string RokNauki, string RokRozpoczecia, string? RokZakonczenia, string LoginWychowawcy)> Klasy { get; } =
        [
        ("A", "4", "2023", "", "Nauczyciel1"),
        ("B", "4", "2023", "", "Nauczyciel1"),
        ("Chemiczno fizyczna", "3", "2024", "", "Nauczyciel2"),
        ("Matematyczna", "2", "2025", "", "Nauczyciel3"),

        ];
        //Informacje do tworzenia ucznia
        public List<(string Pesel, string Imie, string Nazwisko, string DataUrodzenia, string OznaczenieKlasy, string RokRozpoczeciaKlasy, string Opiekun1Login, string Opiekun2Login)> Uczniowie { get; } =
        [
        ("13241265419", "Jan", "Kowalski", "2010-03-15", "A", "2023", "Rodzic1", ""),
        ("12290302544", "Anna", "Nowak", "2010-07-22", "A", "2023", "Rodzic2", ""),
        ("14212725033", "Piotr", "Wiśniewski", "2009-11-05", "Chemiczno fizyczna", "2024", "Rodzic3", ""),
        ("13311514209", "Katarzyna", "Lewandowska", "2008-01-30", "Matematyczna", "2025", "Rodzic1", "Rodzic2"),
        ("12263069290", "Marek", "Zieliński", "2008-05-12", "Matematyczna", "2025", "Rodzic3", ""),

        ];
        //Informacje o liście przedmiotów
        public List<string> Przedmioty { get; } =
        [
        NazwyPrzedmiotu.Polski,
        NazwyPrzedmiotu.Matematyka,
        NazwyPrzedmiotu.Fizyka,
        NazwyPrzedmiotu.Chemia
        ];
        //Informacje niezbędne do utworzenia PrzypisaniaPrzedmiotu (który nauczyciel uczy jakiego przedmiotu)
        public List<(string NazwaPrzedmiotu, string LoginNauczyciela)> PrzypiszPrzedmioty { get; } =
        [
        (NazwyPrzedmiotu.Polski, "Nauczyciel1"),
        (NazwyPrzedmiotu.Polski, "Nauczyciel3"),
        (NazwyPrzedmiotu.Matematyka, "Nauczyciel1"),
        (NazwyPrzedmiotu.Fizyka, "Nauczyciel2"),
        (NazwyPrzedmiotu.Chemia, "Nauczyciel3"),

        ];

        // Informacje niezbędne do utworzenia WpisyPlanu.
        // Z powodu tego, że WpisPlanu jest encją powiązaną z innymi encjami, musimy wyliczyć identyfikatory
        // encji powiązanych: klasa na podstawie oznaczenia i roku rozpoczęcia,
        // przedmiot na podstawie nazwy przedmiotu i nauczyciel na podstawie loginu.
        public List<(string NrLekcja, string Dzien, string Oznaczenie, string RokRozpoczecia, string NazwaPrzedmiotu, string LoginNauczyciela)> WpisyPlanu { get; } =
        [
        ("1", "Poniedzialek", "A", "2023", NazwyPrzedmiotu.Polski, "Nauczyciel1"),
        ("1", "Poniedzialek", "B", "2023", NazwyPrzedmiotu.Polski, "Nauczyciel3"),
        ("2", "Wtorek", "A", "2023", NazwyPrzedmiotu.Matematyka, "Nauczyciel1"),
        ("3", "Sroda", "A", "2023", NazwyPrzedmiotu.Fizyka, "Nauczyciel2"),
        ("4", "Czwartek", "A", "2023", NazwyPrzedmiotu.Chemia, "Nauczyciel3"),

        ];
    }
}


