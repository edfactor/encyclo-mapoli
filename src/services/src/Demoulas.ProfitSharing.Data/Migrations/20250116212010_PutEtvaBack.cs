using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class PutEtvaBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ETVA",
                table: "PAY_PROFIT",
                type: "DECIMAL(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990306,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990313,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990320,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990327,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990403,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990410,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990417,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990424,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990501,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990508,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990515,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990522,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990529,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990605,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990612,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990619,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990626,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990703,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990710,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990717,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990724,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990731,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990807,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990814,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990821,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990828,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990904,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990911,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990918,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990925,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991002,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991009,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991016,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991023,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991030,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000304,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000311,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000318,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000325,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000401,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000408,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000415,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000422,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000429,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000506,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000513,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000520,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000527,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000603,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000610,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000617,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000624,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000701,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000708,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000715,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000722,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000729,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000805,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000812,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000819,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000826,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000902,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000909,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000916,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000923,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000930,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001007,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001014,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001021,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001028,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001230,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010303,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010310,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010317,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010324,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010331,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010407,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010414,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010421,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010428,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010505,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010512,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010519,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010526,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010602,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010609,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010616,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010623,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010630,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010707,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010714,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010721,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010728,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010804,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010811,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010818,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010825,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010901,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010908,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010915,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010922,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010929,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011006,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011013,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011020,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011027,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020302,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020309,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020316,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020323,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020330,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020406,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020413,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020420,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020427,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020504,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020511,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020518,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020525,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020601,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020608,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020615,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020622,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020629,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020706,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020713,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020720,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020727,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020803,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020810,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020817,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020824,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020831,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020907,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020914,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020921,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020928,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021005,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021012,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021019,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021026,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030301,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030308,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030315,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030322,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030329,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030405,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030412,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030419,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030426,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030503,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030510,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030517,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030524,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030531,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030607,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030614,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030621,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030628,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030705,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030712,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030719,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030726,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030802,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030809,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030816,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030823,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030830,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030906,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030913,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030920,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030927,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031004,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031011,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031018,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031025,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)53 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040131,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040306,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040313,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040320,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040327,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040403,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040410,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040417,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040424,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040501,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040508,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040515,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040522,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040529,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040605,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040612,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040619,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040626,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040703,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040710,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040717,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040724,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040731,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040807,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040814,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040821,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040828,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040904,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040911,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040918,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040925,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041002,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041009,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041016,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041023,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041030,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050305,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050312,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050319,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050326,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050402,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050409,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050416,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050423,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050430,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050507,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050514,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050521,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050528,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050604,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050611,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050618,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050625,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050702,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050709,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050716,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050723,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050730,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050806,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050813,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050820,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050827,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050903,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050910,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050917,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050924,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051001,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051008,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051015,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051022,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051029,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051231,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060304,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060311,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060318,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060325,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060401,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060408,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060415,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060422,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060429,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060506,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060513,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060520,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060527,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060603,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060610,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060617,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060624,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060701,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060708,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060715,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060722,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060729,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060805,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060812,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060819,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060826,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060902,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060909,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060916,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060923,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060930,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061007,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061014,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061021,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061028,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061230,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070303,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070310,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070317,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070324,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070331,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070407,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070414,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070421,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070428,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070505,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070512,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070519,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070526,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070602,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070609,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070616,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070623,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070630,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070707,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070714,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070721,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070728,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070804,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070811,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070818,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070825,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070901,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070908,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070915,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070922,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070929,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071006,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071013,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071020,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071027,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080301,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080308,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080315,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080322,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080329,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080405,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080412,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080419,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080426,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080503,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080510,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080517,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080524,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080531,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080607,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080614,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080621,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080628,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080705,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080712,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080719,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080726,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080802,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080809,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080816,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080823,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080830,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080906,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080913,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080920,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080927,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081004,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081011,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081018,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081025,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)53 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090131,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090307,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090314,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090321,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090328,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090404,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090411,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090418,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090425,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090502,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090509,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090516,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090523,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090530,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090606,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090613,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090620,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090627,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090704,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090711,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090718,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090725,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090801,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090808,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090815,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090822,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090829,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090905,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090912,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090919,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090926,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091003,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091010,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091017,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091024,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091031,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100306,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100313,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100320,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100327,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100403,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100410,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100417,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100424,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100501,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100508,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100515,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100522,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100529,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100605,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100612,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100619,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100626,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100703,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100710,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100717,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100724,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100731,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100807,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100814,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100821,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100828,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100904,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100911,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100918,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100925,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101002,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101009,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101016,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101023,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101030,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110305,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110312,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110319,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110326,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110402,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110409,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110416,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110423,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110430,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110507,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110514,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110521,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110528,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110604,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110611,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110618,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110625,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110702,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110709,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110716,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110723,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110730,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110806,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110813,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110820,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110827,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110903,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110910,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110917,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110924,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111001,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111008,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111015,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111022,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111029,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111231,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120303,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120310,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120317,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120324,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120331,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120407,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120414,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120421,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120428,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120505,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120512,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120519,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120526,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120602,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120609,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120616,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120623,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120630,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120707,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120714,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120721,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120728,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120804,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120811,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120818,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120825,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120901,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120908,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120915,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120922,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120929,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121006,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121013,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121020,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121027,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130302,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130309,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130316,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130323,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130330,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130406,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130413,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130420,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130427,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130504,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130511,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130518,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130525,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130601,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130608,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130615,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130622,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130629,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130706,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130713,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130720,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130727,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130803,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130810,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130817,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130824,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130831,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130907,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130914,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130921,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130928,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131005,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131012,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131019,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131026,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140301,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140308,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140315,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140322,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140329,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140405,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140412,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140419,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140426,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140503,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140510,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140517,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140524,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140531,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140607,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140614,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140621,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140628,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140705,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140712,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140719,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140726,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140802,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140809,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140816,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140823,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140830,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140906,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140913,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140920,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140927,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141004,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141011,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141018,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141025,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)53 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150131,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150307,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150314,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150321,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150328,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150404,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150411,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150418,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150425,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150502,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150509,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150516,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150523,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150530,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150606,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150613,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150620,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150627,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150704,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150711,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150718,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150725,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150801,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150808,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150815,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150822,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150829,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150905,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150912,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150919,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150926,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151003,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151010,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151017,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151024,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151031,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160305,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160312,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160319,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160326,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160402,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160409,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160416,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160423,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160430,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160507,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160514,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160521,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160528,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160604,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160611,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160618,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160625,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160702,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160709,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160716,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160723,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160730,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160806,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160813,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160820,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160827,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160903,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160910,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160917,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160924,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161001,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161008,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161015,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161022,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161029,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161231,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170304,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170311,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170318,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170325,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170401,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170408,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170415,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170422,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170429,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170506,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170513,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170520,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170527,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170603,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170610,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170617,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170624,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170701,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170708,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170715,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170722,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170729,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170805,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170812,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170819,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170826,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170902,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170909,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170916,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170923,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170930,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171007,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171014,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171021,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171028,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171230,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180303,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180310,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180317,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180324,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180331,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180407,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180414,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180421,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180428,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180505,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180512,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180519,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180526,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180602,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180609,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180616,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180623,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180630,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180707,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180714,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180721,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180728,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180804,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180811,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180818,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180825,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180901,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180908,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180915,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180922,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180929,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181006,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181013,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181020,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181027,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190302,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190309,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190316,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190323,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190330,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190406,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190413,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190420,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190427,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190504,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190511,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190518,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190525,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190601,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190608,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190615,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190622,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190629,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190706,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190713,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190720,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190727,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190803,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190810,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190817,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190824,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190831,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190907,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190914,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190921,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190928,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191005,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191012,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191019,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191026,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200307,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200314,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200321,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200328,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200404,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200411,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200418,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200425,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200502,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200509,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200516,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200523,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200530,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200606,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200613,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200620,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200627,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200704,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200711,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200718,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200725,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200801,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200808,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200815,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200822,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200829,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200905,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200912,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200919,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200926,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201003,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201010,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201017,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201024,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201031,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)53 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210306,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210313,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210320,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210327,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210403,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210410,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210417,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210424,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210501,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210508,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210515,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210522,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210529,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210605,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210612,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210619,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210626,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210703,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210710,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210717,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210724,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210731,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210807,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210814,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210821,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210828,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210904,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210911,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210918,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210925,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211002,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211009,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211016,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211023,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211030,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220305,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220312,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220319,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220326,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220402,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220409,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220416,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220423,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220430,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220507,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220514,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220521,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220528,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220604,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220611,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220618,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220625,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220702,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220709,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220716,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220723,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220730,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220806,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220813,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220820,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220827,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220903,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220910,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220917,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220924,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221001,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221008,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221015,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221022,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221029,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221231,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230304,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230311,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230318,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230325,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230401,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230408,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230415,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230422,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230429,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230506,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230513,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230520,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230527,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230603,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230610,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230617,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230624,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230701,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230708,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230715,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230722,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230729,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230805,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230812,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230819,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230826,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230902,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230909,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230916,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230923,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230930,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231007,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231014,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231021,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231028,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231230,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240302,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240309,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240316,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240323,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240330,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240406,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240413,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240420,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240427,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240504,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240511,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240518,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240525,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240601,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240608,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240615,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240622,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240629,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240706,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240713,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240720,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240727,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240803,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240810,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240817,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240824,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240831,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240907,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240914,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240921,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240928,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241005,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241012,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241019,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241026,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)1, (short)1, (short)1, (short)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250301,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)2, (short)2, (short)1, (short)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250308,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250315,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250322,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)1, (short)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250329,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)3, (short)3, (short)2, (short)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250405,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250412,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250419,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250426,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250503,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)4, (short)4, (short)2, (short)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250510,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250517,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250524,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250531,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)5, (short)5, (short)2, (short)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250607,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250614,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250621,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)2, (short)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250628,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)6, (short)6, (short)3, (short)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250705,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250712,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250719,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250726,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250802,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)7, (short)7, (short)3, (short)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250809,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250816,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250823,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250830,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)8, (short)8, (short)3, (short)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250906,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250913,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250920,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)3, (short)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250927,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)9, (short)9, (short)4, (short)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251004,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251011,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251018,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251025,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)10, (short)10, (short)4, (short)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)11, (short)11, (short)4, (short)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)4, (short)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (short)12, (short)12, (short)1, (short)52 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETVA",
                table: "PAY_PROFIT");

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990306,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990313,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990320,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990327,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990403,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990410,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990417,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990424,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990501,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990508,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990515,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990522,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990529,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990605,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990612,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990619,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990626,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990703,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990710,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990717,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990724,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990731,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990807,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990814,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990821,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990828,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990904,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990911,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990918,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19990925,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991002,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991009,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991016,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991023,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991030,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 19991225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000304,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000311,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000318,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000325,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000401,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000408,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000415,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000422,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000429,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000506,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000513,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000520,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000527,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000603,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000610,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000617,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000624,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000701,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000708,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000715,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000722,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000729,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000805,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000812,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000819,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000826,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000902,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000909,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000916,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000923,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20000930,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001007,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001014,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001021,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001028,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20001230,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010303,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010310,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010317,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010324,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010331,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010407,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010414,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010421,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010428,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010505,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010512,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010519,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010526,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010602,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010609,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010616,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010623,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010630,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010707,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010714,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010721,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010728,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010804,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010811,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010818,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010825,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010901,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010908,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010915,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010922,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20010929,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011006,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011013,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011020,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011027,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20011229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020302,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020309,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020316,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020323,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020330,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020406,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020413,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020420,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020427,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020504,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020511,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020518,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020525,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020601,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020608,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020615,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020622,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020629,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020706,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020713,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020720,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020727,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020803,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020810,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020817,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020824,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020831,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020907,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020914,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020921,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20020928,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021005,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021012,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021019,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021026,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20021228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030301,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030308,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030315,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030322,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030329,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030405,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030412,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030419,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030426,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030503,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030510,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030517,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030524,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030531,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030607,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030614,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030621,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030628,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030705,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030712,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030719,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030726,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030802,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030809,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030816,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030823,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030830,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030906,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030913,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030920,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20030927,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031004,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031011,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031018,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031025,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20031227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)53 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040131,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040306,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040313,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040320,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040327,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040403,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040410,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040417,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040424,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040501,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040508,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040515,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040522,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040529,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040605,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040612,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040619,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040626,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040703,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040710,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040717,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040724,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040731,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040807,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040814,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040821,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040828,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040904,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040911,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040918,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20040925,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041002,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041009,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041016,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041023,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041030,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20041225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050305,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050312,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050319,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050326,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050402,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050409,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050416,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050423,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050430,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050507,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050514,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050521,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050528,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050604,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050611,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050618,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050625,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050702,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050709,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050716,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050723,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050730,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050806,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050813,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050820,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050827,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050903,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050910,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050917,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20050924,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051001,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051008,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051015,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051022,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051029,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20051231,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060304,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060311,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060318,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060325,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060401,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060408,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060415,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060422,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060429,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060506,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060513,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060520,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060527,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060603,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060610,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060617,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060624,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060701,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060708,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060715,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060722,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060729,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060805,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060812,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060819,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060826,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060902,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060909,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060916,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060923,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20060930,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061007,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061014,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061021,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061028,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20061230,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070303,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070310,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070317,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070324,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070331,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070407,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070414,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070421,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070428,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070505,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070512,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070519,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070526,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070602,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070609,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070616,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070623,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070630,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070707,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070714,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070721,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070728,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070804,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070811,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070818,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070825,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070901,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070908,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070915,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070922,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20070929,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071006,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071013,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071020,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071027,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20071229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080301,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080308,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080315,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080322,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080329,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080405,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080412,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080419,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080426,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080503,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080510,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080517,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080524,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080531,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080607,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080614,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080621,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080628,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080705,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080712,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080719,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080726,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080802,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080809,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080816,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080823,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080830,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080906,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080913,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080920,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20080927,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081004,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081011,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081018,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081025,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20081227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)53 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090131,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090307,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090314,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090321,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090328,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090404,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090411,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090418,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090425,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090502,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090509,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090516,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090523,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090530,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090606,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090613,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090620,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090627,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090704,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090711,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090718,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090725,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090801,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090808,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090815,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090822,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090829,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090905,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090912,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090919,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20090926,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091003,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091010,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091017,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091024,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091031,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20091226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100306,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100313,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100320,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100327,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100403,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100410,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100417,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100424,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100501,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100508,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100515,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100522,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100529,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100605,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100612,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100619,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100626,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100703,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100710,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100717,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100724,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100731,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100807,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100814,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100821,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100828,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100904,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100911,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100918,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20100925,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101002,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101009,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101016,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101023,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101030,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20101225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110305,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110312,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110319,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110326,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110402,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110409,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110416,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110423,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110430,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110507,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110514,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110521,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110528,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110604,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110611,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110618,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110625,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110702,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110709,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110716,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110723,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110730,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110806,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110813,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110820,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110827,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110903,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110910,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110917,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20110924,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111001,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111008,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111015,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111022,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111029,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20111231,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120303,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120310,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120317,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120324,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120331,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120407,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120414,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120421,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120428,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120505,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120512,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120519,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120526,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120602,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120609,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120616,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120623,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120630,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120707,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120714,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120721,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120728,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120804,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120811,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120818,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120825,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120901,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120908,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120915,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120922,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20120929,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121006,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121013,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121020,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121027,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20121229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130302,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130309,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130316,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130323,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130330,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130406,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130413,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130420,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130427,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130504,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130511,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130518,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130525,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130601,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130608,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130615,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130622,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130629,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130706,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130713,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130720,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130727,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130803,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130810,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130817,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130824,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130831,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130907,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130914,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130921,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20130928,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131005,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131012,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131019,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131026,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20131228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140301,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140308,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140315,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140322,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140329,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140405,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140412,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140419,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140426,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140503,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140510,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140517,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140524,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140531,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140607,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140614,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140621,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140628,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140705,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140712,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140719,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140726,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140802,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140809,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140816,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140823,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140830,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140906,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140913,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140920,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20140927,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141004,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141011,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141018,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141025,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20141227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)53 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150131,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150307,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150314,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150321,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150328,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150404,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150411,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150418,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150425,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150502,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150509,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150516,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150523,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150530,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150606,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150613,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150620,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150627,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150704,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150711,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150718,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150725,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150801,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150808,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150815,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150822,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150829,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150905,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150912,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150919,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20150926,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151003,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151010,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151017,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151024,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151031,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20151226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160305,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160312,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160319,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160326,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160402,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160409,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160416,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160423,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160430,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160507,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160514,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160521,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160528,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160604,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160611,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160618,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160625,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160702,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160709,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160716,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160723,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160730,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160806,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160813,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160820,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160827,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160903,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160910,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160917,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20160924,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161001,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161008,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161015,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161022,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161029,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20161231,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170304,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170311,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170318,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170325,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170401,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170408,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170415,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170422,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170429,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170506,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170513,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170520,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170527,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170603,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170610,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170617,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170624,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170701,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170708,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170715,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170722,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170729,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170805,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170812,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170819,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170826,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170902,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170909,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170916,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170923,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20170930,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171007,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171014,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171021,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171028,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20171230,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180303,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180310,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180317,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180324,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180331,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180407,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180414,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180421,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180428,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180505,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180512,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180519,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180526,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180602,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180609,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180616,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180623,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180630,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180707,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180714,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180721,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180728,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180804,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180811,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180818,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180825,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180901,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180908,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180915,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180922,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20180929,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181006,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181013,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181020,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181027,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181103,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181110,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181117,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181124,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20181229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190302,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190309,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190316,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190323,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190330,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190406,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190413,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190420,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190427,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190504,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190511,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190518,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190525,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190601,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190608,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190615,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190622,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190629,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190706,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190713,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190720,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190727,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190803,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190810,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190817,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190824,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190831,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190907,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190914,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190921,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20190928,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191005,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191012,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191019,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191026,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20191228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200229,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200307,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200314,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200321,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200328,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200404,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200411,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200418,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200425,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200502,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200509,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200516,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200523,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200530,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200606,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200613,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200620,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200627,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200704,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200711,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200718,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200725,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200801,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200808,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200815,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200822,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200829,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200905,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200912,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200919,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20200926,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201003,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201010,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201017,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201024,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201031,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20201226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)53 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210306,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210313,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210320,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210327,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210403,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210410,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210417,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210424,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210501,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210508,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210515,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210522,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210529,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210605,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210612,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210619,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210626,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210703,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210710,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210717,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210724,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210731,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210807,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210814,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210821,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210828,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210904,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210911,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210918,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20210925,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211002,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211009,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211016,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211023,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211030,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20211225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220205,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220212,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220219,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220226,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220305,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220312,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220319,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220326,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220402,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220409,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220416,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220423,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220430,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220507,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220514,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220521,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220528,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220604,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220611,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220618,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220625,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220702,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220709,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220716,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220723,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220730,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220806,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220813,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220820,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220827,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220903,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220910,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220917,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20220924,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221001,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221008,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221015,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221022,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221029,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221105,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221112,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221119,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221126,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20221231,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230107,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230114,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230121,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230128,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230204,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230211,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230218,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230225,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230304,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230311,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230318,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230325,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230401,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230408,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230415,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230422,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230429,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230506,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230513,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230520,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230527,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230603,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230610,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230617,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230624,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230701,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230708,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230715,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230722,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230729,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230805,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230812,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230819,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230826,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230902,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230909,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230916,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230923,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20230930,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231007,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231014,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231021,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231028,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231202,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231209,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231216,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231223,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20231230,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240106,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240113,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240120,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240127,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240203,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240210,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240217,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240224,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240302,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240309,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240316,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240323,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240330,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240406,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240413,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240420,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240427,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240504,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240511,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240518,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240525,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240601,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240608,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240615,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240622,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240629,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240706,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240713,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240720,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240727,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240803,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240810,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240817,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240824,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240831,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240907,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240914,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240921,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20240928,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241005,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241012,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241019,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241026,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241102,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241109,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241116,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241123,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241130,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241207,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241214,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241221,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20241228,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250104,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)1 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250111,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)2 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250118,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250125,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)4 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250201,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)1, (byte)1, (byte)1, (byte)5 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250208,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250215,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)7 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250222,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250301,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)2, (byte)2, (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250308,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250315,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)11 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250322,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)1, (byte)12 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250329,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)3, (byte)3, (byte)2, (byte)13 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250405,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)14 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250412,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)15 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250419,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)16 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250426,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)17 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250503,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)4, (byte)4, (byte)2, (byte)18 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250510,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)19 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250517,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)20 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250524,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)21 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250531,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)5, (byte)5, (byte)2, (byte)22 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250607,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)23 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250614,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)24 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250621,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)2, (byte)25 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250628,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)6, (byte)6, (byte)3, (byte)26 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250705,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)27 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250712,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)28 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250719,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)29 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250726,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)30 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250802,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)7, (byte)7, (byte)3, (byte)31 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250809,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)32 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250816,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)33 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250823,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)34 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250830,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)8, (byte)8, (byte)3, (byte)35 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250906,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)36 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250913,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)37 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250920,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)3, (byte)38 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20250927,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)9, (byte)9, (byte)4, (byte)39 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251004,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)40 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251011,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)41 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251018,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)42 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251025,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)43 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251101,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)10, (byte)10, (byte)4, (byte)44 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251108,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)45 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251115,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)46 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251122,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)47 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251129,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)11, (byte)11, (byte)4, (byte)48 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251206,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)49 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251213,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)50 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251220,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)4, (byte)51 });

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND2_N",
                keyValue: 20251227,
                columns: new[] { "ACC_CALPERIOD", "ACC_PERIOD", "ACC_QUARTER", "ACC_WEEKN" },
                values: new object[] { (byte)12, (byte)12, (byte)1, (byte)52 });
        }
    }
}
