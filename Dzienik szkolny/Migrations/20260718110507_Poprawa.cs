using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dzienik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class Poprawa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_AdresUzytkownikaid",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropIndex(
                name: "IX_InformacjeUzytkownik_AdresUzytkownikaid",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "AdresUzytkownikaid",
                table: "InformacjeUzytkownik");

            migrationBuilder.AlterColumn<long>(
                name: "IdUzytkownika",
                table: "InformacjeUzytkownik",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "Adresid",
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

            migrationBuilder.CreateIndex(
                name: "IX_InformacjeUzytkownik_Adresid",
                table: "InformacjeUzytkownik",
                column: "Adresid");

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_Adresid",
                table: "InformacjeUzytkownik",
                column: "Adresid",
                principalTable: "AdresUzytkownika",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_Adresid",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropIndex(
                name: "IX_InformacjeUzytkownik_Adresid",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "Adresid",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "IdAdresu",
                table: "InformacjeUzytkownik");

            migrationBuilder.AlterColumn<int>(
                name: "IdUzytkownika",
                table: "InformacjeUzytkownik",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "AdresUzytkownikaid",
                table: "InformacjeUzytkownik",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InformacjeUzytkownik_AdresUzytkownikaid",
                table: "InformacjeUzytkownik",
                column: "AdresUzytkownikaid");

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AdresUzytkownika_AdresUzytkownikaid",
                table: "InformacjeUzytkownik",
                column: "AdresUzytkownikaid",
                principalTable: "AdresUzytkownika",
                principalColumn: "id");
        }
    }
}
