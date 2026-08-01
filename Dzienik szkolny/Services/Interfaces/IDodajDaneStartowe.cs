using Dzienik_szkolny.Models;
using Dzienik_szkolny.ViewModels;

namespace Dziennik_szkolny.Services.Interfaces
{
    public interface IDodajDaneStartowe
    {
        public Task DodajDaneStartoweAsync();
        public InformacjeUzytkownik Przypiszdane(string[] Dane, string IDUzytkownika);
        public Task DodajRoleStartowe(List<string> role);
    }
}
