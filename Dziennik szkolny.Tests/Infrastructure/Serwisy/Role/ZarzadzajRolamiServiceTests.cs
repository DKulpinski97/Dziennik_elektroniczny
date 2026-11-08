using Dziennik_szkolny.Application.Interfejsy;
using Dziennik_szkolny.Application.Interfejsy.Role;
using Dziennik_szkolny.Application.Interfejsy.Uzytkownik;
using Dziennik_szkolny.Infrastructure;
using Dziennik_szkolny.Infrastructure.DaneStartowe;
using Dziennik_szkolny.Infrastructure.Serwisy.Role;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

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
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
                jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);
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
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            var rola = new IdentityRole { Id = "rola1", Name = "Admin" };
            mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(rola);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
               jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

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
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            var rola = new IdentityRole { Id = "rola1", Name = "Nazwa" };
             mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(rola);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
              jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);


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
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            var rola = new IdentityRole { Id = "rola1", Name = "StaraNazwa" };
             mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(rola);

             mockRoleManager.Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);


            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
              jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

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
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            mockRoleManager.Setup(z => z.RoleExistsAsync("NazwaRolaNieIstnieje")).ReturnsAsync(false);
             mockRoleManager.Setup(z => z.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
              jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

            // Act
            var  wynik = await sut.DodajRole("NazwaRolaNieIstnieje");

             // Assert
             Assert.True(wynik.Sukces);
             mockRoleManager.Verify(z => z.RoleExistsAsync("NazwaRolaNieIstnieje"), Times.Once);
             mockRoleManager.Verify(z => z.CreateAsync(It.Is<IdentityRole>(x => x.Name == "NazwaRolaNieIstnieje")), Times.Once);
         }

         //sprawdzanie czy rola istnieje
         [Fact]
         public async Task DodajRole_SprawdzanieCzyRolaIstnieje_ZwracaFalse()
         {
            // Arrange
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            mockRoleManager.Setup(z => z.RoleExistsAsync("nazwaRolaIstnieje")).ReturnsAsync(true);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
              jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

            // Act
            var  wynik = await sut.DodajRole("nazwaRolaIstnieje");

             // Assert
             Assert.False(wynik.Sukces);
             mockRoleManager.Verify(z => z.RoleExistsAsync("nazwaRolaIstnieje"), Times.Once);
         }

         // ===== UsunRole =====

         // Rola w hierarchii — wersja 1 (pierwotna)
         [Fact]
        public async Task UsunRole_RolaJestCzescianHierarchii_ZwracaBladINieUsuwa()
        {
            // Arrange
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            mockUprawnien.Setup(x => x.CzyRolaMaPowiazaniaPozaAdminemAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                mockRoleStore.Object, null, null, null, null);

            mockRoleManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityRole("TestowaRola"){Id = "dowolneId"});

            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();


            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
              jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

            // Act
            var wynik = await sut.UsunRole("dowolneId", "adminId");

            // Assert
            Assert.False(wynik.Sukces);

            Assert.Equal("Nie można usunąć roli, ponieważ jest częścią hierarchii zarządzania.",wynik.Komunikat);

            mockRoleManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()),Times.Once);
        }

        // Rola poza systemem
        [Fact]
        public async Task UsunRole_RolaNieIstnieje_ZwracaFalseIKomunikat()
        {
            // Arrange
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            mockRoleManager.Setup(z => z.FindByIdAsync("IdPozaSystemem")).ReturnsAsync((IdentityRole)null);

            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
              jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

            // Act
            var wynik = await sut.UsunRole("IdPozaSystemem", "adminId");

            // Assert
            Assert.False(wynik.Sukces);
            Assert.Equal("Rola nie została znaleziona.", wynik.Komunikat);

            mockRoleManager.Verify( z => z.FindByIdAsync("IdPozaSystemem"),Times.Once);

            mockUprawnien.Verify(
                z => z.CzyRolaMaPowiazaniaPozaAdminemAsync(It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }
        // Rola systemowa
        [Fact]
        public async Task UsunRole_RolaJestSystemowa_ZwracaFalseIKomunikat()
        {
            // Arrange
            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
            var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);

            var rola = new IdentityRole{
                Id = "IdRoliSystemowej",
                Name = "Dyrektor"
            };

            mockRoleManager.Setup(z => z.FindByIdAsync("IdRoliSystemowej")).ReturnsAsync(rola);
            
            var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
               jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

            // Act
            var wynik = await sut.UsunRole("IdRoliSystemowej", "adminId");

            // Assert
            Assert.False(wynik.Sukces);

            Assert.Equal("Nie można usunąć roli, która jest systemowa.",wynik.Komunikat);

            mockRoleManager.Verify( z => z.FindByIdAsync("IdRoliSystemowej"), Times.Once);

            mockUprawnien.Verify(z => z.CzyRolaMaPowiazaniaPozaAdminemAsync(It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }

        //Rola usunieta
        [Fact(Skip = "wymaga bazy w pamięci")]
        public async Task UsunRole_RoladoUsuniecia_ZwracaTrueIKomunikat()
         {
            // Arrange
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
             var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);

             var rola = new IdentityRole { Id = "IdRoliDoUsuniecia", Name = "RolaDoSkasowania" };
             mockRoleManager.Setup(z => z.FindByIdAsync("IdRoliDoUsuniecia")).ReturnsAsync(rola);
             mockRoleManager.Setup(z => z.DeleteAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

             var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
             mockUprawnien.Setup(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliDoUsuniecia")).ReturnsAsync(false);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
             jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

            // Act
            var wynik = await sut.UsunRole("IdRoliSystemowej", "adminId");

            // Assert
            Assert.True(wynik.Sukces);
             Assert.Equal("Rola usunięta pomyślnie.", wynik.Komunikat);
             mockUprawnien.Verify(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliDoUsuniecia"), Times.Once);
             mockRoleManager.Verify(z => z.FindByIdAsync("IdRoliDoUsuniecia"), Times.Once);
             mockRoleManager.Verify(z => z.DeleteAsync(It.Is<IdentityRole>(x => x.Name == "RolaDoSkasowania")), Times.Once);
         }
        //Rola usunieta błąd
        [Fact(Skip = "wymaga bazy w pamięci")]
        public async Task UsunRole_BladRoladoUsuniecia_ZwracaFalseIKomunikat()
         {
            // Arrange
            var jednostkaPracy = new Mock<IJednostkaPracy>();
            var pobierajRole = new Mock<IPobierajRole>();
            var pobierajUzytkownika = new Mock<IPobierajUzytkownika>();

            var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
             var mockRoleManager = new Mock<RoleManager<IdentityRole>>(mockRoleStore.Object, null, null, null, null);

             var rola = new IdentityRole { Id = "IdRoliDoUsuniecia", Name = "RolaDoSkasowania" };
             mockRoleManager.Setup(z => z.FindByIdAsync("IdRoliDoUsuniecia")).ReturnsAsync(rola);
             mockRoleManager.Setup(z => z.DeleteAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed());

             var mockUprawnien = new Mock<IPobierajUprawnieniaRoli>();
             mockUprawnien.Setup(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliDoUsuniecia")).ReturnsAsync(false);

            ZarzadzajRolamiService sut = new ZarzadzajRolamiService(mockRoleManager.Object, new DaneStartowe(), mockUprawnien.Object, null,
            jednostkaPracy.Object, pobierajRole.Object, pobierajUzytkownika.Object);

            // Act
            var wynik = await sut.UsunRole("IdRoliDoUsuniecia", "adminId");

             // Assert
             Assert.False(wynik.Sukces);
             Assert.Equal("Nie udało się usunąć roli.", wynik.Komunikat);
             mockUprawnien.Verify(z => z.CzyRolaJestUzywanaWHierarchiiAsync("IdRoliDoUsuniecia"), Times.Once);
             mockRoleManager.Verify(z => z.FindByIdAsync("IdRoliDoUsuniecia"), Times.Once);
             mockRoleManager.Verify(z => z.DeleteAsync(It.Is<IdentityRole>(x => x.Name == "RolaDoSkasowania")), Times.Once);
         }
    }
}
