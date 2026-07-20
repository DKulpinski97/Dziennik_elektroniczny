using System.ComponentModel.DataAnnotations;

namespace Dzienik_szkolny.Models
{
    public class LoginUzytkownika
    {
        [Key]
        public long IdUzytkownika { get; set; }
        [Required]
        [MaxLength(20)]
        public string Login { get; set; }
        [Required]
        public string ZaszyfrowaneHaslo { get; set; }

    }
}
