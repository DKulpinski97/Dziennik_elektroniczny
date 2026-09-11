using Microsoft.AspNetCore.Identity;
using Moq;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Infrastructure.Serwisy.Role;
using Dziennik_szkolny.Infrastructure.DaneStartowe;

namespace Dziennik_szkolny.Tests.Infrastructure.Serwisy.Role
{
    public class ZarzadzajRolamiServiceTests
    {
        // ===== ZmienNazweRoli =====
        //sprawdzanie czy rola istnieje
        [Fact]
        public async Task ZmienNazweRoli_RolaNieIstnieje_ZwracaBladRolaNieIstnieje()
        {
            // Arrange
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object);

            // Act
            var wynik = await sut.ZmienNazweRoli("dowolneId", "NowaNazwa", "Stara nazwa");

            // Assert
            Assert.False(wynik.Sukces);
            Assert.Equal("Rola nie została znaleziona.", wynik.Komunikat);
            mockRoleManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
        }
        //sprawdzanie czy rola systemowa
        [Fact]
        public async Task ZmienNazweRoli_RolaSystemowa_ZwracaBladNazwaSystemu()
        {
            // Arrange
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);

            var rola = new IdentityRole { Id = "rola1", Name = "Admin" };
            mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(rola);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService( mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object);

            // Act
            var wynik = await sut.ZmienNazweRoli("rola1", "NowaNazwa", "Admin");

