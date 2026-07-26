using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Dzienik_szkolny.ViewModels
{
    public class DodajUzytkownikaViewModel
    {
        [Required(ErrorMessage = "Login jest wymagane.")]
        [MaxLength(20, ErrorMessage = "Login może mieć maksymalnie 20 znaków.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format email.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć minimum 6 znaków.")]
        public string Haslo { get; set; }


        [Required(ErrorMessage = "Imię jest wymagane.")]
        [MaxLength(20, ErrorMessage = "Imię może mieć maksymalnie 20 znaków.")]
        public string Imie { get; set; }


        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [MaxLength(60, ErrorMessage = "Nazwisko może mieć maksymalnie 60 znaków.")]
        public string Nazwisko { get; set; }


        [Required(ErrorMessage = "PESEL jest wymagany.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "PESEL musi mieć 11 cyfr.")]
        public string Pesel { get; set; }


        [Required(ErrorMessage = "Telefon jest wymagany.")]
        public string Telefon { get; set; }


        [Required(ErrorMessage = "Miasto jest wymagane.")]
        public string Miasto { get; set; }


        [Required(ErrorMessage = "Ulica jest wymagana.")]
        public string Ulica { get; set; }


        [Required(ErrorMessage = "Numer mieszkania jest wymagany.")]
        public string NrMieszkania { get; set; }

        public List<IdentityRole> Role { get; set; } = new();

        public List<string> IdRoli { get; set; } = new();

    }
}