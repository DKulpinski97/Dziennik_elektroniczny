using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dziennik_szkolny.Domain.Entities
{
    public enum DzienTygodnia { Poniedzialek, Wtorek, Sroda, Czwartek, Piatek }
    public class WpisPlanu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdWpisu { get; set; }
        [Required]
        [Range(1, 8, ErrorMessage = "Numer lekcji musi być w przedziale od 1 do 8.")]
        public byte Lekcja { get; set; }
        [Required]
        public DzienTygodnia Dzien { get; set; }
        public int IdKlasy { get; set; }
        [Required]
        [ForeignKey(nameof(IdKlasy))]
        public Klasa Klasa { get; set; }
        public int IdPrzedmiotu { get; set; }
        public string IdNauczyciela { get; set; }
        public PrzypisaniePrzedmiotu PrzypisaniePrzedmiotu { get; set; }
        
    }
}
