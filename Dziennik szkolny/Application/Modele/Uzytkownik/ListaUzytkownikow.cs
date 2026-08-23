using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dziennik_szkolny.Application.Modele.Uzytkownik
{
    public class ListaUzytkownikow
    {
        public List<SelectListItem> Rodzice { get; set; } = new();
        public List<SelectListItem> Pracownicy { get; set; } = new();
    }
}