            // Assert
            Assert.False(wynik.Sukces);
            Assert.Equal("Nie można zmienić nazwy roli, która jest systemowa.", wynik.Komunikat);
        }
        //sprawdzanie czy zmienana rola ma tą samą nazwę co w bazie
        [Fact]
        public async Task ZmienNazweRoli_StaraNazwaNiezgodna_ZwracaBladWspolbieznosci()
        {
            // Arrange
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                mockRoleStore.Object, null, null, null, null);

            var rola = new IdentityRole { Id = "rola1", Name = "Nazwa" };
            mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(rola);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService( mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object);

            // Act
            var wynik = await sut.ZmienNazweRoli("rola1", "NowaNazwa", "InnaNazwaNizWBazie");

            // Assert
            Assert.False(wynik.Sukces);
            Assert.Equal("Rola została wcześniej zmieniona. Odśwież stronę i spróbuj ponownie.", wynik.Komunikat);
        }
        //Udana zmiana nazwy roli
        [Fact]
        public async Task ZmienNazweRoli_PoprawnaZmianaRoli_ZwracaTrueNazwaSukcesu()
        {
            // Arrange
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>( mockRoleStore.Object, null, null, null, null);

            var rola = new IdentityRole { Id = "rola1", Name = "StaraNazwa" };
            mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(rola);

            mockRoleManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            var sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object);

            // Act
            var wynik = await sut.ZmienNazweRoli("rola1", "NowaNazwa", "StaraNazwa");

            // Assert
            Assert.True(wynik.Sukces);
            Assert.Equal("Nazwa roli zmieniona pomyślnie.", wynik.Komunikat);
        }

        // ===== DodajRole =====

        //sprawdzanie czy rola nie istnieje
        [Fact]
        public async Task DodajRole_SprawdzanieCzyRolaNieIstnieje_ZwracaTrue()
        {
            // Arrange
            var mockRoleStory = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStory.Object, null, null, null, null);

            mockRoleManager.Setup(z => z.RoleExistsAsync("NazwaRolaNieIstnieje")).ReturnsAsync(false);
            mockRoleManager.Setup(z => z.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            var mockUprawnieniaRoli = new Mock<IPobierajUprawnieniaRoli>();

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnieniaRoli.Object);

            // Act
            bool wynik = await sut.DodajRole("NazwaRolaNieIstnieje");

            // Assert
            Assert.True(wynik);
            mockRoleManager.Verify(z => z.RoleExistsAsync("NazwaRolaNieIstnieje"), Times.Once);
            mockRoleManager.Verify(z => z.CreateAsync(It.Is<IdentityRole>(x => x.Name == "NazwaRolaNieIstnieje")), Times.Once);
        }

        //sprawdzanie czy rola istnieje
        [Fact]
        public async Task DodajRole_SprawdzanieCzyRolaIstnieje_ZwracaFalse()
        {
            // Arrange
            var mockRoleStory = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStory.Object, null, null, null, null);

            mockRoleManager.Setup(z => z.RoleExistsAsync("nazwaRolaIstnieje")).ReturnsAsync(true);

            var mockUprawnieniaRoli = new Mock<IPobierajUprawnieniaRoli>();

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnieniaRoli.Object);

            // Act
            bool wynik = await sut.DodajRole("nazwaRolaIstnieje");

            // Assert
            Assert.False(wynik);
            mockRoleManager.Verify(z => z.RoleExistsAsync("nazwaRolaIstnieje"), Times.Once);
        }

        // ===== UsunRole =====

        // Rola w hierarchii — wersja 1 (pierwotna)
        [Fact]
        public async Task UsunRole_RolaJestCzescianHierarchii_ZwracaBladINieUsuwa()
        {
            // Arrange
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            mockUprawnien.Setup(x => x.CzyRolaJestUzywanaWHierarchiiAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                mockRoleStore.Object, null, null, null, null);

            var sut = new ZarzadzajRolamiService(
                mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object);

            // Act
            var wynik = await sut.UsunRole("dowolneId");

            // Assert
            Assert.False(wynik.Sukces);
            Assert.Equal("Nie można usunąć roli, ponieważ jest częścią hierarchii zarządzania.", wynik.Komunikat);
            mockRoleManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Never);
        }

        //Rola poza systemem
        [Fact]
        public async Task UsunRole_RolaNieIstnieje_ZwracaFalseIKomunikat()
        {
            // Arrange
            var mockRoleStory = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStory.Object, null, null, null, null);

            mockRoleManager.Setup(z => z.FindByIdAsync("IdPozaSystemem")).ReturnsAsync((IdentityRole)null);

            var mockUprawnieniaRoli = new Mock<IPobierajUprawnieniaRoli>();
            mockUprawnieniaRoli.Setup(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdPozaSystemem")).ReturnsAsync(false);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnieniaRoli.Object);

            // Act
            var wynik = await sut.UsunRole("IdPozaSystemem");

            // Assert
            Assert.False(wynik.Sukces);
            Assert.Equal("Rola nie została znaleziona.", wynik.Komunikat);
            mockUprawnieniaRoli.Verify(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdPozaSystemem"), Times.Once);
            mockRoleManager.Verify(z => z.FindByIdAsync("IdPozaSystemem"), Times.Once);
        }
        //Rola systemowa
        [Fact]
        public async Task UsunRole_RolaJestSystemowa_ZwracaFalseIKomunikat()
        {
            // Arrange
            var mockRoleStory = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStory.Object, null, null, null, null);

            // TODO: obiekt IdentityRole zagnieżdżony bezpośrednio w ReturnsAsync — rozważ wydzielenie do nazwanej zmiennej (np. "var rola = new IdentityRole {...}")
            var rola = new IdentityRole { Id = "IdRoliSystemowej", Name = "Admin" };
            mockRoleManager.Setup(z => z.FindByIdAsync("IdRoliSystemowej")).ReturnsAsync(rola);

            var mockUprawnieniaRoli = new Mock<IPobierajUprawnieniaRoli>();
            mockUprawnieniaRoli.Setup(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliSystemowej")).ReturnsAsync(false);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnieniaRoli.Object);

            // Act
            var wynik = await sut.UsunRole("IdRoliSystemowej");

            // Assert
            Assert.False(wynik.Sukces);
            Assert.Equal("Nie można usunąć roli, która jest systemowa.", wynik.Komunikat);
            mockUprawnieniaRoli.Verify(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliSystemowej"), Times.Once);
            mockRoleManager.Verify(z => z.FindByIdAsync("IdRoliSystemowej"), Times.Once);
        }

        //Rola usunieta
        [Fact]
        public async Task UsunRole_RoladoUsuniecia_ZwracaTrueIKomunikat()
        {
            // Arrange
            var mockRoleStory = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStory.Object, null, null, null, null);

            var rola = new IdentityRole { Id = "IdRoliDoUsuniecia", Name = "RolaDoSkasowania" };
            mockRoleManager.Setup(z => z.FindByIdAsync("IdRoliDoUsuniecia")).ReturnsAsync(rola);
            mockRoleManager.Setup(z => z.DeleteAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            var mockUprawnieniaRoli = new Mock<IPobierajUprawnieniaRoli>();
            mockUprawnieniaRoli.Setup(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliDoUsuniecia")).ReturnsAsync(false);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnieniaRoli.Object);

            // Act
            var wynik = await sut.UsunRole("IdRoliDoUsuniecia");

            // Assert
            Assert.True(wynik.Sukces);
            Assert.Equal("Rola usunięta pomyślnie.", wynik.Komunikat);
            mockUprawnieniaRoli.Verify(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliDoUsuniecia"), Times.Once);
            mockRoleManager.Verify(z => z.FindByIdAsync("IdRoliDoUsuniecia"), Times.Once);
            mockRoleManager.Verify(z => z.DeleteAsync(It.Is<IdentityRole>(x => x.Name == "RolaDoSkasowania")), Times.Once);
        }

        //Rola usunieta błąd
        [Fact]
        public async Task UsunRole_BladRoladoUsuniecia_ZwracaFalseIKomunikat()
        {
            // Arrange
            var mockRoleStory = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStory.Object, null, null, null, null);

            var rola = new IdentityRole { Id = "IdRoliDoUsuniecia", Name = "RolaDoSkasowania" };
            mockRoleManager.Setup(z => z.FindByIdAsync("IdRoliDoUsuniecia")).ReturnsAsync(rola);
            mockRoleManager.Setup(z => z.DeleteAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed());

            var mockUprawnieniaRoli = new Mock<IPobierajUprawnieniaRoli>();
            mockUprawnieniaRoli.Setup(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliDoUsuniecia")).ReturnsAsync(false);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnieniaRoli.Object);

            // Act
            var wynik = await sut.UsunRole("IdRoliDoUsuniecia");

            // Assert
            Assert.False(wynik.Sukces);
            Assert.Equal("Nie udało się usunąć roli.", wynik.Komunikat);
            mockUprawnieniaRoli.Verify(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliDoUsuniecia"), Times.Once);
            mockRoleManager.Verify(z => z.FindByIdAsync("IdRoliDoUsuniecia"), Times.Once);
            mockRoleManager.Verify(z => z.DeleteAsync(It.Is<IdentityRole>(x => x.Name == "RolaDoSkasowania")), Times.Once);
        }
    }
}
