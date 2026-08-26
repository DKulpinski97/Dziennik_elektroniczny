using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Dziennik_szkolny.ViewModel
{
    public class UzytkownikaViewModel
    {
        [Required(ErrorMessage = "Login jest wymagane.")]
        [MaxLength(20, ErrorMessage = "Login może mieć maksymalnie 20 znaków.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format email.")]
        public string Email { get; set; }


        
        public string? Haslo { get; set; }


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
        [ValidateNever]
        public List<SelectListItem> DostepneRole { get; set; }

        public List<string> WybraneRole { get; set; }
        [ValidateNever]
        public string idUzytkownika { get; set; }
        [ValidateNever]
        public long IdDanych { get; set; }

    }
}