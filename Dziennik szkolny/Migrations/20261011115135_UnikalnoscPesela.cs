using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dziennik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class UnikalnoscPesela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InformacjeUzytkownik_Pesel",
                table: "InformacjeUzytkownik",
                column: "Pesel",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InformacjeUzytkownik_Pesel",
                table: "InformacjeUzytkownik");
        }
    }
}
