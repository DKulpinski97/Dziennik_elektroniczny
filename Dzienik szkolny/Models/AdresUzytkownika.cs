using System.ComponentModel.DataAnnotations;

namespace Dzienik_szkolny.Models
{
    public class AdresUzytkownika
    {
        [Key]
        public long IdAdresu { get; set; }
        [Required]
        [MaxLength(60)]
        public string Miejscowosc { get; set; }
        [Required]
        [MaxLength(6)]
        public string KodPocztowy { get; set; }
        [Required]
        [MaxLength(100)]
        public string Adres { get; set; }
        [Required]
        [MaxLength(10)]
        public string NrMieszkania { get; set; }


        public long IdUzytkownika { get; set; }

        public List<InformacjeRodzic> Uzytkownikcy { get; set; }
    }
}
