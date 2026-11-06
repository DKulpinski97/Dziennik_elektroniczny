using System.ComponentModel.DataAnnotations;

namespace Dziennik_szkolny.Application.Modele
{
    public class Login
    {
        [Required(ErrorMessage = "Login jest wymagany.")]
        public string LoginUzytkownika { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        public string HasloUzytkownika { get; set; }
    }
}
