using System.ComponentModel.DataAnnotations;

namespace Dzienik_szkolny.Models
{
    public class InformacjePracownik
    {
        [Key]
        public long IdPracownika { get; set; }
        [Required]
        [MaxLength(20)]
        public string Imie { get; set; }
        [Required]
        [MaxLength(60)]
        public string Nazwisko { get; set; }
        [Required]
        [MaxLength(11)]
        public string Pesel { get; set; }


        // Referencja (właściwość nawigacyjna)
        public LoginUzytkownika Login { get; set; }
    }
}
