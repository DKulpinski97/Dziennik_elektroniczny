namespace Dziennik_szkolny.Domain
{
    /// <summary>
    /// Jedno wspólne źródło nazw ról systemowych — zamiast wpisywać "Admin", "Dyrektor" itd.
    /// jako gołe stringi w wielu miejscach (Authorize, DaneStartowe, widoki), odwołujemy się
    /// do tych stałych. Literówka w nazwie stałej jest błędem kompilacji, a nie cichym
    /// błędem w runtime.
    /// </summary>
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
        public const string BrakRoli = "Brak roli";

        // Gotowe kombinacje ról do użycia w [Authorize(Roles = "...")],
        // które wymaga stałej znanej w czasie kompilacji.
        public const string SuperAdminAdminDyrektor = SuperAdmin + "," + Admin + "," + Dyrektor;
    }
}
