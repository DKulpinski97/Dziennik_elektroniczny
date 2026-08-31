namespace Dziennik_szkolny.Infrastructure.DaneStartowe
{
    public class DaneStartowe
    {
        //Lista ról pracowników w systemie
        public List<string> RolePracownika { get; } =
        [
        "Admin",
        "SuperAdmin",
        "Nauczyciel",
        "Dyrektor",
        "ViceDyrektor",
        "Sekretarka",

        ];
        //Lista ról w systemie
        public List<string> Role { get; } =
        [
        "Admin",
        "SuperAdmin",
        "Nauczyciel",
        "Uczen",
        "Brak roli",
        "Dyrektor",
        "ViceDyrektor",
        "Sekretarka",
        "Rodzic"

        ];
        //Połączenia w rolach w systemie
        public List<(string RolaZarzadzajaca, string RolaZarzadzana)> UprawnieniaRoli { get; } =
[
    // SuperAdmin
    ("SuperAdmin", "Admin"),

    // Admin
    ("Admin", "Dyrektor"),
    ("Admin", "ViceDyrektor"),
    ("Admin", "Sekretarka"),
    ("Admin", "Nauczyciel"),
    ("Admin", "Rodzic"),

    // Dyrektor
    ("Dyrektor", "Sekretarka"),
    ("Dyrektor", "Nauczyciel"),
    ("Dyrektor", "Rodzic"),
            // Dyrektor
    ("ViceDyrektor", "Sekretarka"),
    ("ViceDyrektor", "Nauczyciel"),
    ("ViceDyrektor", "Rodzic"),

    // Sekretarka
    ("Sekretarka", "Uczen"),
    ("Sekretarka", "Rodzic")
];
        //Lista ról przypisanych do użytkowników w systemie
        public List<List<string>> PrzypisanieRoli { get; } =
   [
        ["Admin"],
        ["SuperAdmin"],
        ["Dyrektor"],
        ["Sekretarka"],
        ["Nauczyciel", "Rodzic"],
        ["Nauczyciel"],
        ["Nauczyciel"],
        ["Rodzic"],
        ["Rodzic"],
        ["Rodzic"]
   ];
        //Lista loginów i haseł użytkowników w systemie
        public List<string[]> Loginy { get; } =
        [
        new[] { "Admin", "Admin@gmail.com", "Admin" },
        new[] { "SuperAdmin", "SuperAdmin@gmail.com", "SuperAdmin" },
        new[] { "Dyrektor", "Dyrektor@gmail.com", "Dyrektor" },
        new[] { "Sekretarka", "Sekretarka@gmail.com", "Sekretarka" },

        new[] { "Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1" },
        new[] { "Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2" },
        new[] { "Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3" },

        new[] { "Rodzic1", "Rodzic1@gmail.com", "Rodzic1" },
        new[] { "Rodzic2", "Rodzic2@gmail.com", "Rodzic2" },
        new[] { "Rodzic3", "Rodzic3@gmail.com", "Rodzic3" }
        ];
        //Informacje o użytkownikach w systemie: Imię, Nazwisko, PESEL, Telefon, Miasto, Ulica, Numer domu/mieszkania
        public List<string[]> Informacje { get; } =
        [
        ["Admin", "Admin@gmail.com", "Admin", "AdminImie", "AdminNaz", "85010112345", "500100100", "Warszawa", "Marszałkowska", "11"],
        ["SuperAdmin", "SuperAdmin@gmail.com", "SuperAdmin", "SuperAdminImie", "SuperAdminNaz", "92031512347", "500100100", "Warszawa", "Marszałkowska", "11"],
        ["Dyrektor", "Dyrektor@gmail.com", "Dyrektor", "DyrektorImie", "DyrektorNaz", "82020223456", "500200200", "Warszawa", "Puławska", "14"],
        ["Sekretarka", "Sekretarka@gmail.com", "Sekretarka", "SekretarkaImie", "SekretarkaNaz", "90030334567", "500300300", "Warszawa", "Grochowska", "24"],

        ["Nauczyciel1", "Nauczyciel1@gmail.com", "Nauczyciel1", "Nauczyciel1Imie", "Nauczyciel1Naz", "88040445678", "500400400", "Warszawa", "Górczewska", "1"],
        ["Nauczyciel2", "Nauczyciel2@gmail.com", "Nauczyciel2", "Nauczyciel2Imie", "Nauczyciel2Naz", "87050556789", "500500500", "Warszawa", "Górczewska", "2"],
        ["Nauczyciel3", "Nauczyciel3@gmail.com", "Nauczyciel3", "Nauczyciel3Imie", "Nauczyciel3Naz", "86060667890", "500600600", "Warszawa", "Targowa", "41"],

        ["Rodzic1", "Rodzic1@gmail.com", "Rodzic1", "Rodzic1Imie", "Rodzic1Naz", "85070778901", "500700700", "Warszawa", "Modlińska", "25"],
        ["Rodzic2", "Rodzic2@gmail.com", "Rodzic2", "Rodzic2Imie", "Rodzic2Naz", "84080889012", "500800800", "Warszawa", "Wawelska", "15"],
        ["Rodzic3", "Rodzic3@gmail.com", "Rodzic3", "Rodzic3Imie", "Rodzic3Naz", "83090990123", "500900900", "Warszawa", "Białobrzeska", "26"]
        ];
    }
}
