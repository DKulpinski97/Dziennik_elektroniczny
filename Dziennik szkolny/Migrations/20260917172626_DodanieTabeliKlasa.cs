using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dziennik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class DodanieTabeliKlasa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Klasa",
                columns: table => new
                {
                    IdKlasy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Oznaczenie = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RokNauki = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    RokRozpoczecia = table.Column<int>(type: "int", nullable: false),
                    RokZakonczenia = table.Column<int>(type: "int", nullable: true),
                    IdWychowawcy = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klasa", x => x.IdKlasy);
                    table.ForeignKey(
                        name: "FK_Klasa_AspNetUsers_IdWychowawcy",
                        column: x => x.IdWychowawcy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Klasa_IdWychowawcy",
                table: "Klasa",
                column: "IdWychowawcy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Klasa");
        }
    }
}
