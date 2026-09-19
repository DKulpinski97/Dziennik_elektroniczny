using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dziennik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class PrzygotowanieTabeliUczen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Uczniowie",
                columns: table => new
                {
                    IdUcznia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Pesel = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Imie = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nazwisko = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataUrodzenia = table.Column<DateOnly>(type: "date", nullable: false),
                    IdKlasy = table.Column<int>(type: "int", nullable: false),
                    IdOpiekun1 = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdOpiekun2 = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uczniowie", x => x.IdUcznia);
                    table.ForeignKey(
                        name: "FK_Uczniowie_AspNetUsers_IdOpiekun1",
                        column: x => x.IdOpiekun1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uczniowie_AspNetUsers_IdOpiekun2",
                        column: x => x.IdOpiekun2,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Uczniowie_Klasa_IdKlasy",
                        column: x => x.IdKlasy,
                        principalTable: "Klasa",
                        principalColumn: "IdKlasy",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Uczniowie_IdKlasy",
                table: "Uczniowie",
                column: "IdKlasy");

            migrationBuilder.CreateIndex(
                name: "IX_Uczniowie_IdOpiekun1",
                table: "Uczniowie",
                column: "IdOpiekun1");

            migrationBuilder.CreateIndex(
                name: "IX_Uczniowie_IdOpiekun2",
                table: "Uczniowie",
                column: "IdOpiekun2");

            migrationBuilder.CreateIndex(
                name: "IX_Uczniowie_Pesel",
                table: "Uczniowie",
                column: "Pesel",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Uczniowie");
        }
    }
}
