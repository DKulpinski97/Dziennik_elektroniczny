using System.ComponentModel.DataAnnotations;

namespace Dzienik_szkolny.Models
{
    public class InformacjeRodzic
    {

        [Key]
        public long IdRodzica { get; set; }
        [Required]
        [MaxLength(20)]
        public string Imie { get; set; }
        [Required]
        [MaxLength(60)]
        public string Nazwisko { get; set; }
        [Required]
        [MaxLength(11)]
        public string Pesel { get; set; }
        [Required]
        [MaxLength(9)]
        public string Telefon { get; set; }

        public long IdUzytkownika { get; set; }
        public long IdAdresu { get; set; }

        // Referencja (właściwość nawigacyjna)
        public LoginUzytkownika Login { get; set; }
        public AdresUzytkownika Adres { get; set; }
    }
}
