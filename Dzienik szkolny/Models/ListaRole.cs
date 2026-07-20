using System.ComponentModel.DataAnnotations;

namespace Dzienik_szkolny.Models
{
    public class ListaRole
    {
        [Key]
        public long IdRoli { get; set; }
        [Required]
        [MaxLength(20)]
        public string NazwaRoli { get; set; }
    }
}
