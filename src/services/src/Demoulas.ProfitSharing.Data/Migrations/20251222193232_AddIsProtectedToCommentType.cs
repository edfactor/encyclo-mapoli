using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsProtectedToCommentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DATEMODIFIED",
                table: "COMMENT_TYPE");

            migrationBuilder.DropColumn(
                name: "USERMODIFIED",
                table: "COMMENT_TYPE");

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
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: false,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MODIFIED_AT_UTC",
                table: "COMMENT_TYPE",
                type: "TIMESTAMP WITH TIME ZONE",
                nullable: true,
                defaultValueSql: "SYSTIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "USER_NAME",
                table: "COMMENT_TYPE",
                type: "NVARCHAR2(96)",
                maxLength: 96,
                nullable: true,
                defaultValueSql: "SYS_CONTEXT('USERENV', 'CLIENT_IDENTIFIER')");

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)1,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)2,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)3,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)4,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)5,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)6,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)7,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)8,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)9,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)10,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)11,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)12,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)13,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)14,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)15,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)16,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)17,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)18,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)19,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)20,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)21,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)22,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)23,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)24,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)25,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)26,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)27,
                columns: new string[0],
                values: new object[0]);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)28,
                columns: new string[0],
                values: new object[0]);
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

            migrationBuilder.AddColumn<string>(
                name: "DATEMODIFIED",
                table: "COMMENT_TYPE",
                type: "NVARCHAR2(10)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "USERMODIFIED",
                table: "COMMENT_TYPE",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)1,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)2,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)3,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)4,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)5,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)6,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)7,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)8,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)9,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)10,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)11,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)12,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)13,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)14,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)15,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)16,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)17,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)18,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)19,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)20,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)21,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)22,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)23,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)24,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)25,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)26,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)27,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "COMMENT_TYPE",
                keyColumn: "ID",
                keyValue: (byte)28,
                columns: new[] { "DATEMODIFIED", "USERMODIFIED" },
                values: new object[] { null, null });
        }
    }
}
