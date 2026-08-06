using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dzienik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class PoprawionoTabeleUzytkownicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_AdresIdAdresu",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AspNetUsers_LoginId",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropTable(
                name: "AdresUzytkownika");

            migrationBuilder.DropTable(
                name: "InformacjePracownik");

            migrationBuilder.DropIndex(
                name: "IX_InformacjeUzytkownik_AdresIdAdresu",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "AdresIdAdresu",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "IdAdresu",
                table: "InformacjeUzytkownik");

            migrationBuilder.RenameColumn(
                name: "LoginId",
                table: "InformacjeUzytkownik",
                newName: "loginUzytkownikaId");

            migrationBuilder.RenameColumn(
                name: "IdRodzica",
                table: "InformacjeUzytkownik",
                newName: "IdOsoby");

            migrationBuilder.RenameIndex(
                name: "IX_InformacjeUzytkownik_LoginId",
                table: "InformacjeUzytkownik",
                newName: "IX_InformacjeUzytkownik_loginUzytkownikaId");

            migrationBuilder.AddColumn<string>(
                name: "Miasto",
                table: "InformacjeUzytkownik",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NrMieszkania",
                table: "InformacjeUzytkownik",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Ulica",
                table: "InformacjeUzytkownik",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AspNetUsers_loginUzytkownikaId",
                table: "InformacjeUzytkownik",
                column: "loginUzytkownikaId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AspNetUsers_loginUzytkownikaId",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "Miasto",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "NrMieszkania",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "Ulica",
                table: "InformacjeUzytkownik");

            migrationBuilder.RenameColumn(
                name: "loginUzytkownikaId",
                table: "InformacjeUzytkownik",
                newName: "LoginId");

            migrationBuilder.RenameColumn(
                name: "IdOsoby",
                table: "InformacjeUzytkownik",
                newName: "IdRodzica");

            migrationBuilder.RenameIndex(
                name: "IX_InformacjeUzytkownik_loginUzytkownikaId",
                table: "InformacjeUzytkownik",
                newName: "IX_InformacjeUzytkownik_LoginId");

            migrationBuilder.AddColumn<long>(
                name: "AdresIdAdresu",
                table: "InformacjeUzytkownik",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "IdAdresu",
                table: "InformacjeUzytkownik",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "AdresUzytkownika",
                columns: table => new
                {
                    IdAdresu = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Adres = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUzytkownika = table.Column<long>(type: "bigint", nullable: false),
                    KodPocztowy = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Miejscowosc = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NrMieszkania = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdresUzytkownika", x => x.IdAdresu);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InformacjePracownik",
                columns: table => new
                {
                    IdPracownika = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LoginId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Imie = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nazwisko = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pesel = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformacjePracownik", x => x.IdPracownika);
                    table.ForeignKey(
                        name: "FK_InformacjePracownik_AspNetUsers_LoginId",
                        column: x => x.LoginId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_InformacjeUzytkownik_AdresIdAdresu",
                table: "InformacjeUzytkownik",
                column: "AdresIdAdresu");

            migrationBuilder.CreateIndex(
                name: "IX_InformacjePracownik_LoginId",
                table: "InformacjePracownik",
                column: "LoginId");

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_AdresIdAdresu",
                table: "InformacjeUzytkownik",
                column: "AdresIdAdresu",
                principalTable: "AdresUzytkownika",
                principalColumn: "IdAdresu",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AspNetUsers_LoginId",
                table: "InformacjeUzytkownik",
                column: "LoginId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
