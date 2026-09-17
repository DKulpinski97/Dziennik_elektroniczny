using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dzienik_szkolny.Migrations
{
    /// <inheritdoc />
    public partial class NaprawaAutoIncrementIdOsoby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migracja "PoprawionoTabeleUzytkownicy" zmieniła nazwę kolumny
            // IdRodzica -> IdOsoby wyłącznie przez RenameColumn. Ta operacja
            // nie niesie ze sobą informacji o AUTO_INCREMENT, więc w zależności
            // od wersji serwera MySQL/MariaDB (a konkretnie od tego, czy Pomelo
            // wygenerowało "RENAME COLUMN", czy musiało spaść do starszego
            // "CHANGE COLUMN" z pełną redefinicją typu) kolumna mogła stracić
            // atrybut AUTO_INCREMENT, mimo że model C# (encja InformacjeUzytkownik,
            // atrybut [DatabaseGenerated(DatabaseGeneratedOption.Identity)]) nadal
            // zakłada, że to kolumna tożsamościowa.
            //
            // Poniższy AlterColumn jawnie wymusza AUTO_INCREMENT na IdOsoby,
            // niezależnie od tego, co faktycznie wykonała poprzednia migracja.
            // IdOsoby jest już kluczem głównym tabeli (PK_InformacjeUzytkownik),
            // więc MySQL pozwoli na dodanie AUTO_INCREMENT bez dodatkowych zmian
            // indeksów.
            migrationBuilder.AlterColumn<long>(
                name: "IdOsoby",
                table: "InformacjeUzytkownik",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "IdOsoby",
                table: "InformacjeUzytkownik",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
