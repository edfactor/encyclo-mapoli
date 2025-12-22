using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentTypeTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)",
                oldDefaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CREATED_AT_UTC",
                table: "COMMENT_TYPE",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "COMMENT_TYPE",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "COMMENT_TYPE",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)1,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)2,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)3,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)4,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)5,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)6,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)7,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)8,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)9,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)10,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)11,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)12,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)13,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)14,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)15,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)16,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)17,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)18,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)19,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)20,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)21,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)22,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)23,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)24,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)25,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)26,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)27,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)28,
                columns: new[] { "MODIFIED_AT_UTC", "USER_NAME" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CREATED_AT_UTC",
                table: "COMMENT_TYPE");

            migrationBuilder.DropColumn(
                name: "MODIFIED_AT_UTC",
                table: "COMMENT_TYPE");

            migrationBuilder.DropColumn(
                name: "USER_NAME",
                table: "COMMENT_TYPE");

            migrationBuilder.AlterColumn<byte>(
                name: "YEARS_OF_SERVICE_CREDIT",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(short),
                oldType: "NUMBER(3)",
                oldDefaultValue: (short)0);
        }
    }
}
