using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dzienik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class DodanokluczGlowny : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AspNetUsers_loginUzytkownikaId",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropIndex(
                name: "IX_InformacjeUzytkownik_loginUzytkownikaId",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropColumn(
                name: "loginUzytkownikaId",
                table: "InformacjeUzytkownik");

            migrationBuilder.AlterColumn<string>(
                name: "IdUzytkownika",
                table: "InformacjeUzytkownik",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_InformacjeUzytkownik_IdUzytkownika",
                table: "InformacjeUzytkownik",
                column: "IdUzytkownika");

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AspNetUsers_IdUzytkownika",
                table: "InformacjeUzytkownik",
                column: "IdUzytkownika",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformacjeUzytkownik_AspNetUsers_IdUzytkownika",
                table: "InformacjeUzytkownik");

            migrationBuilder.DropIndex(
                name: "IX_InformacjeUzytkownik_IdUzytkownika",
                table: "InformacjeUzytkownik");

            migrationBuilder.AlterColumn<long>(
                name: "IdUzytkownika",
                table: "InformacjeUzytkownik",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "loginUzytkownikaId",
                table: "InformacjeUzytkownik",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_InformacjeUzytkownik_loginUzytkownikaId",
                table: "InformacjeUzytkownik",
                column: "loginUzytkownikaId");

            migrationBuilder.AddForeignKey(
                name: "FK_InformacjeUzytkownik_AspNetUsers_loginUzytkownikaId",
                table: "InformacjeUzytkownik",
                column: "loginUzytkownikaId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
