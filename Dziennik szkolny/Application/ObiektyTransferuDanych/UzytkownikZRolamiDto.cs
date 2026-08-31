namespace Dziennik_szkolny.Application.ObiektyTransferuDanych
{
    public class UzytkownikZRolamiDto
    {
        public string Id { get; set; } = null!;
        public string? Login { get; set; }
        public List<string> Role { get; set; } = [];
    }
}
