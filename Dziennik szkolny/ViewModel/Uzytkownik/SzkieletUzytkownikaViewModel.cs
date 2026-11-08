using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Dziennik_szkolny.ViewModel.Uzytkownik
{
    public abstract class SzkieletUzytkownikaViewModel
    {
        [Required(ErrorMessage = "Login jest wymagany.")]
        [MaxLength(20, ErrorMessage = "Login może mieć maksymalnie 20 znaków.")]
        public string Login { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "E-mail jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane.")]
        [MaxLength(20, ErrorMessage = "Imię może mieć maksymalnie 20 znaków.")]
        [Display(Name = "Imię")]
        public string Imie { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [MaxLength(60, ErrorMessage = "Nazwisko może mieć maksymalnie 60 znaków.")]
        public string Nazwisko { get; set; }

        [Display(Name = "PESEL")]
        [Required(ErrorMessage = "PESEL jest wymagany.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "PESEL musi mieć 11 cyfr.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL może składać się wyłącznie z cyfr.")]
        public string Pesel { get; set; }


        [Required(ErrorMessage = "Telefon jest wymagany.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Telefon musi mieć dokładnie 9 cyfr.")]
        [RegularExpression(@"^[0-9]{9}$", ErrorMessage = "Telefon może składać się wyłącznie z cyfr.")]
        public string Telefon { get; set; }


        [Required(ErrorMessage = "Miasto jest wymagane.")]
        [MaxLength(100, ErrorMessage = "Miasto może mieć maksymalnie 100 znaków.")]
        public string Miasto { get; set; }


        [Required(ErrorMessage = "Ulica jest wymagana.")]
        [MaxLength(100, ErrorMessage = "Ulica może mieć maksymalnie 100 znaków.")]
        public string Ulica { get; set; }

        [Display(Name = "Numer mieszkania")]
        [Required(ErrorMessage = "Numer mieszkania jest wymagany.")]
        [MaxLength(10, ErrorMessage = "Numer mieszkania może mieć maksymalnie 10 znaków.")]
        public string NrMieszkania { get; set; }

        [ValidateNever]
        //Są to role kture można przypisać użytkownikowi.
        public List<SelectListItem> DostepneRole { get; set; } = new();
        [MinLength(1, ErrorMessage = "Wybierz co najmniej jedną rolę.")]

        //Są to role które zostały przypisane użytkownikowi. Zawiera identyfikatory ról.
        public List<string> WybraneRole { get; set; } = new();

    }
}