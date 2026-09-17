using Dziennik_szkolny.Infrastructure.Identyfikatory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dziennik_szkolny.Domain.Entities
{
    public class Klasa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdKlasy { get; set; }
        [Required]
        [MaxLength(100)]
        public string Oznaczenie { get; set; }
        [Required]
        public byte RokNauki { get; set; }
        public int RokRozpoczecia { get; set; }
 
        public int? RokZakonczenia { get; set; }

        public string? WychowawcaId { get; set; }
        [ForeignKey(nameof(WychowawcaId))]
        public LoginUzytkownika? Wychowawca { get; set; }
    
        /*dowiązanie na potem
         * List<Uczen> Uczniowie { get; set; } = new();
         */

    }
}
