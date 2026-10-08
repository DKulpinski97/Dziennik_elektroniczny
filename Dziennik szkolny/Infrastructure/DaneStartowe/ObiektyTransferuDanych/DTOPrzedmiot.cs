using Dziennik_szkolny.Domain.Entities;

namespace Dziennik_szkolny.Infrastructure.DaneStartowe.ObiektyTransferuDanych
{
    //Fabryka została pominięta celowo, ponieważ obiekt nie wymaga dodatkowej metody tworzenia.
    //Jest to świadome odstępstwo, aby nie mnożyć sztucznych metod.
    //W tym przypadku konstruktor nie wykonuje żadnej transformacji w celu pozyskania danych do obiektu
    public class DTOPrzedmiot
    {
        public string NazwaPrzedmiotu { get; }
        public DTOPrzedmiot(string nazwaPrzedmiotu)
        {
            this.NazwaPrzedmiotu = nazwaPrzedmiotu;
        }
        public Przedmiot DoEncja()
        {
            return new Przedmiot
            {
                NazwaPrzedmiotu = this.NazwaPrzedmiotu
            };
        }
    }
}
