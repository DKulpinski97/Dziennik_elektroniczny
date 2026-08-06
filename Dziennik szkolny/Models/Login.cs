using System.ComponentModel.DataAnnotations;

namespace Dziennik_szkolny.Models
{
    public class Login
    {
        [Required]
        public string LoginUzytkownika { get; set; }

        [Required]
        public string HasloUzytkownika { get; set; }
    }
}
