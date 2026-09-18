using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dziennik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class PrzygotowanaTabelaLaczeniePrzedmiotuZNauczycielem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrzypisanePrzedmioty",
                columns: table => new
                {
                    IdPrzedmiotu = table.Column<int>(type: "int", nullable: false),
                    IdNauczyciela = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrzypisanePrzedmioty", x => new { x.IdPrzedmiotu, x.IdNauczyciela });
                    table.ForeignKey(
                        name: "FK_PrzypisanePrzedmioty_AspNetUsers_IdNauczyciela",
                        column: x => x.IdNauczyciela,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrzypisanePrzedmioty_Przedmioty_IdPrzedmiotu",
                        column: x => x.IdPrzedmiotu,
                        principalTable: "Przedmioty",
                        principalColumn: "IdPrzedmiotu",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PrzypisanePrzedmioty_IdNauczyciela",
                table: "PrzypisanePrzedmioty",
                column: "IdNauczyciela");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrzypisanePrzedmioty");
        }
    }
}
