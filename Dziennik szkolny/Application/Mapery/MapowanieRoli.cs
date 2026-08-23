using Dziennik_szkolny.Application.ObiektyTransferuDanych;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dziennik_szkolny.Application.Mapery
{
    public class MapowanieRoli
    {
        public List<SelectListItem> MapujRoleNaSelectList(List<DaneRoli> role)
        {
            return role.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.Nazwa
            }).ToList();
        }
    }
}
