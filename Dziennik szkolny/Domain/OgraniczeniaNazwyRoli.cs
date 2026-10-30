namespace Dziennik_szkolny.Domain
{
    public static class OgraniczeniaNazwyRoli
    {
        public const int MinimalnaDlugosc = 2;
        public const int MaksymalnaDlugosc = 50;
        public const string KomunikatBledu ="Nazwa roli musi mieć od 2 do 50 znaków. Znaki białe na początku i końcu nie są wliczane.";
        public const string Podpowiedz = "Słowa są łączone bez spacji i zaczynają się wielką literą, np. „it specjalista” daje „ItSpecjalista”.";
    }
}
