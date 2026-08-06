using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dzienik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class Poprawabazykasowanierekurencyjnychtabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoczenieRoli");

            migrationBuilder.DropTable(
                name: "ListaRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                    LoginId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RolaIdRoli = table.Column<long>(type: "bigint", nullable: false),
                    IdRoli = table.Column<long>(type: "bigint", nullable: false),
                    IdUzytkownika = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoczenieRoli", x => x.IdLoczenia);
                    table.ForeignKey(
                        name: "FK_LoczenieRoli_AspNetUsers_LoginId",
                        column: x => x.LoginId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LoczenieRoli_ListaRole_RolaIdRoli",
                        column: x => x.RolaIdRoli,
                        principalTable: "ListaRole",
                        principalColumn: "IdRoli",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LoczenieRoli_LoginId",
                table: "LoczenieRoli",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_LoczenieRoli_RolaIdRoli",
                table: "LoczenieRoli",
                column: "RolaIdRoli");
        }
    }
}
