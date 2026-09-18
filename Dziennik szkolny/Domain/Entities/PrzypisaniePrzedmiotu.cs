using Dziennik_szkolny.Infrastructure.Identyfikatory;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dziennik_szkolny.Domain.Entities
{
    [PrimaryKey(nameof(IdPrzedmiotu), nameof(IdNauczyciela))]
    public class PrzypisaniePrzedmiotu
    {
        public int IdPrzedmiotu { get; set; }
        [Required]
        [ForeignKey(nameof(IdPrzedmiotu))]
        public virtual Przedmiot Przedmiot { get; set; }

        public string IdNauczyciela { get; set; } = string.Empty;
        [Required]
        [ForeignKey(nameof(IdNauczyciela))]
        public virtual LoginUzytkownika Nauczyciel { get; set; }
    }
}
