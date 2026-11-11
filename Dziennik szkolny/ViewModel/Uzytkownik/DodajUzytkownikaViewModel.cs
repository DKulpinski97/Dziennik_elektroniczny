using System.ComponentModel.DataAnnotations;


namespace Dziennik_szkolny.ViewModel.Uzytkownik
{
    public class DodajUzytkownikaViewModel : SzkieletUzytkownikaViewModel
    {
        [Display(Name = "Hasło")]
        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Haslo { get; set; }
    }
}

