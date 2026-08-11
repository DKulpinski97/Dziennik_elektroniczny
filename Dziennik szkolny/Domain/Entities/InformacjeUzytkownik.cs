using Dziennik_szkolny.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dziennik_szkolny.Domain.Entities
{
    public class InformacjeUzytkownik
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdOsoby { get; set; }
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
        [Required]
        [MaxLength(100)]
        public string Miasto { get; set; }
        [Required]
        [MaxLength(100)]
        public string Ulica { get; set; }
        [Required]
        [MaxLength(10)]
        public string NrMieszkania { get; set; }

        public string IdUzytkownika { get; set; }

        [ForeignKey(nameof(IdUzytkownika))]
        public LoginUzytkownika LoginUzytkownika { get; set; }

    }
}
