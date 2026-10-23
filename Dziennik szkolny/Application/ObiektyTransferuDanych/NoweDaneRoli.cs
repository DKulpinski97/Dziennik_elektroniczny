using System.ComponentModel.DataAnnotations;

namespace Dziennik_szkolny.Application.ObiektyTransferuDanych
{
    public class NoweDaneRoli
    {
        private string _nazwa = string.Empty;

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwa roli musi mieć od 2 do 50 znaków. Nie liczymy znaków białych na początku i na końcu.")]
        public string Nazwa
        {
            get => _nazwa;
            set => _nazwa = value?.Trim() ?? string.Empty;
        }
    }
}
