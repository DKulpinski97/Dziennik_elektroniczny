using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dziennik_szkolny.Domain.Entities
{
    [Index(nameof(NazwaPrzedmiotu), IsUnique = true)]
    public class Przedmiot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPrzedmiotu { get; set; }
        [Required]
        [MaxLength(50)]
        public string NazwaPrzedmiotu { get; set; }

         //odwołania
         public virtual List<PrzypisaniePrzedmiotu> Nauczyciele { get; set; } = new List<PrzypisaniePrzedmiotu>();
         
    }
}
