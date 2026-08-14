namespace Dziennik_szkolny.Application.Interfejsy
{
    public interface IJednostkaPracy
    {
        Task RozpocznijTransakcjeAsync();
        Task ZatwierdzAsync();
        Task CofnijAsync();
    }
}
