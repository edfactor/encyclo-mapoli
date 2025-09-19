using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class PayClassificationIdString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Non-destructive conversion of PAY_CLASSIFICATION.ID & dependent FK columns from NUMBER(2) -> NVARCHAR2(4)

            // 1. Drop FKs so we can manipulate columns
            migrationBuilder.DropForeignKey(
                name: "FK_DEMOGRAPHIC_PAY_CLASSIFICATION_PAY_CLASSIFICATION_ID",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropForeignKey(
                name: "FK_DEMOGRAPHIC_HISTORY_PAY_CLASSIFICATION_PAY_CLASSIFICATION_ID",
                table: "DEMOGRAPHIC_HISTORY");

            // 2. Drop PK to allow replacing the ID column
            migrationBuilder.DropPrimaryKey(
                name: "PK_PAY_CLASSIFICATION",
                table: "PAY_CLASSIFICATION");

            // 3. Add new string columns
            migrationBuilder.AddColumn<string>(
                name: "ID_NEW",
                table: "PAY_CLASSIFICATION",
                type: "NVARCHAR2(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PAY_CLASSIFICATION_ID_NEW",
                table: "DEMOGRAPHIC",
                type: "NVARCHAR2(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PAY_CLASSIFICATION_ID_NEW",
                table: "DEMOGRAPHIC_HISTORY",
                type: "NVARCHAR2(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            // 4. Copy data (numeric -> string)
            migrationBuilder.Sql("UPDATE PAY_CLASSIFICATION SET ID_NEW = TO_CHAR(ID)");
            migrationBuilder.Sql("UPDATE DEMOGRAPHIC SET PAY_CLASSIFICATION_ID_NEW = TO_CHAR(PAY_CLASSIFICATION_ID)");
            migrationBuilder.Sql("UPDATE DEMOGRAPHIC_HISTORY SET PAY_CLASSIFICATION_ID_NEW = TO_CHAR(PAY_CLASSIFICATION_ID)");

            // 5. Drop old numeric columns
            migrationBuilder.DropColumn(
                name: "ID",
                table: "PAY_CLASSIFICATION");

            migrationBuilder.DropColumn(
                name: "PAY_CLASSIFICATION_ID",
                table: "DEMOGRAPHIC");

            migrationBuilder.DropColumn(
                name: "PAY_CLASSIFICATION_ID",
                table: "DEMOGRAPHIC_HISTORY");

            // 6. Rename new columns to original names
            migrationBuilder.RenameColumn(
                name: "ID_NEW",
                table: "PAY_CLASSIFICATION",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "PAY_CLASSIFICATION_ID_NEW",
                table: "DEMOGRAPHIC",
                newName: "PAY_CLASSIFICATION_ID");

            migrationBuilder.RenameColumn(
                name: "PAY_CLASSIFICATION_ID_NEW",
                table: "DEMOGRAPHIC_HISTORY",
                newName: "PAY_CLASSIFICATION_ID");

            // 7. Recreate PK
            migrationBuilder.AddPrimaryKey(
                name: "PK_PAY_CLASSIFICATION",
                table: "PAY_CLASSIFICATION",
                column: "ID");

            // 8. Recreate FKs
            migrationBuilder.AddForeignKey(
                name: "FK_DEMOGRAPHIC_PAY_CLASSIFICATION_PAY_CLASSIFICATION_ID",
                table: "DEMOGRAPHIC",
                column: "PAY_CLASSIFICATION_ID",
                principalTable: "PAY_CLASSIFICATION",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DEMOGRAPHIC_HISTORY_PAY_CLASSIFICATION_PAY_CLASSIFICATION_ID",
                table: "DEMOGRAPHIC_HISTORY",
                column: "PAY_CLASSIFICATION_ID",
                principalTable: "PAY_CLASSIFICATION",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Down migration (string -> byte) not implemented due to potential data loss & production forward-only strategy.
            throw new NotSupportedException("Reverting PayClassificationIdString migration is not supported (would be destructive).");
        }
    }
}
