namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IWeryfikacjaDanychLogowania
    {
        Task<bool> CzyIstniejeLoginAsync(string login);

        Task<bool> CzyIstniejeEmailAsync(string email);

        Task<bool> CzyIstniejeInnyLoginAsync(string login,string idUzytkownika);

        Task<bool> CzyIstniejeInnyEmailAsync(string email,string idUzytkownika);
    }
}
