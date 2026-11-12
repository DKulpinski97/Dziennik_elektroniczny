using Dziennik_szkolny.Domain;
using System.ComponentModel.DataAnnotations;

namespace Dziennik_szkolny.Application.ObiektyTransferuDanych
{
    public class NoweDaneRoli
    {
        private string _nazwa = string.Empty;

        [Required(ErrorMessage = "To pole jest wymagane.")]
        [StringLength(OgraniczeniaNazwyRoli.MaksymalnaDlugosc, MinimumLength = OgraniczeniaNazwyRoli.MinimalnaDlugosc, ErrorMessage = OgraniczeniaNazwyRoli.KomunikatBledu)]
        public string Nazwa
        {
            get => _nazwa;
            set => _nazwa = value?.Trim() ?? string.Empty;
        }
    }
}
