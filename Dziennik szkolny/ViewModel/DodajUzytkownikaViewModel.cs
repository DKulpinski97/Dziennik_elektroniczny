using System.ComponentModel.DataAnnotations;

namespace Dziennik_szkolny.ViewModel
{
    public class DodajUzytkownikaViewModel : SzkieletUzytkownikaViewModel
    {
        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string Haslo { get; set; }
    }
}

