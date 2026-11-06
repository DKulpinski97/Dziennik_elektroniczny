using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Dziennik_szkolny.ViewModel.Uzytkownik
{
    public  class EdytujUzytkownikaViewModel : SzkieletUzytkownikaViewModel
    {
        [ValidateNever]
        public string IdUzytkownika { get; set; }
        [Display(Name = "Hasło")]
        public string? Haslo { get; set; }
    }
}
