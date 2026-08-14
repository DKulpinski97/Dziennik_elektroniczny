namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IWeryfikacjaDanychLogowania
    {
        Task<bool> CzyIstniejeLoginAsync(string login);

        Task<bool> CzyIstniejeEmailAsync(string email);
    }
}
