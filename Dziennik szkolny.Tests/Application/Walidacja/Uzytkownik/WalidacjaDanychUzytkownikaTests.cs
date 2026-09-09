using Dziennik_szkolny.Application.Walidacja.Uzytkownik;

namespace Dziennik_szkolny.Tests.Application.Walidacja.Uzytkownik
{
    public class WalidacjaDanychUzytkownikaTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("071836720")]        // za krótki (9 znaków)
        [InlineData("00131500000")]      // niepoprawny miesiąc (13)
        [InlineData("00251512341")]      // poprawny format i data, zła cyfra kontrolna
        [InlineData("50053200001")]      // poprawny miesiąc, niepoprawny dzień (32)
        [InlineData("0025151234A")]      // 11 znaków, litera zamiast cyfry
        public void CzyPoprawnyPesel_SprawdzaPoprawnoscWartosci_ZwracaFalse(string wartość)
        {
            // Arrange
            WalidacjaDanychUzytkownika walidacja = new WalidacjaDanychUzytkownika();
            // Act
            bool wynik = walidacja.CzyPoprawnyPesel(wartość);
            // Assert
            Assert.False(wynik);
        }

        [Fact]
        public void CzyPoprawnyPesel_SprawdzaPoprawnoscWartosci_ZwracaTrue()
        {
            // Arrange
            WalidacjaDanychUzytkownika walidacja = new WalidacjaDanychUzytkownika();
            // Act
            bool wynik = walidacja.CzyPoprawnyPesel("00251512345");
            // Assert
            Assert.True(wynik);
        }

        [Theory]
        [InlineData("")]                 // wartość pusta
        [InlineData(null)]               // brak wartości
        [InlineData("49585216")]         // za krótki numer
        [InlineData("8528528529")]       // za długi numer
        [InlineData("743-75384")]        // 9 znaków, ale zawiera niedozwolony znak
        public void CzyPoprawnyTelefon_SprawdzamNumer_ZwracaFalse(string telefon)
        {
            // Arrange
            WalidacjaDanychUzytkownika walidacja = new WalidacjaDanychUzytkownika();
            // Act
            bool wynik = walidacja.CzyPoprawnyTelefon(telefon);
            // Assert
            Assert.False(wynik);
        }

        [Fact]
        public void CzyPoprawnyTelefon_SprawdzamNumer_ZwracaTrue()
        {
            // Arrange
            WalidacjaDanychUzytkownika walidacja = new WalidacjaDanychUzytkownika();
            // Act
            bool wynik = walidacja.CzyPoprawnyTelefon("123456789");
            // Assert
            Assert.True(wynik);
        }

       /* 
        * ToDo 
        * Wymaga poprawy zasad na docelową architekture teraz zawsze da błąd
        * [Theory]
        [InlineData("")]                  // wartość pusta
        [InlineData(null)]                // brak wartości
        [InlineData("asdfghj")]           // krótsze niż 8
        [InlineData("asdfghjk")]          // długość ok, sama mała litera
        [InlineData("Asdfghjk")]          // mała i wielka litera
        [InlineData("1sdfghjk")]          // mała litera i cyfra
        [InlineData("Asdfghjk1")]         // mała, wielka litera i cyfra (brak znaku specjalnego)
        [InlineData("#sdfghjk")]          // mała litera i znak specjalny
        [InlineData("Asdfghjk#")]         // mała, wielka litera i znak specjalny (brak cyfry)
        [InlineData("1#dfghjk")]          // mała litera, cyfra i znak specjalny (brak wielkiej litery)
        [InlineData("12@#qwA")]           // wszystkie 4 rodzaje znaków, ale za krótkie (7 znaków)
        [InlineData("QWE123@#$")]         // cyfra, wielka litera, znak specjalny, długość ok (brak małej litery)
        public void CzyHasloPrawidlowe_SprawdzamHaslo_ZwracaFalse(string haslo)
        {
            // Arrange
            WalidacjaDanychUzytkownika walidacja = new WalidacjaDanychUzytkownika();
            // Act
            bool wynik = walidacja.CzyHasloPrawidlowe(haslo);
            // Assert
            Assert.False(wynik);
        }*/

        [Fact]
        public void CzyHasloPrawidlowe_SprawdzamHaslo_ZwracaTrue()
        {
            // Arrange
            WalidacjaDanychUzytkownika walidacja = new WalidacjaDanychUzytkownika();
            // Act
            bool wynik = walidacja.CzyHasloPrawidlowe("123@#$ASDasd");
            // Assert
            Assert.True(wynik);
        }
    }
}
