using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dzienik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class PoprawionoStruktureTabeli : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_Adresid",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "Rola",
                table: "InformacjeUzytkownik");

            migrationBuilder.RenameColumn(
                name: "Adresid",
                table: "InformacjeUzytkownik",
                newName: "AdresIdAdresu");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "InformacjeUzytkownik",
                newName: "IdRodzica");

            migrationBuilder.RenameIndex(
                name: "IX_InformacjeUzytkownik_Adresid",
                table: "InformacjeUzytkownik",
                newName: "IX_InformacjeUzytkownik_AdresIdAdresu");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AdresUzytkownika",
                newName: "IdAdresu");

            migrationBuilder.AlterColumn<string>(
                name: "ZaszyfrowaneHaslo",
                table: "LoginyUzytkowiow",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "InformacjeUzytkownik",
                type: "varchar(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InformacjePracownik",
                columns: table => new
                {
                    IdPracownika = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Imie = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nazwisko = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Pesel = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUzytkownika = table.Column<long>(type: "bigint", nullable: false),
                    LoginIdUzytkownika = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformacjePracownik", x => x.IdPracownika);
                    table.ForeignKey(
                        name: "FK_InformacjePracownik_LoginyUzytkowiow_LoginIdUzytkownika",
                        column: x => x.LoginIdUzytkownika,
                        principalTable: "LoginyUzytkowiow",
                        principalColumn: "IdUzytkownika",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ListaRole",
                columns: table => new
                {
                    IdRoli = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NazwaRoli = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaRole", x => x.IdRoli);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LoczenieRoli",
                columns: table => new
                {
                    IdLoczenia = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdUzytkownika = table.Column<long>(type: "bigint", nullable: false),
                    IdRoli = table.Column<long>(type: "bigint", nullable: false),
                    LoginIdUzytkownika = table.Column<long>(type: "bigint", nullable: false),
                    RolaIdRoli = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoczenieRoli", x => x.IdLoczenia);
                    table.ForeignKey(
                        name: "FK_LoczenieRoli_ListaRole_RolaIdRoli",
                        column: x => x.RolaIdRoli,
                        principalTable: "ListaRole",
                        principalColumn: "IdRoli",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoczenieRoli_LoginyUzytkowiow_LoginIdUzytkownika",
                        column: x => x.LoginIdUzytkownika,
                        principalTable: "LoginyUzytkowiow",
                        principalColumn: "IdUzytkownika",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_InformacjePracownik_LoginIdUzytkownika",
                table: "InformacjePracownik",
                column: "LoginIdUzytkownika");

            migrationBuilder.CreateIndex(
                name: "IX_LoczenieRoli_LoginIdUzytkownika",
                table: "LoczenieRoli",
                column: "LoginIdUzytkownika");

            migrationBuilder.CreateIndex(
                name: "IX_LoczenieRoli_RolaIdRoli",
                table: "LoczenieRoli",
                column: "RolaIdRoli");

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_AdresIdAdresu",
                table: "InformacjeUzytkownik",
                column: "AdresIdAdresu",
                principalTable: "AdresUzytkownika",
                principalColumn: "IdAdresu",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_AdresIdAdresu",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropTable(
                name: "InformacjePracownik");

            migrationBuilder.DropTable(
                name: "LoczenieRoli");

            migrationBuilder.DropTable(
                name: "ListaRole");

            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "InformacjeUzytkownik");

            migrationBuilder.RenameColumn(
                name: "AdresIdAdresu",
                table: "InformacjeUzytkownik",
                newName: "Adresid");

            migrationBuilder.RenameColumn(
                name: "IdRodzica",
                table: "InformacjeUzytkownik",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_InformacjeUzytkownik_AdresIdAdresu",
                table: "InformacjeUzytkownik",
                newName: "IX_InformacjeUzytkownik_Adresid");

            migrationBuilder.RenameColumn(
                name: "IdAdresu",
                table: "AdresUzytkownika",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "ZaszyfrowaneHaslo",
                table: "LoginyUzytkowiow",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Rola",
                table: "InformacjeUzytkownik",
                type: "varchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_Adresid",
                table: "InformacjeUzytkownik",
                column: "Adresid",
                principalTable: "AdresUzytkownika",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
