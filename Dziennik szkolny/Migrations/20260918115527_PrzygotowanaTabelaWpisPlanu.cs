using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dziennik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class PrzygotowanaTabelaWpisPlanu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WpisyPlanu",
                columns: table => new
                {
                    IdWpisu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Lekcja = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Dzien = table.Column<int>(type: "int", nullable: false),
                    IdKlasy = table.Column<int>(type: "int", nullable: false),
                    IdPrzedmiotu = table.Column<int>(type: "int", nullable: false),
                    IdNauczyciela = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WpisyPlanu", x => x.IdWpisu);
                    table.ForeignKey(
                        name: "FK_WpisyPlanu_Klasa_IdKlasy",
                        column: x => x.IdKlasy,
                        principalTable: "Klasa",
                        principalColumn: "IdKlasy",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WpisyPlanu_PrzypisanePrzedmioty_IdPrzedmiotu_IdNauczyciela",
                        columns: x => new { x.IdPrzedmiotu, x.IdNauczyciela },
                        principalTable: "PrzypisanePrzedmioty",
                        principalColumns: new[] { "IdPrzedmiotu", "IdNauczyciela" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_WpisyPlanu_IdKlasy",
                table: "WpisyPlanu",
                column: "IdKlasy");

            migrationBuilder.CreateIndex(
                name: "IX_WpisyPlanu_IdPrzedmiotu_IdNauczyciela",
                table: "WpisyPlanu",
                columns: new[] { "IdPrzedmiotu", "IdNauczyciela" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WpisyPlanu");
        }
    }
}
