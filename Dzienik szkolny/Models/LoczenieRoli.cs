using System.ComponentModel.DataAnnotations;

namespace Dzienik_szkolny.Models
{
    public class LoczenieRoli
    {
        [Key]
        public long IdLoczenia { get; set; }
        public long IdUzytkownika { get; set; }
        public long IdRoli { get; set; }

        // Referencja (właściwość nawigacyjna)
        public LoginUzytkownika Login { get; set; }
        public ListaRole Rola { get; set; }
    }
}
