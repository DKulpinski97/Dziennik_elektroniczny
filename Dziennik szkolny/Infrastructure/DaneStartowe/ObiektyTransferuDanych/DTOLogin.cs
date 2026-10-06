using Dziennik_szkolny.Infrastructure.Identyfikatory;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych
{
    //Fabryka została pominięta celowo, ponieważ obiekt nie wymaga dodatkowej metody tworzenia.
    //Jest to świadome odstępstwo, aby nie mnożyć sztucznych metod.
    //W tym przypadku konstruktor nie wykonuje żadnej transformacji w celu pozyskania danych do obiektu
    public class DTOLogin
    {
        public string Login { get; }
        public string Email { get; }
        public string Haslo { get; }

        public DTOLogin(string login, string email, string haslo)
        {
            this.Login = login;
            this.Email = email;
            this.Haslo = haslo;
        }
        public LoginUzytkownika DoEncja()
        {
            return new LoginUzytkownika
            {
                UserName = this.Login,
                Email = this.Email,
            };
        }
    }
}

