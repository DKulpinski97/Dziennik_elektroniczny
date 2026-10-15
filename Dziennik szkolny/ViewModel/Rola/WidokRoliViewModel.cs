using System.Collections.Generic;
using Dziennik_szkolny.Application.ObiektyTransferuDanych;

namespace Dziennik_szkolny.ViewModel.Rola
{
    public class WidokRoliViewModel
    {
        public List<DaneRoli> RoleSystemowe { get; set; }
        public List<DaneRoli> PozostaleRole { get; set; }
    }
}

