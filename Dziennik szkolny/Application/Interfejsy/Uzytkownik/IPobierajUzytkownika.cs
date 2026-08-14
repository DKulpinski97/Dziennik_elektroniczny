using Dziennik_szkolny.Domain.Entities;
using Dziennik_szkolny.Infrastructure.Identyfikatory;

namespace Dziennik_szkolny.Application.Interfejsy.Uzytkownik
{
    public interface IPobierajUzytkownika
    {
        Task<LoginUzytkownika> PobierzUzytkownikaPoLoginieAsync(string login);
        Task<InformacjeUzytkownik> PobierzInformacjeUzytkownikaPoIdLoginu(string IdUzytkownika);
    }
}
