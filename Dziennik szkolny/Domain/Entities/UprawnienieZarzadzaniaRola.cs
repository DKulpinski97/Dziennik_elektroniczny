namespace Dziennik_szkolny.Domain.Entities
{
    public class UprawnienieZarzadzaniaRola
    {
        public int Id { get; set; }

        public string RolaZarzadzajacaId { get; set; } = null!;

        public string RolaZarzadzanaId { get; set; } = null!;
    }
}
