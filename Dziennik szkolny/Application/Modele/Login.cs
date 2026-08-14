using System.ComponentModel.DataAnnotations;

namespace Dziennik_szkolny.Application.Modele
{
    public class Login
    {
        [Required]
        public string LoginUzytkownika { get; set; }

        [Required]
        public string HasloUzytkownika { get; set; }
    }
}
