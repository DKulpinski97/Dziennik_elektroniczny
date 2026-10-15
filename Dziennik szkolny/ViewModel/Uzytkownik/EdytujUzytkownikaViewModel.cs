using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Dziennik_szkolny.ViewModel.Uzytkownik
{
    public  class EdytujUzytkownikaViewModel : SzkieletUzytkownikaViewModel
    {
        [ValidateNever]
        public string IdUzytkownika { get; set; }

        public string? Haslo { get; set; }
    }
}
