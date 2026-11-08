using Dziennik_szkolny.Application.Walidacja.Uzytkownik;
using Dziennik_szkolny.Domain.reguly;

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
        [InlineData("\u0660\u0661\u0662\u0665\u0661\u0665\u0661\u0662\u0663\u0664\u0665")] // cyfry Unicode zamiast ASCII
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
        [InlineData("\u0661\u0662\u0663\u0664\u0665\u0666\u0667\u0668\u0669")] // cyfry Unicode zamiast ASCII
        [InlineData("123456789\n")] // zawiera znak specjalny (nowa linia)
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

        [Theory]
        [InlineData("")]                  // wartość pusta
        [InlineData(null)]                // brak wartości
        [InlineData("A1a@")]              // wszystkie 4 rodzaje, ale tylko 4 znaki
        [InlineData("Ab1@cde")]           // wszystkie 4 rodzaje, za krótkie (7 znaków)
        [InlineData("asdfghjk")]          // długość ok, sama mała litera
        [InlineData("Asdfghjk")]          // mała i wielka litera
        [InlineData("1sdfghjk")]          // mała litera i cyfra
        [InlineData("Asdfghjk1")]         // brak znaku specjalnego
        [InlineData("#sdfghjk")]          // mała litera i znak specjalny
        [InlineData("Asdfghjk#")]         // brak cyfry
        [InlineData("1#dfghjk")]          // brak wielkiej litery
        [InlineData("QWE123@#$")]         // brak małej litery
        public void CzyHasloPrawidlowe_SprawdzamHaslo_ZwracaFalse(string? haslo)
        {
            // Arrange
            WalidacjaDanychUzytkownika walidacja = new WalidacjaDanychUzytkownika();
            // Act
            bool wynik = walidacja.CzyHasloPrawidlowe(haslo);
            // Assert
            Assert.False(wynik);
        }

        [Theory]
        [InlineData("Ab1@cdef")]          // dokładnie 8 znaków (granica), wszystkie 4 rodzaje
        [InlineData("123@#$ASDasd")]      // dłuższe, wszystkie 4 rodzaje
        public void CzyHasloPrawidlowe_SprawdzamHaslo_ZwracaTrue(string haslo)
        {
            // Arrange
            WalidacjaDanychUzytkownika walidacja = new WalidacjaDanychUzytkownika();
            // Act
            bool wynik = walidacja.CzyHasloPrawidlowe(haslo);
            // Assert
            Assert.True(wynik);
        }

        [Fact]
        public void OgraniczeniaHasla_KomunikatBledu_ZawieraMinimalnaDlugosc()
        {
            // Assert
            Assert.Contains(OgraniczeniaHasla.MinimalnaDlugosc.ToString(), OgraniczeniaHasla.KomunikatBledu);
        }
    }
}
