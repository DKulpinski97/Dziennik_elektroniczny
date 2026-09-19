using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dziennik_szkolny.Domain.Entities
{
    [Index(nameof(Pesel), IsUnique = true)]
    public class Uczen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUcznia { get; set; }
        [Required]
        [MaxLength(11)]
        public string Pesel { get; set; }
        [Required]
        [MaxLength(30)]
        public string Imie { get; set; }
        [Required]
        [MaxLength(60)]
        public string Nazwisko { get; set; }
        [Required]
        public DateOnly DataUrodzenia { get; set; }

        //nawigacjie
        public int IdKlasy { get; set; }
        [ForeignKey(nameof(IdKlasy))]
        public virtual Klasa Klasa { get; set; }

        [Required]
        public string IdOpiekun1 { get; set; }
        [ForeignKey(nameof(IdOpiekun1))]
        public virtual LoginUzytkownika Opiekun1 { get; set; }
        public string? IdOpiekun2 { get; set; }
        [ForeignKey(nameof(IdOpiekun2))]
        public virtual LoginUzytkownika? Opiekun2 { get; set; }


    }
}
