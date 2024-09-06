using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class profitCodeIsLookupTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PROFIT_DETAIL_PROFITCODES_PROFIT_CODE_ID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PROFIT_CODE",
                table: "PROFIT_CODE");

            migrationBuilder.DropIndex(
                name: "CALDAR_RECORD_ACC_APWKEND_N",
                table: "CALDAR_RECORD");

            migrationBuilder.DropIndex(
                name: "CALDAR_RECORD_ACC_WEDATE2",
                table: "CALDAR_RECORD");

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "CODE",
                keyColumnType: "NUMBER(5)",
                keyValue: (short)0);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "CODE",
                keyColumnType: "NUMBER(5)",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "CODE",
                keyColumnType: "NUMBER(5)",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "CODE",
                keyColumnType: "NUMBER(5)",
                keyValue: (short)3);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "CODE",
                keyColumnType: "NUMBER(5)",
                keyValue: (short)5);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "CODE",
                keyColumnType: "NUMBER(5)",
                keyValue: (short)6);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "CODE",
                keyColumnType: "NUMBER(5)",
                keyValue: (short)8);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "CODE",
                keyColumnType: "NUMBER(5)",
                keyValue: (short)9);

            migrationBuilder.DropColumn(
                name: "CODE",
                table: "PROFIT_CODE");

            migrationBuilder.RenameIndex(
                name: "IX_PROFIT_DETAIL_PROFIT_CODE_ID",
                table: "PROFIT_DETAIL",
                newName: "IX_PROFIT_DETAIL_PROFITCODEID");

            migrationBuilder.RenameColumn(
                name: "DEFINITION",
                table: "PROFIT_CODE",
                newName: "NAME");

            migrationBuilder.AlterColumn<byte>(
                name: "PROFIT_CODE_ID",
                table: "PROFIT_DETAIL",
                type: "NUMBER(3)",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "NUMBER(5)");

            migrationBuilder.AddColumn<byte>(
                name: "ID",
                table: "PROFIT_CODE",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<int>(
                name: "ACC_WKEND2_N",
                table: "CALDAR_RECORD",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "NUMBER(19)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ACC_APWKEND",
                table: "CALDAR_RECORD",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ACC_WKEND_N",
                table: "CALDAR_RECORD",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)")
                .OldAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PROFIT_CODE",
                table: "PROFIT_CODE",
                column: "ID");

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101,
                column: "ACC_WKEND2_N",
                value: 20000101);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 108,
                column: "ACC_WKEND2_N",
                value: 20000108);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 115,
                column: "ACC_WKEND2_N",
                value: 20000115);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 122,
                column: "ACC_WKEND2_N",
                value: 20000122);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 129,
                column: "ACC_WKEND2_N",
                value: 20000129);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 205,
                column: "ACC_WKEND2_N",
                value: 20000205);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 212,
                column: "ACC_WKEND2_N",
                value: 20000212);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 219,
                column: "ACC_WKEND2_N",
                value: 20000219);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 226,
                column: "ACC_WKEND2_N",
                value: 20000226);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 304,
                column: "ACC_WKEND2_N",
                value: 20000304);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 311,
                column: "ACC_WKEND2_N",
                value: 20000311);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 318,
                column: "ACC_WKEND2_N",
                value: 20000318);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 325,
                column: "ACC_WKEND2_N",
                value: 20000325);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 401,
                column: "ACC_WKEND2_N",
                value: 20000401);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 408,
                column: "ACC_WKEND2_N",
                value: 20000408);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 415,
                column: "ACC_WKEND2_N",
                value: 20000415);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 422,
                column: "ACC_WKEND2_N",
                value: 20000422);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 429,
                column: "ACC_WKEND2_N",
                value: 20000429);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 506,
                column: "ACC_WKEND2_N",
                value: 20000506);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 513,
                column: "ACC_WKEND2_N",
                value: 20000513);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 520,
                column: "ACC_WKEND2_N",
                value: 20000520);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 527,
                column: "ACC_WKEND2_N",
                value: 20000527);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 603,
                column: "ACC_WKEND2_N",
                value: 20000603);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 610,
                column: "ACC_WKEND2_N",
                value: 20000610);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 617,
                column: "ACC_WKEND2_N",
                value: 20000617);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 624,
                column: "ACC_WKEND2_N",
                value: 20000624);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 701,
                column: "ACC_WKEND2_N",
                value: 20000701);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 708,
                column: "ACC_WKEND2_N",
                value: 20000708);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 715,
                column: "ACC_WKEND2_N",
                value: 20000715);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 722,
                column: "ACC_WKEND2_N",
                value: 20000722);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 729,
                column: "ACC_WKEND2_N",
                value: 20000729);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 805,
                column: "ACC_WKEND2_N",
                value: 20000805);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 812,
                column: "ACC_WKEND2_N",
                value: 20000812);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 819,
                column: "ACC_WKEND2_N",
                value: 20000819);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 826,
                column: "ACC_WKEND2_N",
                value: 20000826);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 902,
                column: "ACC_WKEND2_N",
                value: 20000902);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 909,
                column: "ACC_WKEND2_N",
                value: 20000909);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 916,
                column: "ACC_WKEND2_N",
                value: 20000916);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 923,
                column: "ACC_WKEND2_N",
                value: 20000923);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 930,
                column: "ACC_WKEND2_N",
                value: 20000930);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1007,
                column: "ACC_WKEND2_N",
                value: 20001007);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1014,
                column: "ACC_WKEND2_N",
                value: 20001014);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1021,
                column: "ACC_WKEND2_N",
                value: 20001021);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1028,
                column: "ACC_WKEND2_N",
                value: 20001028);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1104,
                column: "ACC_WKEND2_N",
                value: 20001104);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1111,
                column: "ACC_WKEND2_N",
                value: 20001111);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1118,
                column: "ACC_WKEND2_N",
                value: 20001118);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1125,
                column: "ACC_WKEND2_N",
                value: 20001125);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1202,
                column: "ACC_WKEND2_N",
                value: 20001202);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1209,
                column: "ACC_WKEND2_N",
                value: 20001209);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1216,
                column: "ACC_WKEND2_N",
                value: 20001216);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1223,
                column: "ACC_WKEND2_N",
                value: 20001223);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1230,
                column: "ACC_WKEND2_N",
                value: 20001230);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10106,
                column: "ACC_WKEND2_N",
                value: 20010106);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10113,
                column: "ACC_WKEND2_N",
                value: 20010113);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10120,
                column: "ACC_WKEND2_N",
                value: 20010120);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10127,
                column: "ACC_WKEND2_N",
                value: 20010127);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10203,
                column: "ACC_WKEND2_N",
                value: 20010203);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10210,
                column: "ACC_WKEND2_N",
                value: 20010210);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10217,
                column: "ACC_WKEND2_N",
                value: 20010217);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10224,
                column: "ACC_WKEND2_N",
                value: 20010224);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10303,
                column: "ACC_WKEND2_N",
                value: 20010303);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10310,
                column: "ACC_WKEND2_N",
                value: 20010310);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10317,
                column: "ACC_WKEND2_N",
                value: 20010317);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10324,
                column: "ACC_WKEND2_N",
                value: 20010324);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10331,
                column: "ACC_WKEND2_N",
                value: 20010331);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10407,
                column: "ACC_WKEND2_N",
                value: 20010407);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10414,
                column: "ACC_WKEND2_N",
                value: 20010414);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10421,
                column: "ACC_WKEND2_N",
                value: 20010421);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10428,
                column: "ACC_WKEND2_N",
                value: 20010428);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10505,
                column: "ACC_WKEND2_N",
                value: 20010505);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10512,
                column: "ACC_WKEND2_N",
                value: 20010512);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10519,
                column: "ACC_WKEND2_N",
                value: 20010519);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10526,
                column: "ACC_WKEND2_N",
                value: 20010526);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10602,
                column: "ACC_WKEND2_N",
                value: 20010602);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10609,
                column: "ACC_WKEND2_N",
                value: 20010609);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10616,
                column: "ACC_WKEND2_N",
                value: 20010616);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10623,
                column: "ACC_WKEND2_N",
                value: 20010623);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10630,
                column: "ACC_WKEND2_N",
                value: 20010630);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10707,
                column: "ACC_WKEND2_N",
                value: 20010707);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10714,
                column: "ACC_WKEND2_N",
                value: 20010714);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10721,
                column: "ACC_WKEND2_N",
                value: 20010721);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10728,
                column: "ACC_WKEND2_N",
                value: 20010728);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10804,
                column: "ACC_WKEND2_N",
                value: 20010804);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10811,
                column: "ACC_WKEND2_N",
                value: 20010811);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10818,
                column: "ACC_WKEND2_N",
                value: 20010818);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10825,
                column: "ACC_WKEND2_N",
                value: 20010825);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10901,
                column: "ACC_WKEND2_N",
                value: 20010901);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10908,
                column: "ACC_WKEND2_N",
                value: 20010908);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10915,
                column: "ACC_WKEND2_N",
                value: 20010915);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10922,
                column: "ACC_WKEND2_N",
                value: 20010922);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10929,
                column: "ACC_WKEND2_N",
                value: 20010929);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11006,
                column: "ACC_WKEND2_N",
                value: 20011006);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11013,
                column: "ACC_WKEND2_N",
                value: 20011013);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11020,
                column: "ACC_WKEND2_N",
                value: 20011020);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11027,
                column: "ACC_WKEND2_N",
                value: 20011027);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11103,
                column: "ACC_WKEND2_N",
                value: 20011103);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11110,
                column: "ACC_WKEND2_N",
                value: 20011110);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11117,
                column: "ACC_WKEND2_N",
                value: 20011117);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11124,
                column: "ACC_WKEND2_N",
                value: 20011124);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11201,
                column: "ACC_WKEND2_N",
                value: 20011201);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11208,
                column: "ACC_WKEND2_N",
                value: 20011208);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11215,
                column: "ACC_WKEND2_N",
                value: 20011215);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11222,
                column: "ACC_WKEND2_N",
                value: 20011222);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11229,
                column: "ACC_WKEND2_N",
                value: 20011229);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20105,
                column: "ACC_WKEND2_N",
                value: 20020105);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20112,
                column: "ACC_WKEND2_N",
                value: 20020112);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20119,
                column: "ACC_WKEND2_N",
                value: 20020119);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20126,
                column: "ACC_WKEND2_N",
                value: 20020126);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20202,
                column: "ACC_WKEND2_N",
                value: 20020202);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20209,
                column: "ACC_WKEND2_N",
                value: 20020209);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20216,
                column: "ACC_WKEND2_N",
                value: 20020216);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20223,
                column: "ACC_WKEND2_N",
                value: 20020223);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20302,
                column: "ACC_WKEND2_N",
                value: 20020302);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20309,
                column: "ACC_WKEND2_N",
                value: 20020309);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20316,
                column: "ACC_WKEND2_N",
                value: 20020316);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20323,
                column: "ACC_WKEND2_N",
                value: 20020323);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20330,
                column: "ACC_WKEND2_N",
                value: 20020330);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20406,
                column: "ACC_WKEND2_N",
                value: 20020406);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20413,
                column: "ACC_WKEND2_N",
                value: 20020413);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20420,
                column: "ACC_WKEND2_N",
                value: 20020420);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20427,
                column: "ACC_WKEND2_N",
                value: 20020427);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20504,
                column: "ACC_WKEND2_N",
                value: 20020504);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20511,
                column: "ACC_WKEND2_N",
                value: 20020511);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20518,
                column: "ACC_WKEND2_N",
                value: 20020518);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20525,
                column: "ACC_WKEND2_N",
                value: 20020525);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20601,
                column: "ACC_WKEND2_N",
                value: 20020601);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20608,
                column: "ACC_WKEND2_N",
                value: 20020608);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20615,
                column: "ACC_WKEND2_N",
                value: 20020615);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20622,
                column: "ACC_WKEND2_N",
                value: 20020622);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20629,
                column: "ACC_WKEND2_N",
                value: 20020629);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20706,
                column: "ACC_WKEND2_N",
                value: 20020706);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20713,
                column: "ACC_WKEND2_N",
                value: 20020713);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20720,
                column: "ACC_WKEND2_N",
                value: 20020720);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20727,
                column: "ACC_WKEND2_N",
                value: 20020727);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20803,
                column: "ACC_WKEND2_N",
                value: 20020803);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20810,
                column: "ACC_WKEND2_N",
                value: 20020810);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20817,
                column: "ACC_WKEND2_N",
                value: 20020817);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20824,
                column: "ACC_WKEND2_N",
                value: 20020824);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20831,
                column: "ACC_WKEND2_N",
                value: 20020831);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20907,
                column: "ACC_WKEND2_N",
                value: 20020907);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20914,
                column: "ACC_WKEND2_N",
                value: 20020914);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20921,
                column: "ACC_WKEND2_N",
                value: 20020921);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20928,
                column: "ACC_WKEND2_N",
                value: 20020928);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21005,
                column: "ACC_WKEND2_N",
                value: 20021005);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21012,
                column: "ACC_WKEND2_N",
                value: 20021012);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21019,
                column: "ACC_WKEND2_N",
                value: 20021019);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21026,
                column: "ACC_WKEND2_N",
                value: 20021026);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21102,
                column: "ACC_WKEND2_N",
                value: 20021102);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21109,
                column: "ACC_WKEND2_N",
                value: 20021109);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21116,
                column: "ACC_WKEND2_N",
                value: 20021116);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21123,
                column: "ACC_WKEND2_N",
                value: 20021123);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21130,
                column: "ACC_WKEND2_N",
                value: 20021130);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21207,
                column: "ACC_WKEND2_N",
                value: 20021207);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21214,
                column: "ACC_WKEND2_N",
                value: 20021214);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21221,
                column: "ACC_WKEND2_N",
                value: 20021221);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21228,
                column: "ACC_WKEND2_N",
                value: 20021228);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30104,
                column: "ACC_WKEND2_N",
                value: 20030104);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30111,
                column: "ACC_WKEND2_N",
                value: 20030111);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30118,
                column: "ACC_WKEND2_N",
                value: 20030118);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30125,
                column: "ACC_WKEND2_N",
                value: 20030125);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30201,
                column: "ACC_WKEND2_N",
                value: 20030201);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30208,
                column: "ACC_WKEND2_N",
                value: 20030208);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30215,
                column: "ACC_WKEND2_N",
                value: 20030215);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30222,
                column: "ACC_WKEND2_N",
                value: 20030222);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30301,
                column: "ACC_WKEND2_N",
                value: 20030301);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30308,
                column: "ACC_WKEND2_N",
                value: 20030308);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30315,
                column: "ACC_WKEND2_N",
                value: 20030315);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30322,
                column: "ACC_WKEND2_N",
                value: 20030322);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30329,
                column: "ACC_WKEND2_N",
                value: 20030329);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30405,
                column: "ACC_WKEND2_N",
                value: 20030405);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30412,
                column: "ACC_WKEND2_N",
                value: 20030412);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30419,
                column: "ACC_WKEND2_N",
                value: 20030419);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30426,
                column: "ACC_WKEND2_N",
                value: 20030426);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30503,
                column: "ACC_WKEND2_N",
                value: 20030503);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30510,
                column: "ACC_WKEND2_N",
                value: 20030510);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30517,
                column: "ACC_WKEND2_N",
                value: 20030517);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30524,
                column: "ACC_WKEND2_N",
                value: 20030524);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30531,
                column: "ACC_WKEND2_N",
                value: 20030531);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30607,
                column: "ACC_WKEND2_N",
                value: 20030607);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30614,
                column: "ACC_WKEND2_N",
                value: 20030614);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30621,
                column: "ACC_WKEND2_N",
                value: 20030621);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30628,
                column: "ACC_WKEND2_N",
                value: 20030628);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30705,
                column: "ACC_WKEND2_N",
                value: 20030705);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30712,
                column: "ACC_WKEND2_N",
                value: 20030712);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30719,
                column: "ACC_WKEND2_N",
                value: 20030719);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30726,
                column: "ACC_WKEND2_N",
                value: 20030726);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30802,
                column: "ACC_WKEND2_N",
                value: 20030802);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30809,
                column: "ACC_WKEND2_N",
                value: 20030809);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30816,
                column: "ACC_WKEND2_N",
                value: 20030816);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30823,
                column: "ACC_WKEND2_N",
                value: 20030823);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30830,
                column: "ACC_WKEND2_N",
                value: 20030830);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30906,
                column: "ACC_WKEND2_N",
                value: 20030906);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30913,
                column: "ACC_WKEND2_N",
                value: 20030913);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30920,
                column: "ACC_WKEND2_N",
                value: 20030920);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30927,
                column: "ACC_WKEND2_N",
                value: 20030927);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31004,
                column: "ACC_WKEND2_N",
                value: 20031004);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31011,
                column: "ACC_WKEND2_N",
                value: 20031011);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31018,
                column: "ACC_WKEND2_N",
                value: 20031018);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31025,
                column: "ACC_WKEND2_N",
                value: 20031025);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31101,
                column: "ACC_WKEND2_N",
                value: 20031101);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31108,
                column: "ACC_WKEND2_N",
                value: 20031108);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31115,
                column: "ACC_WKEND2_N",
                value: 20031115);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31122,
                column: "ACC_WKEND2_N",
                value: 20031122);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31129,
                column: "ACC_WKEND2_N",
                value: 20031129);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31206,
                column: "ACC_WKEND2_N",
                value: 20031206);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31213,
                column: "ACC_WKEND2_N",
                value: 20031213);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31220,
                column: "ACC_WKEND2_N",
                value: 20031220);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31227,
                column: "ACC_WKEND2_N",
                value: 20031227);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40103,
                column: "ACC_WKEND2_N",
                value: 20040103);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40110,
                column: "ACC_WKEND2_N",
                value: 20040110);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40117,
                column: "ACC_WKEND2_N",
                value: 20040117);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40124,
                column: "ACC_WKEND2_N",
                value: 20040124);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40131,
                column: "ACC_WKEND2_N",
                value: 20040131);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40207,
                column: "ACC_WKEND2_N",
                value: 20040207);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40214,
                column: "ACC_WKEND2_N",
                value: 20040214);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40221,
                column: "ACC_WKEND2_N",
                value: 20040221);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40228,
                column: "ACC_WKEND2_N",
                value: 20040228);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40306,
                column: "ACC_WKEND2_N",
                value: 20040306);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40313,
                column: "ACC_WKEND2_N",
                value: 20040313);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40320,
                column: "ACC_WKEND2_N",
                value: 20040320);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40327,
                column: "ACC_WKEND2_N",
                value: 20040327);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40403,
                column: "ACC_WKEND2_N",
                value: 20040403);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40410,
                column: "ACC_WKEND2_N",
                value: 20040410);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40417,
                column: "ACC_WKEND2_N",
                value: 20040417);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40424,
                column: "ACC_WKEND2_N",
                value: 20040424);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40501,
                column: "ACC_WKEND2_N",
                value: 20040501);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40508,
                column: "ACC_WKEND2_N",
                value: 20040508);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40515,
                column: "ACC_WKEND2_N",
                value: 20040515);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40522,
                column: "ACC_WKEND2_N",
                value: 20040522);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40529,
                column: "ACC_WKEND2_N",
                value: 20040529);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40605,
                column: "ACC_WKEND2_N",
                value: 20040605);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40612,
                column: "ACC_WKEND2_N",
                value: 20040612);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40619,
                column: "ACC_WKEND2_N",
                value: 20040619);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40626,
                column: "ACC_WKEND2_N",
                value: 20040626);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40703,
                column: "ACC_WKEND2_N",
                value: 20040703);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40710,
                column: "ACC_WKEND2_N",
                value: 20040710);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40717,
                column: "ACC_WKEND2_N",
                value: 20040717);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40724,
                column: "ACC_WKEND2_N",
                value: 20040724);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40731,
                column: "ACC_WKEND2_N",
                value: 20040731);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40807,
                column: "ACC_WKEND2_N",
                value: 20040807);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40814,
                column: "ACC_WKEND2_N",
                value: 20040814);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40821,
                column: "ACC_WKEND2_N",
                value: 20040821);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40828,
                column: "ACC_WKEND2_N",
                value: 20040828);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40904,
                column: "ACC_WKEND2_N",
                value: 20040904);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40911,
                column: "ACC_WKEND2_N",
                value: 20040911);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40918,
                column: "ACC_WKEND2_N",
                value: 20040918);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40925,
                column: "ACC_WKEND2_N",
                value: 20040925);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41002,
                column: "ACC_WKEND2_N",
                value: 20041002);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41009,
                column: "ACC_WKEND2_N",
                value: 20041009);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41016,
                column: "ACC_WKEND2_N",
                value: 20041016);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41023,
                column: "ACC_WKEND2_N",
                value: 20041023);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41030,
                column: "ACC_WKEND2_N",
                value: 20041030);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41106,
                column: "ACC_WKEND2_N",
                value: 20041106);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41113,
                column: "ACC_WKEND2_N",
                value: 20041113);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41120,
                column: "ACC_WKEND2_N",
                value: 20041120);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41127,
                column: "ACC_WKEND2_N",
                value: 20041127);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41204,
                column: "ACC_WKEND2_N",
                value: 20041204);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41211,
                column: "ACC_WKEND2_N",
                value: 20041211);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41218,
                column: "ACC_WKEND2_N",
                value: 20041218);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41225,
                column: "ACC_WKEND2_N",
                value: 20041225);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50101,
                column: "ACC_WKEND2_N",
                value: 20050101);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50108,
                column: "ACC_WKEND2_N",
                value: 20050108);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50115,
                column: "ACC_WKEND2_N",
                value: 20050115);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50122,
                column: "ACC_WKEND2_N",
                value: 20050122);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50129,
                column: "ACC_WKEND2_N",
                value: 20050129);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50205,
                column: "ACC_WKEND2_N",
                value: 20050205);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50212,
                column: "ACC_WKEND2_N",
                value: 20050212);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50219,
                column: "ACC_WKEND2_N",
                value: 20050219);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50226,
                column: "ACC_WKEND2_N",
                value: 20050226);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50305,
                column: "ACC_WKEND2_N",
                value: 20050305);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50312,
                column: "ACC_WKEND2_N",
                value: 20050312);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50319,
                column: "ACC_WKEND2_N",
                value: 20050319);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50326,
                column: "ACC_WKEND2_N",
                value: 20050326);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50402,
                column: "ACC_WKEND2_N",
                value: 20050402);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50409,
                column: "ACC_WKEND2_N",
                value: 20050409);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50416,
                column: "ACC_WKEND2_N",
                value: 20050416);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50423,
                column: "ACC_WKEND2_N",
                value: 20050423);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50430,
                column: "ACC_WKEND2_N",
                value: 20050430);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50507,
                column: "ACC_WKEND2_N",
                value: 20050507);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50514,
                column: "ACC_WKEND2_N",
                value: 20050514);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50521,
                column: "ACC_WKEND2_N",
                value: 20050521);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50528,
                column: "ACC_WKEND2_N",
                value: 20050528);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50604,
                column: "ACC_WKEND2_N",
                value: 20050604);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50611,
                column: "ACC_WKEND2_N",
                value: 20050611);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50618,
                column: "ACC_WKEND2_N",
                value: 20050618);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50625,
                column: "ACC_WKEND2_N",
                value: 20050625);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50702,
                column: "ACC_WKEND2_N",
                value: 20050702);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50709,
                column: "ACC_WKEND2_N",
                value: 20050709);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50716,
                column: "ACC_WKEND2_N",
                value: 20050716);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50723,
                column: "ACC_WKEND2_N",
                value: 20050723);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50730,
                column: "ACC_WKEND2_N",
                value: 20050730);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50806,
                column: "ACC_WKEND2_N",
                value: 20050806);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50813,
                column: "ACC_WKEND2_N",
                value: 20050813);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50820,
                column: "ACC_WKEND2_N",
                value: 20050820);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50827,
                column: "ACC_WKEND2_N",
                value: 20050827);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50903,
                column: "ACC_WKEND2_N",
                value: 20050903);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50910,
                column: "ACC_WKEND2_N",
                value: 20050910);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50917,
                column: "ACC_WKEND2_N",
                value: 20050917);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50924,
                column: "ACC_WKEND2_N",
                value: 20050924);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51001,
                column: "ACC_WKEND2_N",
                value: 20051001);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51008,
                column: "ACC_WKEND2_N",
                value: 20051008);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51015,
                column: "ACC_WKEND2_N",
                value: 20051015);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51022,
                column: "ACC_WKEND2_N",
                value: 20051022);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51029,
                column: "ACC_WKEND2_N",
                value: 20051029);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51105,
                column: "ACC_WKEND2_N",
                value: 20051105);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51112,
                column: "ACC_WKEND2_N",
                value: 20051112);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51119,
                column: "ACC_WKEND2_N",
                value: 20051119);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51126,
                column: "ACC_WKEND2_N",
                value: 20051126);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51203,
                column: "ACC_WKEND2_N",
                value: 20051203);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51210,
                column: "ACC_WKEND2_N",
                value: 20051210);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51217,
                column: "ACC_WKEND2_N",
                value: 20051217);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51224,
                column: "ACC_WKEND2_N",
                value: 20051224);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51231,
                column: "ACC_WKEND2_N",
                value: 20051231);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60107,
                column: "ACC_WKEND2_N",
                value: 20060107);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60114,
                column: "ACC_WKEND2_N",
                value: 20060114);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60121,
                column: "ACC_WKEND2_N",
                value: 20060121);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60128,
                column: "ACC_WKEND2_N",
                value: 20060128);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60204,
                column: "ACC_WKEND2_N",
                value: 20060204);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60211,
                column: "ACC_WKEND2_N",
                value: 20060211);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60218,
                column: "ACC_WKEND2_N",
                value: 20060218);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60225,
                column: "ACC_WKEND2_N",
                value: 20060225);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60304,
                column: "ACC_WKEND2_N",
                value: 20060304);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60311,
                column: "ACC_WKEND2_N",
                value: 20060311);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60318,
                column: "ACC_WKEND2_N",
                value: 20060318);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60325,
                column: "ACC_WKEND2_N",
                value: 20060325);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60401,
                column: "ACC_WKEND2_N",
                value: 20060401);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60408,
                column: "ACC_WKEND2_N",
                value: 20060408);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60415,
                column: "ACC_WKEND2_N",
                value: 20060415);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60422,
                column: "ACC_WKEND2_N",
                value: 20060422);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60429,
                column: "ACC_WKEND2_N",
                value: 20060429);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60506,
                column: "ACC_WKEND2_N",
                value: 20060506);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60513,
                column: "ACC_WKEND2_N",
                value: 20060513);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60520,
                column: "ACC_WKEND2_N",
                value: 20060520);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60527,
                column: "ACC_WKEND2_N",
                value: 20060527);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60603,
                column: "ACC_WKEND2_N",
                value: 20060603);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60610,
                column: "ACC_WKEND2_N",
                value: 20060610);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60617,
                column: "ACC_WKEND2_N",
                value: 20060617);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60624,
                column: "ACC_WKEND2_N",
                value: 20060624);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60701,
                column: "ACC_WKEND2_N",
                value: 20060701);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60708,
                column: "ACC_WKEND2_N",
                value: 20060708);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60715,
                column: "ACC_WKEND2_N",
                value: 20060715);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60722,
                column: "ACC_WKEND2_N",
                value: 20060722);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60729,
                column: "ACC_WKEND2_N",
                value: 20060729);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60805,
                column: "ACC_WKEND2_N",
                value: 20060805);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60812,
                column: "ACC_WKEND2_N",
                value: 20060812);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60819,
                column: "ACC_WKEND2_N",
                value: 20060819);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60826,
                column: "ACC_WKEND2_N",
                value: 20060826);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60902,
                column: "ACC_WKEND2_N",
                value: 20060902);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60909,
                column: "ACC_WKEND2_N",
                value: 20060909);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60916,
                column: "ACC_WKEND2_N",
                value: 20060916);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60923,
                column: "ACC_WKEND2_N",
                value: 20060923);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60930,
                column: "ACC_WKEND2_N",
                value: 20060930);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61007,
                column: "ACC_WKEND2_N",
                value: 20061007);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61014,
                column: "ACC_WKEND2_N",
                value: 20061014);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61021,
                column: "ACC_WKEND2_N",
                value: 20061021);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61028,
                column: "ACC_WKEND2_N",
                value: 20061028);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61104,
                column: "ACC_WKEND2_N",
                value: 20061104);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61111,
                column: "ACC_WKEND2_N",
                value: 20061111);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61118,
                column: "ACC_WKEND2_N",
                value: 20061118);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61125,
                column: "ACC_WKEND2_N",
                value: 20061125);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61202,
                column: "ACC_WKEND2_N",
                value: 20061202);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61209,
                column: "ACC_WKEND2_N",
                value: 20061209);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61216,
                column: "ACC_WKEND2_N",
                value: 20061216);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61223,
                column: "ACC_WKEND2_N",
                value: 20061223);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61230,
                column: "ACC_WKEND2_N",
                value: 20061230);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70106,
                column: "ACC_WKEND2_N",
                value: 20070106);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70113,
                column: "ACC_WKEND2_N",
                value: 20070113);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70120,
                column: "ACC_WKEND2_N",
                value: 20070120);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70127,
                column: "ACC_WKEND2_N",
                value: 20070127);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70203,
                column: "ACC_WKEND2_N",
                value: 20070203);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70210,
                column: "ACC_WKEND2_N",
                value: 20070210);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70217,
                column: "ACC_WKEND2_N",
                value: 20070217);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70224,
                column: "ACC_WKEND2_N",
                value: 20070224);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70303,
                column: "ACC_WKEND2_N",
                value: 20070303);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70310,
                column: "ACC_WKEND2_N",
                value: 20070310);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70317,
                column: "ACC_WKEND2_N",
                value: 20070317);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70324,
                column: "ACC_WKEND2_N",
                value: 20070324);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70331,
                column: "ACC_WKEND2_N",
                value: 20070331);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70407,
                column: "ACC_WKEND2_N",
                value: 20070407);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70414,
                column: "ACC_WKEND2_N",
                value: 20070414);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70421,
                column: "ACC_WKEND2_N",
                value: 20070421);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70428,
                column: "ACC_WKEND2_N",
                value: 20070428);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70505,
                column: "ACC_WKEND2_N",
                value: 20070505);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70512,
                column: "ACC_WKEND2_N",
                value: 20070512);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70519,
                column: "ACC_WKEND2_N",
                value: 20070519);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70526,
                column: "ACC_WKEND2_N",
                value: 20070526);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70602,
                column: "ACC_WKEND2_N",
                value: 20070602);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70609,
                column: "ACC_WKEND2_N",
                value: 20070609);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70616,
                column: "ACC_WKEND2_N",
                value: 20070616);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70623,
                column: "ACC_WKEND2_N",
                value: 20070623);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70630,
                column: "ACC_WKEND2_N",
                value: 20070630);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70707,
                column: "ACC_WKEND2_N",
                value: 20070707);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70714,
                column: "ACC_WKEND2_N",
                value: 20070714);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70721,
                column: "ACC_WKEND2_N",
                value: 20070721);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70728,
                column: "ACC_WKEND2_N",
                value: 20070728);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70804,
                column: "ACC_WKEND2_N",
                value: 20070804);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70811,
                column: "ACC_WKEND2_N",
                value: 20070811);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70818,
                column: "ACC_WKEND2_N",
                value: 20070818);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70825,
                column: "ACC_WKEND2_N",
                value: 20070825);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70901,
                column: "ACC_WKEND2_N",
                value: 20070901);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70908,
                column: "ACC_WKEND2_N",
                value: 20070908);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70915,
                column: "ACC_WKEND2_N",
                value: 20070915);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70922,
                column: "ACC_WKEND2_N",
                value: 20070922);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70929,
                column: "ACC_WKEND2_N",
                value: 20070929);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71006,
                column: "ACC_WKEND2_N",
                value: 20071006);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71013,
                column: "ACC_WKEND2_N",
                value: 20071013);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71020,
                column: "ACC_WKEND2_N",
                value: 20071020);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71027,
                column: "ACC_WKEND2_N",
                value: 20071027);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71103,
                column: "ACC_WKEND2_N",
                value: 20071103);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71110,
                column: "ACC_WKEND2_N",
                value: 20071110);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71117,
                column: "ACC_WKEND2_N",
                value: 20071117);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71124,
                column: "ACC_WKEND2_N",
                value: 20071124);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71201,
                column: "ACC_WKEND2_N",
                value: 20071201);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71208,
                column: "ACC_WKEND2_N",
                value: 20071208);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71215,
                column: "ACC_WKEND2_N",
                value: 20071215);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71222,
                column: "ACC_WKEND2_N",
                value: 20071222);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71229,
                column: "ACC_WKEND2_N",
                value: 20071229);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80105,
                column: "ACC_WKEND2_N",
                value: 20080105);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80112,
                column: "ACC_WKEND2_N",
                value: 20080112);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80119,
                column: "ACC_WKEND2_N",
                value: 20080119);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80126,
                column: "ACC_WKEND2_N",
                value: 20080126);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80202,
                column: "ACC_WKEND2_N",
                value: 20080202);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80209,
                column: "ACC_WKEND2_N",
                value: 20080209);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80216,
                column: "ACC_WKEND2_N",
                value: 20080216);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80223,
                column: "ACC_WKEND2_N",
                value: 20080223);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80301,
                column: "ACC_WKEND2_N",
                value: 20080301);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80308,
                column: "ACC_WKEND2_N",
                value: 20080308);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80315,
                column: "ACC_WKEND2_N",
                value: 20080315);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80322,
                column: "ACC_WKEND2_N",
                value: 20080322);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80329,
                column: "ACC_WKEND2_N",
                value: 20080329);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80405,
                column: "ACC_WKEND2_N",
                value: 20080405);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80412,
                column: "ACC_WKEND2_N",
                value: 20080412);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80419,
                column: "ACC_WKEND2_N",
                value: 20080419);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80426,
                column: "ACC_WKEND2_N",
                value: 20080426);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80503,
                column: "ACC_WKEND2_N",
                value: 20080503);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80510,
                column: "ACC_WKEND2_N",
                value: 20080510);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80517,
                column: "ACC_WKEND2_N",
                value: 20080517);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80524,
                column: "ACC_WKEND2_N",
                value: 20080524);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80531,
                column: "ACC_WKEND2_N",
                value: 20080531);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80607,
                column: "ACC_WKEND2_N",
                value: 20080607);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80614,
                column: "ACC_WKEND2_N",
                value: 20080614);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80621,
                column: "ACC_WKEND2_N",
                value: 20080621);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80628,
                column: "ACC_WKEND2_N",
                value: 20080628);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80705,
                column: "ACC_WKEND2_N",
                value: 20080705);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80712,
                column: "ACC_WKEND2_N",
                value: 20080712);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80719,
                column: "ACC_WKEND2_N",
                value: 20080719);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80726,
                column: "ACC_WKEND2_N",
                value: 20080726);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80802,
                column: "ACC_WKEND2_N",
                value: 20080802);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80809,
                column: "ACC_WKEND2_N",
                value: 20080809);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80816,
                column: "ACC_WKEND2_N",
                value: 20080816);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80823,
                column: "ACC_WKEND2_N",
                value: 20080823);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80830,
                column: "ACC_WKEND2_N",
                value: 20080830);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80906,
                column: "ACC_WKEND2_N",
                value: 20080906);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80913,
                column: "ACC_WKEND2_N",
                value: 20080913);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80920,
                column: "ACC_WKEND2_N",
                value: 20080920);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80927,
                column: "ACC_WKEND2_N",
                value: 20080927);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81004,
                column: "ACC_WKEND2_N",
                value: 20081004);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81011,
                column: "ACC_WKEND2_N",
                value: 20081011);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81018,
                column: "ACC_WKEND2_N",
                value: 20081018);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81025,
                column: "ACC_WKEND2_N",
                value: 20081025);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81101,
                column: "ACC_WKEND2_N",
                value: 20081101);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81108,
                column: "ACC_WKEND2_N",
                value: 20081108);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81115,
                column: "ACC_WKEND2_N",
                value: 20081115);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81122,
                column: "ACC_WKEND2_N",
                value: 20081122);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81129,
                column: "ACC_WKEND2_N",
                value: 20081129);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81206,
                column: "ACC_WKEND2_N",
                value: 20081206);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81213,
                column: "ACC_WKEND2_N",
                value: 20081213);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81220,
                column: "ACC_WKEND2_N",
                value: 20081220);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81227,
                column: "ACC_WKEND2_N",
                value: 20081227);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90103,
                column: "ACC_WKEND2_N",
                value: 20090103);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90110,
                column: "ACC_WKEND2_N",
                value: 20090110);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90117,
                column: "ACC_WKEND2_N",
                value: 20090117);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90124,
                column: "ACC_WKEND2_N",
                value: 20090124);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90131,
                column: "ACC_WKEND2_N",
                value: 20090131);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90207,
                column: "ACC_WKEND2_N",
                value: 20090207);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90214,
                column: "ACC_WKEND2_N",
                value: 20090214);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90221,
                column: "ACC_WKEND2_N",
                value: 20090221);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90228,
                column: "ACC_WKEND2_N",
                value: 20090228);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90307,
                column: "ACC_WKEND2_N",
                value: 20090307);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90314,
                column: "ACC_WKEND2_N",
                value: 20090314);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90321,
                column: "ACC_WKEND2_N",
                value: 20090321);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90328,
                column: "ACC_WKEND2_N",
                value: 20090328);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90404,
                column: "ACC_WKEND2_N",
                value: 20090404);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90411,
                column: "ACC_WKEND2_N",
                value: 20090411);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90418,
                column: "ACC_WKEND2_N",
                value: 20090418);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90425,
                column: "ACC_WKEND2_N",
                value: 20090425);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90502,
                column: "ACC_WKEND2_N",
                value: 20090502);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90509,
                column: "ACC_WKEND2_N",
                value: 20090509);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90516,
                column: "ACC_WKEND2_N",
                value: 20090516);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90523,
                column: "ACC_WKEND2_N",
                value: 20090523);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90530,
                column: "ACC_WKEND2_N",
                value: 20090530);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90606,
                column: "ACC_WKEND2_N",
                value: 20090606);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90613,
                column: "ACC_WKEND2_N",
                value: 20090613);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90620,
                column: "ACC_WKEND2_N",
                value: 20090620);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90627,
                column: "ACC_WKEND2_N",
                value: 20090627);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90704,
                column: "ACC_WKEND2_N",
                value: 20090704);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90711,
                column: "ACC_WKEND2_N",
                value: 20090711);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90718,
                column: "ACC_WKEND2_N",
                value: 20090718);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90725,
                column: "ACC_WKEND2_N",
                value: 20090725);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90801,
                column: "ACC_WKEND2_N",
                value: 20090801);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90808,
                column: "ACC_WKEND2_N",
                value: 20090808);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90815,
                column: "ACC_WKEND2_N",
                value: 20090815);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90822,
                column: "ACC_WKEND2_N",
                value: 20090822);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90829,
                column: "ACC_WKEND2_N",
                value: 20090829);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90905,
                column: "ACC_WKEND2_N",
                value: 20090905);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90912,
                column: "ACC_WKEND2_N",
                value: 20090912);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90919,
                column: "ACC_WKEND2_N",
                value: 20090919);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90926,
                column: "ACC_WKEND2_N",
                value: 20090926);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91003,
                column: "ACC_WKEND2_N",
                value: 20091003);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91010,
                column: "ACC_WKEND2_N",
                value: 20091010);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91017,
                column: "ACC_WKEND2_N",
                value: 20091017);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91024,
                column: "ACC_WKEND2_N",
                value: 20091024);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91031,
                column: "ACC_WKEND2_N",
                value: 20091031);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91107,
                column: "ACC_WKEND2_N",
                value: 20091107);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91114,
                column: "ACC_WKEND2_N",
                value: 20091114);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91121,
                column: "ACC_WKEND2_N",
                value: 20091121);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91128,
                column: "ACC_WKEND2_N",
                value: 20091128);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91205,
                column: "ACC_WKEND2_N",
                value: 20091205);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91212,
                column: "ACC_WKEND2_N",
                value: 20091212);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91219,
                column: "ACC_WKEND2_N",
                value: 20091219);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91226,
                column: "ACC_WKEND2_N",
                value: 20091226);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100102,
                column: "ACC_WKEND2_N",
                value: 20100102);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100109,
                column: "ACC_WKEND2_N",
                value: 20100109);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100116,
                column: "ACC_WKEND2_N",
                value: 20100116);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100123,
                column: "ACC_WKEND2_N",
                value: 20100123);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100130,
                column: "ACC_WKEND2_N",
                value: 20100130);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100206,
                column: "ACC_WKEND2_N",
                value: 20100206);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100213,
                column: "ACC_WKEND2_N",
                value: 20100213);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100220,
                column: "ACC_WKEND2_N",
                value: 20100220);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100227,
                column: "ACC_WKEND2_N",
                value: 20100227);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100306,
                column: "ACC_WKEND2_N",
                value: 20100306);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100313,
                column: "ACC_WKEND2_N",
                value: 20100313);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100320,
                column: "ACC_WKEND2_N",
                value: 20100320);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100327,
                column: "ACC_WKEND2_N",
                value: 20100327);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100403,
                column: "ACC_WKEND2_N",
                value: 20100403);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100410,
                column: "ACC_WKEND2_N",
                value: 20100410);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100417,
                column: "ACC_WKEND2_N",
                value: 20100417);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100424,
                column: "ACC_WKEND2_N",
                value: 20100424);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100501,
                column: "ACC_WKEND2_N",
                value: 20100501);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100508,
                column: "ACC_WKEND2_N",
                value: 20100508);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100515,
                column: "ACC_WKEND2_N",
                value: 20100515);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100522,
                column: "ACC_WKEND2_N",
                value: 20100522);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100529,
                column: "ACC_WKEND2_N",
                value: 20100529);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100605,
                column: "ACC_WKEND2_N",
                value: 20100605);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100612,
                column: "ACC_WKEND2_N",
                value: 20100612);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100619,
                column: "ACC_WKEND2_N",
                value: 20100619);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100626,
                column: "ACC_WKEND2_N",
                value: 20100626);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100703,
                column: "ACC_WKEND2_N",
                value: 20100703);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100710,
                column: "ACC_WKEND2_N",
                value: 20100710);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100717,
                column: "ACC_WKEND2_N",
                value: 20100717);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100724,
                column: "ACC_WKEND2_N",
                value: 20100724);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100731,
                column: "ACC_WKEND2_N",
                value: 20100731);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100807,
                column: "ACC_WKEND2_N",
                value: 20100807);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100814,
                column: "ACC_WKEND2_N",
                value: 20100814);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100821,
                column: "ACC_WKEND2_N",
                value: 20100821);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100828,
                column: "ACC_WKEND2_N",
                value: 20100828);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100904,
                column: "ACC_WKEND2_N",
                value: 20100904);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100911,
                column: "ACC_WKEND2_N",
                value: 20100911);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100918,
                column: "ACC_WKEND2_N",
                value: 20100918);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100925,
                column: "ACC_WKEND2_N",
                value: 20100925);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101002,
                column: "ACC_WKEND2_N",
                value: 20101002);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101009,
                column: "ACC_WKEND2_N",
                value: 20101009);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101016,
                column: "ACC_WKEND2_N",
                value: 20101016);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101023,
                column: "ACC_WKEND2_N",
                value: 20101023);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101030,
                column: "ACC_WKEND2_N",
                value: 20101030);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101106,
                column: "ACC_WKEND2_N",
                value: 20101106);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101113,
                column: "ACC_WKEND2_N",
                value: 20101113);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101120,
                column: "ACC_WKEND2_N",
                value: 20101120);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101127,
                column: "ACC_WKEND2_N",
                value: 20101127);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101204,
                column: "ACC_WKEND2_N",
                value: 20101204);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101211,
                column: "ACC_WKEND2_N",
                value: 20101211);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101218,
                column: "ACC_WKEND2_N",
                value: 20101218);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101225,
                column: "ACC_WKEND2_N",
                value: 20101225);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110101,
                column: "ACC_WKEND2_N",
                value: 20110101);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110108,
                column: "ACC_WKEND2_N",
                value: 20110108);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110115,
                column: "ACC_WKEND2_N",
                value: 20110115);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110122,
                column: "ACC_WKEND2_N",
                value: 20110122);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110129,
                column: "ACC_WKEND2_N",
                value: 20110129);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110205,
                column: "ACC_WKEND2_N",
                value: 20110205);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110212,
                column: "ACC_WKEND2_N",
                value: 20110212);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110219,
                column: "ACC_WKEND2_N",
                value: 20110219);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110226,
                column: "ACC_WKEND2_N",
                value: 20110226);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110305,
                column: "ACC_WKEND2_N",
                value: 20110305);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110312,
                column: "ACC_WKEND2_N",
                value: 20110312);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110319,
                column: "ACC_WKEND2_N",
                value: 20110319);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110326,
                column: "ACC_WKEND2_N",
                value: 20110326);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110402,
                column: "ACC_WKEND2_N",
                value: 20110402);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110409,
                column: "ACC_WKEND2_N",
                value: 20110409);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110416,
                column: "ACC_WKEND2_N",
                value: 20110416);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110423,
                column: "ACC_WKEND2_N",
                value: 20110423);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110430,
                column: "ACC_WKEND2_N",
                value: 20110430);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110507,
                column: "ACC_WKEND2_N",
                value: 20110507);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110514,
                column: "ACC_WKEND2_N",
                value: 20110514);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110521,
                column: "ACC_WKEND2_N",
                value: 20110521);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110528,
                column: "ACC_WKEND2_N",
                value: 20110528);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110604,
                column: "ACC_WKEND2_N",
                value: 20110604);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110611,
                column: "ACC_WKEND2_N",
                value: 20110611);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110618,
                column: "ACC_WKEND2_N",
                value: 20110618);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110625,
                column: "ACC_WKEND2_N",
                value: 20110625);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110702,
                column: "ACC_WKEND2_N",
                value: 20110702);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110709,
                column: "ACC_WKEND2_N",
                value: 20110709);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110716,
                column: "ACC_WKEND2_N",
                value: 20110716);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110723,
                column: "ACC_WKEND2_N",
                value: 20110723);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110730,
                column: "ACC_WKEND2_N",
                value: 20110730);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110806,
                column: "ACC_WKEND2_N",
                value: 20110806);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110813,
                column: "ACC_WKEND2_N",
                value: 20110813);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110820,
                column: "ACC_WKEND2_N",
                value: 20110820);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110827,
                column: "ACC_WKEND2_N",
                value: 20110827);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110903,
                column: "ACC_WKEND2_N",
                value: 20110903);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110910,
                column: "ACC_WKEND2_N",
                value: 20110910);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110917,
                column: "ACC_WKEND2_N",
                value: 20110917);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110924,
                column: "ACC_WKEND2_N",
                value: 20110924);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111001,
                column: "ACC_WKEND2_N",
                value: 20111001);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111008,
                column: "ACC_WKEND2_N",
                value: 20111008);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111015,
                column: "ACC_WKEND2_N",
                value: 20111015);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111022,
                column: "ACC_WKEND2_N",
                value: 20111022);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111029,
                column: "ACC_WKEND2_N",
                value: 20111029);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111105,
                column: "ACC_WKEND2_N",
                value: 20111105);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111112,
                column: "ACC_WKEND2_N",
                value: 20111112);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111119,
                column: "ACC_WKEND2_N",
                value: 20111119);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111126,
                column: "ACC_WKEND2_N",
                value: 20111126);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111203,
                column: "ACC_WKEND2_N",
                value: 20111203);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111210,
                column: "ACC_WKEND2_N",
                value: 20111210);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111217,
                column: "ACC_WKEND2_N",
                value: 20111217);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111224,
                column: "ACC_WKEND2_N",
                value: 20111224);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111231,
                column: "ACC_WKEND2_N",
                value: 20111231);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120107,
                column: "ACC_WKEND2_N",
                value: 20120107);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120114,
                column: "ACC_WKEND2_N",
                value: 20120114);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120121,
                column: "ACC_WKEND2_N",
                value: 20120121);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120128,
                column: "ACC_WKEND2_N",
                value: 20120128);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120204,
                column: "ACC_WKEND2_N",
                value: 20120204);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120211,
                column: "ACC_WKEND2_N",
                value: 20120211);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120218,
                column: "ACC_WKEND2_N",
                value: 20120218);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120225,
                column: "ACC_WKEND2_N",
                value: 20120225);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120303,
                column: "ACC_WKEND2_N",
                value: 20120303);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120310,
                column: "ACC_WKEND2_N",
                value: 20120310);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120317,
                column: "ACC_WKEND2_N",
                value: 20120317);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120324,
                column: "ACC_WKEND2_N",
                value: 20120324);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120331,
                column: "ACC_WKEND2_N",
                value: 20120331);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120407,
                column: "ACC_WKEND2_N",
                value: 20120407);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120414,
                column: "ACC_WKEND2_N",
                value: 20120414);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120421,
                column: "ACC_WKEND2_N",
                value: 20120421);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120428,
                column: "ACC_WKEND2_N",
                value: 20120428);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120505,
                column: "ACC_WKEND2_N",
                value: 20120505);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120512,
                column: "ACC_WKEND2_N",
                value: 20120512);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120519,
                column: "ACC_WKEND2_N",
                value: 20120519);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120526,
                column: "ACC_WKEND2_N",
                value: 20120526);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120602,
                column: "ACC_WKEND2_N",
                value: 20120602);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120609,
                column: "ACC_WKEND2_N",
                value: 20120609);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120616,
                column: "ACC_WKEND2_N",
                value: 20120616);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120623,
                column: "ACC_WKEND2_N",
                value: 20120623);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120630,
                column: "ACC_WKEND2_N",
                value: 20120630);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120707,
                column: "ACC_WKEND2_N",
                value: 20120707);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120714,
                column: "ACC_WKEND2_N",
                value: 20120714);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120721,
                column: "ACC_WKEND2_N",
                value: 20120721);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120728,
                column: "ACC_WKEND2_N",
                value: 20120728);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120804,
                column: "ACC_WKEND2_N",
                value: 20120804);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120811,
                column: "ACC_WKEND2_N",
                value: 20120811);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120818,
                column: "ACC_WKEND2_N",
                value: 20120818);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120825,
                column: "ACC_WKEND2_N",
                value: 20120825);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120901,
                column: "ACC_WKEND2_N",
                value: 20120901);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120908,
                column: "ACC_WKEND2_N",
                value: 20120908);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120915,
                column: "ACC_WKEND2_N",
                value: 20120915);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120922,
                column: "ACC_WKEND2_N",
                value: 20120922);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120929,
                column: "ACC_WKEND2_N",
                value: 20120929);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121006,
                column: "ACC_WKEND2_N",
                value: 20121006);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121013,
                column: "ACC_WKEND2_N",
                value: 20121013);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121020,
                column: "ACC_WKEND2_N",
                value: 20121020);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121027,
                column: "ACC_WKEND2_N",
                value: 20121027);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121103,
                column: "ACC_WKEND2_N",
                value: 20121103);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121110,
                column: "ACC_WKEND2_N",
                value: 20121110);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121117,
                column: "ACC_WKEND2_N",
                value: 20121117);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121124,
                column: "ACC_WKEND2_N",
                value: 20121124);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121201,
                column: "ACC_WKEND2_N",
                value: 20121201);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121208,
                column: "ACC_WKEND2_N",
                value: 20121208);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121215,
                column: "ACC_WKEND2_N",
                value: 20121215);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121222,
                column: "ACC_WKEND2_N",
                value: 20121222);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121229,
                column: "ACC_WKEND2_N",
                value: 20121229);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130105,
                column: "ACC_WKEND2_N",
                value: 20130105);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130112,
                column: "ACC_WKEND2_N",
                value: 20130112);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130119,
                column: "ACC_WKEND2_N",
                value: 20130119);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130126,
                column: "ACC_WKEND2_N",
                value: 20130126);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130202,
                column: "ACC_WKEND2_N",
                value: 20130202);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130209,
                column: "ACC_WKEND2_N",
                value: 20130209);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130216,
                column: "ACC_WKEND2_N",
                value: 20130216);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130223,
                column: "ACC_WKEND2_N",
                value: 20130223);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130302,
                column: "ACC_WKEND2_N",
                value: 20130302);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130309,
                column: "ACC_WKEND2_N",
                value: 20130309);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130316,
                column: "ACC_WKEND2_N",
                value: 20130316);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130323,
                column: "ACC_WKEND2_N",
                value: 20130323);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130330,
                column: "ACC_WKEND2_N",
                value: 20130330);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130406,
                column: "ACC_WKEND2_N",
                value: 20130406);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130413,
                column: "ACC_WKEND2_N",
                value: 20130413);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130420,
                column: "ACC_WKEND2_N",
                value: 20130420);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130427,
                column: "ACC_WKEND2_N",
                value: 20130427);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130504,
                column: "ACC_WKEND2_N",
                value: 20130504);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130511,
                column: "ACC_WKEND2_N",
                value: 20130511);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130518,
                column: "ACC_WKEND2_N",
                value: 20130518);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130525,
                column: "ACC_WKEND2_N",
                value: 20130525);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130601,
                column: "ACC_WKEND2_N",
                value: 20130601);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130608,
                column: "ACC_WKEND2_N",
                value: 20130608);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130615,
                column: "ACC_WKEND2_N",
                value: 20130615);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130622,
                column: "ACC_WKEND2_N",
                value: 20130622);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130629,
                column: "ACC_WKEND2_N",
                value: 20130629);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130706,
                column: "ACC_WKEND2_N",
                value: 20130706);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130713,
                column: "ACC_WKEND2_N",
                value: 20130713);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130720,
                column: "ACC_WKEND2_N",
                value: 20130720);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130727,
                column: "ACC_WKEND2_N",
                value: 20130727);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130803,
                column: "ACC_WKEND2_N",
                value: 20130803);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130810,
                column: "ACC_WKEND2_N",
                value: 20130810);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130817,
                column: "ACC_WKEND2_N",
                value: 20130817);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130824,
                column: "ACC_WKEND2_N",
                value: 20130824);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130831,
                column: "ACC_WKEND2_N",
                value: 20130831);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130907,
                column: "ACC_WKEND2_N",
                value: 20130907);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130914,
                column: "ACC_WKEND2_N",
                value: 20130914);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130921,
                column: "ACC_WKEND2_N",
                value: 20130921);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130928,
                column: "ACC_WKEND2_N",
                value: 20130928);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131005,
                column: "ACC_WKEND2_N",
                value: 20131005);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131012,
                column: "ACC_WKEND2_N",
                value: 20131012);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131019,
                column: "ACC_WKEND2_N",
                value: 20131019);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131026,
                column: "ACC_WKEND2_N",
                value: 20131026);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131102,
                column: "ACC_WKEND2_N",
                value: 20131102);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131109,
                column: "ACC_WKEND2_N",
                value: 20131109);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131116,
                column: "ACC_WKEND2_N",
                value: 20131116);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131123,
                column: "ACC_WKEND2_N",
                value: 20131123);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131130,
                column: "ACC_WKEND2_N",
                value: 20131130);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131207,
                column: "ACC_WKEND2_N",
                value: 20131207);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131214,
                column: "ACC_WKEND2_N",
                value: 20131214);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131221,
                column: "ACC_WKEND2_N",
                value: 20131221);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131228,
                column: "ACC_WKEND2_N",
                value: 20131228);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140104,
                column: "ACC_WKEND2_N",
                value: 20140104);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140111,
                column: "ACC_WKEND2_N",
                value: 20140111);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140118,
                column: "ACC_WKEND2_N",
                value: 20140118);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140125,
                column: "ACC_WKEND2_N",
                value: 20140125);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140201,
                column: "ACC_WKEND2_N",
                value: 20140201);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140208,
                column: "ACC_WKEND2_N",
                value: 20140208);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140215,
                column: "ACC_WKEND2_N",
                value: 20140215);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140222,
                column: "ACC_WKEND2_N",
                value: 20140222);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140301,
                column: "ACC_WKEND2_N",
                value: 20140301);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140308,
                column: "ACC_WKEND2_N",
                value: 20140308);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140315,
                column: "ACC_WKEND2_N",
                value: 20140315);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140322,
                column: "ACC_WKEND2_N",
                value: 20140322);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140329,
                column: "ACC_WKEND2_N",
                value: 20140329);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140405,
                column: "ACC_WKEND2_N",
                value: 20140405);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140412,
                column: "ACC_WKEND2_N",
                value: 20140412);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140419,
                column: "ACC_WKEND2_N",
                value: 20140419);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140426,
                column: "ACC_WKEND2_N",
                value: 20140426);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140503,
                column: "ACC_WKEND2_N",
                value: 20140503);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140510,
                column: "ACC_WKEND2_N",
                value: 20140510);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140517,
                column: "ACC_WKEND2_N",
                value: 20140517);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140524,
                column: "ACC_WKEND2_N",
                value: 20140524);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140531,
                column: "ACC_WKEND2_N",
                value: 20140531);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140607,
                column: "ACC_WKEND2_N",
                value: 20140607);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140614,
                column: "ACC_WKEND2_N",
                value: 20140614);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140621,
                column: "ACC_WKEND2_N",
                value: 20140621);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140628,
                column: "ACC_WKEND2_N",
                value: 20140628);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140705,
                column: "ACC_WKEND2_N",
                value: 20140705);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140712,
                column: "ACC_WKEND2_N",
                value: 20140712);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140719,
                column: "ACC_WKEND2_N",
                value: 20140719);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140726,
                column: "ACC_WKEND2_N",
                value: 20140726);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140802,
                column: "ACC_WKEND2_N",
                value: 20140802);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140809,
                column: "ACC_WKEND2_N",
                value: 20140809);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140816,
                column: "ACC_WKEND2_N",
                value: 20140816);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140823,
                column: "ACC_WKEND2_N",
                value: 20140823);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140830,
                column: "ACC_WKEND2_N",
                value: 20140830);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140906,
                column: "ACC_WKEND2_N",
                value: 20140906);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140913,
                column: "ACC_WKEND2_N",
                value: 20140913);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140920,
                column: "ACC_WKEND2_N",
                value: 20140920);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140927,
                column: "ACC_WKEND2_N",
                value: 20140927);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141004,
                column: "ACC_WKEND2_N",
                value: 20141004);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141011,
                column: "ACC_WKEND2_N",
                value: 20141011);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141018,
                column: "ACC_WKEND2_N",
                value: 20141018);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141025,
                column: "ACC_WKEND2_N",
                value: 20141025);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141101,
                column: "ACC_WKEND2_N",
                value: 20141101);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141108,
                column: "ACC_WKEND2_N",
                value: 20141108);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141115,
                column: "ACC_WKEND2_N",
                value: 20141115);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141122,
                column: "ACC_WKEND2_N",
                value: 20141122);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141129,
                column: "ACC_WKEND2_N",
                value: 20141129);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141206,
                column: "ACC_WKEND2_N",
                value: 20141206);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141213,
                column: "ACC_WKEND2_N",
                value: 20141213);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141220,
                column: "ACC_WKEND2_N",
                value: 20141220);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141227,
                column: "ACC_WKEND2_N",
                value: 20141227);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150103,
                column: "ACC_WKEND2_N",
                value: 20150103);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150110,
                column: "ACC_WKEND2_N",
                value: 20150110);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150117,
                column: "ACC_WKEND2_N",
                value: 20150117);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150124,
                column: "ACC_WKEND2_N",
                value: 20150124);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150131,
                column: "ACC_WKEND2_N",
                value: 20150131);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150207,
                column: "ACC_WKEND2_N",
                value: 20150207);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150214,
                column: "ACC_WKEND2_N",
                value: 20150214);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150221,
                column: "ACC_WKEND2_N",
                value: 20150221);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150228,
                column: "ACC_WKEND2_N",
                value: 20150228);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150307,
                column: "ACC_WKEND2_N",
                value: 20150307);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150314,
                column: "ACC_WKEND2_N",
                value: 20150314);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150321,
                column: "ACC_WKEND2_N",
                value: 20150321);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150328,
                column: "ACC_WKEND2_N",
                value: 20150328);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150404,
                column: "ACC_WKEND2_N",
                value: 20150404);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150411,
                column: "ACC_WKEND2_N",
                value: 20150411);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150418,
                column: "ACC_WKEND2_N",
                value: 20150418);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150425,
                column: "ACC_WKEND2_N",
                value: 20150425);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150502,
                column: "ACC_WKEND2_N",
                value: 20150502);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150509,
                column: "ACC_WKEND2_N",
                value: 20150509);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150516,
                column: "ACC_WKEND2_N",
                value: 20150516);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150523,
                column: "ACC_WKEND2_N",
                value: 20150523);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150530,
                column: "ACC_WKEND2_N",
                value: 20150530);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150606,
                column: "ACC_WKEND2_N",
                value: 20150606);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150613,
                column: "ACC_WKEND2_N",
                value: 20150613);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150620,
                column: "ACC_WKEND2_N",
                value: 20150620);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150627,
                column: "ACC_WKEND2_N",
                value: 20150627);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150704,
                column: "ACC_WKEND2_N",
                value: 20150704);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150711,
                column: "ACC_WKEND2_N",
                value: 20150711);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150718,
                column: "ACC_WKEND2_N",
                value: 20150718);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150725,
                column: "ACC_WKEND2_N",
                value: 20150725);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150801,
                column: "ACC_WKEND2_N",
                value: 20150801);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150808,
                column: "ACC_WKEND2_N",
                value: 20150808);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150815,
                column: "ACC_WKEND2_N",
                value: 20150815);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150822,
                column: "ACC_WKEND2_N",
                value: 20150822);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150829,
                column: "ACC_WKEND2_N",
                value: 20150829);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150905,
                column: "ACC_WKEND2_N",
                value: 20150905);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150912,
                column: "ACC_WKEND2_N",
                value: 20150912);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150919,
                column: "ACC_WKEND2_N",
                value: 20150919);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150926,
                column: "ACC_WKEND2_N",
                value: 20150926);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151003,
                column: "ACC_WKEND2_N",
                value: 20151003);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151010,
                column: "ACC_WKEND2_N",
                value: 20151010);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151017,
                column: "ACC_WKEND2_N",
                value: 20151017);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151024,
                column: "ACC_WKEND2_N",
                value: 20151024);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151031,
                column: "ACC_WKEND2_N",
                value: 20151031);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151107,
                column: "ACC_WKEND2_N",
                value: 20151107);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151114,
                column: "ACC_WKEND2_N",
                value: 20151114);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151121,
                column: "ACC_WKEND2_N",
                value: 20151121);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151128,
                column: "ACC_WKEND2_N",
                value: 20151128);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151205,
                column: "ACC_WKEND2_N",
                value: 20151205);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151212,
                column: "ACC_WKEND2_N",
                value: 20151212);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151219,
                column: "ACC_WKEND2_N",
                value: 20151219);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151226,
                column: "ACC_WKEND2_N",
                value: 20151226);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160102,
                column: "ACC_WKEND2_N",
                value: 20160102);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160109,
                column: "ACC_WKEND2_N",
                value: 20160109);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160116,
                column: "ACC_WKEND2_N",
                value: 20160116);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160123,
                column: "ACC_WKEND2_N",
                value: 20160123);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160130,
                column: "ACC_WKEND2_N",
                value: 20160130);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160206,
                column: "ACC_WKEND2_N",
                value: 20160206);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160213,
                column: "ACC_WKEND2_N",
                value: 20160213);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160220,
                column: "ACC_WKEND2_N",
                value: 20160220);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160227,
                column: "ACC_WKEND2_N",
                value: 20160227);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160305,
                column: "ACC_WKEND2_N",
                value: 20160305);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160312,
                column: "ACC_WKEND2_N",
                value: 20160312);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160319,
                column: "ACC_WKEND2_N",
                value: 20160319);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160326,
                column: "ACC_WKEND2_N",
                value: 20160326);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160402,
                column: "ACC_WKEND2_N",
                value: 20160402);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160409,
                column: "ACC_WKEND2_N",
                value: 20160409);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160416,
                column: "ACC_WKEND2_N",
                value: 20160416);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160423,
                column: "ACC_WKEND2_N",
                value: 20160423);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160430,
                column: "ACC_WKEND2_N",
                value: 20160430);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160507,
                column: "ACC_WKEND2_N",
                value: 20160507);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160514,
                column: "ACC_WKEND2_N",
                value: 20160514);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160521,
                column: "ACC_WKEND2_N",
                value: 20160521);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160528,
                column: "ACC_WKEND2_N",
                value: 20160528);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160604,
                column: "ACC_WKEND2_N",
                value: 20160604);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160611,
                column: "ACC_WKEND2_N",
                value: 20160611);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160618,
                column: "ACC_WKEND2_N",
                value: 20160618);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160625,
                column: "ACC_WKEND2_N",
                value: 20160625);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160702,
                column: "ACC_WKEND2_N",
                value: 20160702);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160709,
                column: "ACC_WKEND2_N",
                value: 20160709);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160716,
                column: "ACC_WKEND2_N",
                value: 20160716);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160723,
                column: "ACC_WKEND2_N",
                value: 20160723);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160730,
                column: "ACC_WKEND2_N",
                value: 20160730);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160806,
                column: "ACC_WKEND2_N",
                value: 20160806);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160813,
                column: "ACC_WKEND2_N",
                value: 20160813);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160820,
                column: "ACC_WKEND2_N",
                value: 20160820);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160827,
                column: "ACC_WKEND2_N",
                value: 20160827);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160903,
                column: "ACC_WKEND2_N",
                value: 20160903);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160910,
                column: "ACC_WKEND2_N",
                value: 20160910);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160917,
                column: "ACC_WKEND2_N",
                value: 20160917);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160924,
                column: "ACC_WKEND2_N",
                value: 20160924);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161001,
                column: "ACC_WKEND2_N",
                value: 20161001);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161008,
                column: "ACC_WKEND2_N",
                value: 20161008);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161015,
                column: "ACC_WKEND2_N",
                value: 20161015);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161022,
                column: "ACC_WKEND2_N",
                value: 20161022);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161029,
                column: "ACC_WKEND2_N",
                value: 20161029);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161105,
                column: "ACC_WKEND2_N",
                value: 20161105);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161112,
                column: "ACC_WKEND2_N",
                value: 20161112);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161119,
                column: "ACC_WKEND2_N",
                value: 20161119);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161126,
                column: "ACC_WKEND2_N",
                value: 20161126);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161203,
                column: "ACC_WKEND2_N",
                value: 20161203);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161210,
                column: "ACC_WKEND2_N",
                value: 20161210);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161217,
                column: "ACC_WKEND2_N",
                value: 20161217);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161224,
                column: "ACC_WKEND2_N",
                value: 20161224);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161231,
                column: "ACC_WKEND2_N",
                value: 20161231);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170107,
                column: "ACC_WKEND2_N",
                value: 20170107);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170114,
                column: "ACC_WKEND2_N",
                value: 20170114);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170121,
                column: "ACC_WKEND2_N",
                value: 20170121);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170128,
                column: "ACC_WKEND2_N",
                value: 20170128);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170204,
                column: "ACC_WKEND2_N",
                value: 20170204);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170211,
                column: "ACC_WKEND2_N",
                value: 20170211);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170218,
                column: "ACC_WKEND2_N",
                value: 20170218);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170225,
                column: "ACC_WKEND2_N",
                value: 20170225);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170304,
                column: "ACC_WKEND2_N",
                value: 20170304);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170311,
                column: "ACC_WKEND2_N",
                value: 20170311);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170318,
                column: "ACC_WKEND2_N",
                value: 20170318);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170325,
                column: "ACC_WKEND2_N",
                value: 20170325);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170401,
                column: "ACC_WKEND2_N",
                value: 20170401);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170408,
                column: "ACC_WKEND2_N",
                value: 20170408);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170415,
                column: "ACC_WKEND2_N",
                value: 20170415);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170422,
                column: "ACC_WKEND2_N",
                value: 20170422);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170429,
                column: "ACC_WKEND2_N",
                value: 20170429);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170506,
                column: "ACC_WKEND2_N",
                value: 20170506);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170513,
                column: "ACC_WKEND2_N",
                value: 20170513);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170520,
                column: "ACC_WKEND2_N",
                value: 20170520);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170527,
                column: "ACC_WKEND2_N",
                value: 20170527);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170603,
                column: "ACC_WKEND2_N",
                value: 20170603);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170610,
                column: "ACC_WKEND2_N",
                value: 20170610);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170617,
                column: "ACC_WKEND2_N",
                value: 20170617);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170624,
                column: "ACC_WKEND2_N",
                value: 20170624);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170701,
                column: "ACC_WKEND2_N",
                value: 20170701);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170708,
                column: "ACC_WKEND2_N",
                value: 20170708);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170715,
                column: "ACC_WKEND2_N",
                value: 20170715);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170722,
                column: "ACC_WKEND2_N",
                value: 20170722);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170729,
                column: "ACC_WKEND2_N",
                value: 20170729);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170805,
                column: "ACC_WKEND2_N",
                value: 20170805);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170812,
                column: "ACC_WKEND2_N",
                value: 20170812);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170819,
                column: "ACC_WKEND2_N",
                value: 20170819);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170826,
                column: "ACC_WKEND2_N",
                value: 20170826);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170902,
                column: "ACC_WKEND2_N",
                value: 20170902);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170909,
                column: "ACC_WKEND2_N",
                value: 20170909);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170916,
                column: "ACC_WKEND2_N",
                value: 20170916);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170923,
                column: "ACC_WKEND2_N",
                value: 20170923);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170930,
                column: "ACC_WKEND2_N",
                value: 20170930);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171007,
                column: "ACC_WKEND2_N",
                value: 20171007);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171014,
                column: "ACC_WKEND2_N",
                value: 20171014);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171021,
                column: "ACC_WKEND2_N",
                value: 20171021);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171028,
                column: "ACC_WKEND2_N",
                value: 20171028);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171104,
                column: "ACC_WKEND2_N",
                value: 20171104);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171111,
                column: "ACC_WKEND2_N",
                value: 20171111);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171118,
                column: "ACC_WKEND2_N",
                value: 20171118);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171125,
                column: "ACC_WKEND2_N",
                value: 20171125);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171202,
                column: "ACC_WKEND2_N",
                value: 20171202);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171209,
                column: "ACC_WKEND2_N",
                value: 20171209);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171216,
                column: "ACC_WKEND2_N",
                value: 20171216);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171223,
                column: "ACC_WKEND2_N",
                value: 20171223);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171230,
                column: "ACC_WKEND2_N",
                value: 20171230);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180106,
                column: "ACC_WKEND2_N",
                value: 20180106);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180113,
                column: "ACC_WKEND2_N",
                value: 20180113);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180120,
                column: "ACC_WKEND2_N",
                value: 20180120);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180127,
                column: "ACC_WKEND2_N",
                value: 20180127);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180203,
                column: "ACC_WKEND2_N",
                value: 20180203);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180210,
                column: "ACC_WKEND2_N",
                value: 20180210);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180217,
                column: "ACC_WKEND2_N",
                value: 20180217);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180224,
                column: "ACC_WKEND2_N",
                value: 20180224);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180303,
                column: "ACC_WKEND2_N",
                value: 20180303);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180310,
                column: "ACC_WKEND2_N",
                value: 20180310);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180317,
                column: "ACC_WKEND2_N",
                value: 20180317);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180324,
                column: "ACC_WKEND2_N",
                value: 20180324);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180331,
                column: "ACC_WKEND2_N",
                value: 20180331);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180407,
                column: "ACC_WKEND2_N",
                value: 20180407);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180414,
                column: "ACC_WKEND2_N",
                value: 20180414);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180421,
                column: "ACC_WKEND2_N",
                value: 20180421);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180428,
                column: "ACC_WKEND2_N",
                value: 20180428);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180505,
                column: "ACC_WKEND2_N",
                value: 20180505);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180512,
                column: "ACC_WKEND2_N",
                value: 20180512);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180519,
                column: "ACC_WKEND2_N",
                value: 20180519);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180526,
                column: "ACC_WKEND2_N",
                value: 20180526);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180602,
                column: "ACC_WKEND2_N",
                value: 20180602);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180609,
                column: "ACC_WKEND2_N",
                value: 20180609);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180616,
                column: "ACC_WKEND2_N",
                value: 20180616);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180623,
                column: "ACC_WKEND2_N",
                value: 20180623);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180630,
                column: "ACC_WKEND2_N",
                value: 20180630);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180707,
                column: "ACC_WKEND2_N",
                value: 20180707);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180714,
                column: "ACC_WKEND2_N",
                value: 20180714);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180721,
                column: "ACC_WKEND2_N",
                value: 20180721);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180728,
                column: "ACC_WKEND2_N",
                value: 20180728);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180804,
                column: "ACC_WKEND2_N",
                value: 20180804);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180811,
                column: "ACC_WKEND2_N",
                value: 20180811);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180818,
                column: "ACC_WKEND2_N",
                value: 20180818);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180825,
                column: "ACC_WKEND2_N",
                value: 20180825);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180901,
                column: "ACC_WKEND2_N",
                value: 20180901);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180908,
                column: "ACC_WKEND2_N",
                value: 20180908);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180915,
                column: "ACC_WKEND2_N",
                value: 20180915);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180922,
                column: "ACC_WKEND2_N",
                value: 20180922);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180929,
                column: "ACC_WKEND2_N",
                value: 20180929);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181006,
                column: "ACC_WKEND2_N",
                value: 20181006);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181013,
                column: "ACC_WKEND2_N",
                value: 20181013);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181020,
                column: "ACC_WKEND2_N",
                value: 20181020);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181027,
                column: "ACC_WKEND2_N",
                value: 20181027);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181103,
                column: "ACC_WKEND2_N",
                value: 20181103);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181110,
                column: "ACC_WKEND2_N",
                value: 20181110);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181117,
                column: "ACC_WKEND2_N",
                value: 20181117);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181124,
                column: "ACC_WKEND2_N",
                value: 20181124);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181201,
                column: "ACC_WKEND2_N",
                value: 20181201);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181208,
                column: "ACC_WKEND2_N",
                value: 20181208);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181215,
                column: "ACC_WKEND2_N",
                value: 20181215);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181222,
                column: "ACC_WKEND2_N",
                value: 20181222);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181229,
                column: "ACC_WKEND2_N",
                value: 20181229);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190105,
                column: "ACC_WKEND2_N",
                value: 20190105);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190112,
                column: "ACC_WKEND2_N",
                value: 20190112);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190119,
                column: "ACC_WKEND2_N",
                value: 20190119);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190126,
                column: "ACC_WKEND2_N",
                value: 20190126);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190202,
                column: "ACC_WKEND2_N",
                value: 20190202);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190209,
                column: "ACC_WKEND2_N",
                value: 20190209);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190216,
                column: "ACC_WKEND2_N",
                value: 20190216);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190223,
                column: "ACC_WKEND2_N",
                value: 20190223);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190302,
                column: "ACC_WKEND2_N",
                value: 20190302);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190309,
                column: "ACC_WKEND2_N",
                value: 20190309);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190316,
                column: "ACC_WKEND2_N",
                value: 20190316);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190323,
                column: "ACC_WKEND2_N",
                value: 20190323);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190330,
                column: "ACC_WKEND2_N",
                value: 20190330);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190406,
                column: "ACC_WKEND2_N",
                value: 20190406);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190413,
                column: "ACC_WKEND2_N",
                value: 20190413);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190420,
                column: "ACC_WKEND2_N",
                value: 20190420);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190427,
                column: "ACC_WKEND2_N",
                value: 20190427);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190504,
                column: "ACC_WKEND2_N",
                value: 20190504);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190511,
                column: "ACC_WKEND2_N",
                value: 20190511);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190518,
                column: "ACC_WKEND2_N",
                value: 20190518);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190525,
                column: "ACC_WKEND2_N",
                value: 20190525);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190601,
                column: "ACC_WKEND2_N",
                value: 20190601);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190608,
                column: "ACC_WKEND2_N",
                value: 20190608);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190615,
                column: "ACC_WKEND2_N",
                value: 20190615);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190622,
                column: "ACC_WKEND2_N",
                value: 20190622);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190629,
                column: "ACC_WKEND2_N",
                value: 20190629);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190706,
                column: "ACC_WKEND2_N",
                value: 20190706);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190713,
                column: "ACC_WKEND2_N",
                value: 20190713);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190720,
                column: "ACC_WKEND2_N",
                value: 20190720);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190727,
                column: "ACC_WKEND2_N",
                value: 20190727);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190803,
                column: "ACC_WKEND2_N",
                value: 20190803);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190810,
                column: "ACC_WKEND2_N",
                value: 20190810);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190817,
                column: "ACC_WKEND2_N",
                value: 20190817);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190824,
                column: "ACC_WKEND2_N",
                value: 20190824);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190831,
                column: "ACC_WKEND2_N",
                value: 20190831);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190907,
                column: "ACC_WKEND2_N",
                value: 20190907);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190914,
                column: "ACC_WKEND2_N",
                value: 20190914);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190921,
                column: "ACC_WKEND2_N",
                value: 20190921);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190928,
                column: "ACC_WKEND2_N",
                value: 20190928);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191005,
                column: "ACC_WKEND2_N",
                value: 20191005);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191012,
                column: "ACC_WKEND2_N",
                value: 20191012);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191019,
                column: "ACC_WKEND2_N",
                value: 20191019);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191026,
                column: "ACC_WKEND2_N",
                value: 20191026);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191102,
                column: "ACC_WKEND2_N",
                value: 20191102);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191109,
                column: "ACC_WKEND2_N",
                value: 20191109);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191116,
                column: "ACC_WKEND2_N",
                value: 20191116);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191123,
                column: "ACC_WKEND2_N",
                value: 20191123);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191130,
                column: "ACC_WKEND2_N",
                value: 20191130);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191207,
                column: "ACC_WKEND2_N",
                value: 20191207);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191214,
                column: "ACC_WKEND2_N",
                value: 20191214);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191221,
                column: "ACC_WKEND2_N",
                value: 20191221);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191228,
                column: "ACC_WKEND2_N",
                value: 20191228);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200104,
                column: "ACC_WKEND2_N",
                value: 20200104);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200111,
                column: "ACC_WKEND2_N",
                value: 20200111);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200118,
                column: "ACC_WKEND2_N",
                value: 20200118);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200125,
                column: "ACC_WKEND2_N",
                value: 20200125);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200201,
                column: "ACC_WKEND2_N",
                value: 20200201);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200208,
                column: "ACC_WKEND2_N",
                value: 20200208);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200215,
                column: "ACC_WKEND2_N",
                value: 20200215);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200222,
                column: "ACC_WKEND2_N",
                value: 20200222);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200229,
                column: "ACC_WKEND2_N",
                value: 20200229);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200307,
                column: "ACC_WKEND2_N",
                value: 20200307);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200314,
                column: "ACC_WKEND2_N",
                value: 20200314);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200321,
                column: "ACC_WKEND2_N",
                value: 20200321);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200328,
                column: "ACC_WKEND2_N",
                value: 20200328);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200404,
                column: "ACC_WKEND2_N",
                value: 20200404);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200411,
                column: "ACC_WKEND2_N",
                value: 20200411);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200418,
                column: "ACC_WKEND2_N",
                value: 20200418);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200425,
                column: "ACC_WKEND2_N",
                value: 20200425);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200502,
                column: "ACC_WKEND2_N",
                value: 20200502);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200509,
                column: "ACC_WKEND2_N",
                value: 20200509);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200516,
                column: "ACC_WKEND2_N",
                value: 20200516);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200523,
                column: "ACC_WKEND2_N",
                value: 20200523);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200530,
                column: "ACC_WKEND2_N",
                value: 20200530);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200606,
                column: "ACC_WKEND2_N",
                value: 20200606);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200613,
                column: "ACC_WKEND2_N",
                value: 20200613);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200620,
                column: "ACC_WKEND2_N",
                value: 20200620);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200627,
                column: "ACC_WKEND2_N",
                value: 20200627);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200704,
                column: "ACC_WKEND2_N",
                value: 20200704);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200711,
                column: "ACC_WKEND2_N",
                value: 20200711);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200718,
                column: "ACC_WKEND2_N",
                value: 20200718);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200725,
                column: "ACC_WKEND2_N",
                value: 20200725);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200801,
                column: "ACC_WKEND2_N",
                value: 20200801);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200808,
                column: "ACC_WKEND2_N",
                value: 20200808);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200815,
                column: "ACC_WKEND2_N",
                value: 20200815);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200822,
                column: "ACC_WKEND2_N",
                value: 20200822);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200829,
                column: "ACC_WKEND2_N",
                value: 20200829);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200905,
                column: "ACC_WKEND2_N",
                value: 20200905);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200912,
                column: "ACC_WKEND2_N",
                value: 20200912);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200919,
                column: "ACC_WKEND2_N",
                value: 20200919);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200926,
                column: "ACC_WKEND2_N",
                value: 20200926);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201003,
                column: "ACC_WKEND2_N",
                value: 20201003);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201010,
                column: "ACC_WKEND2_N",
                value: 20201010);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201017,
                column: "ACC_WKEND2_N",
                value: 20201017);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201024,
                column: "ACC_WKEND2_N",
                value: 20201024);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201031,
                column: "ACC_WKEND2_N",
                value: 20201031);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201107,
                column: "ACC_WKEND2_N",
                value: 20201107);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201114,
                column: "ACC_WKEND2_N",
                value: 20201114);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201121,
                column: "ACC_WKEND2_N",
                value: 20201121);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201128,
                column: "ACC_WKEND2_N",
                value: 20201128);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201205,
                column: "ACC_WKEND2_N",
                value: 20201205);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201212,
                column: "ACC_WKEND2_N",
                value: 20201212);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201219,
                column: "ACC_WKEND2_N",
                value: 20201219);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201226,
                column: "ACC_WKEND2_N",
                value: 20201226);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210102,
                column: "ACC_WKEND2_N",
                value: 20210102);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210109,
                column: "ACC_WKEND2_N",
                value: 20210109);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210116,
                column: "ACC_WKEND2_N",
                value: 20210116);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210123,
                column: "ACC_WKEND2_N",
                value: 20210123);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210130,
                column: "ACC_WKEND2_N",
                value: 20210130);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210206,
                column: "ACC_WKEND2_N",
                value: 20210206);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210213,
                column: "ACC_WKEND2_N",
                value: 20210213);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210220,
                column: "ACC_WKEND2_N",
                value: 20210220);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210227,
                column: "ACC_WKEND2_N",
                value: 20210227);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210306,
                column: "ACC_WKEND2_N",
                value: 20210306);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210313,
                column: "ACC_WKEND2_N",
                value: 20210313);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210320,
                column: "ACC_WKEND2_N",
                value: 20210320);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210327,
                column: "ACC_WKEND2_N",
                value: 20210327);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210403,
                column: "ACC_WKEND2_N",
                value: 20210403);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210410,
                column: "ACC_WKEND2_N",
                value: 20210410);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210417,
                column: "ACC_WKEND2_N",
                value: 20210417);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210424,
                column: "ACC_WKEND2_N",
                value: 20210424);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210501,
                column: "ACC_WKEND2_N",
                value: 20210501);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210508,
                column: "ACC_WKEND2_N",
                value: 20210508);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210515,
                column: "ACC_WKEND2_N",
                value: 20210515);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210522,
                column: "ACC_WKEND2_N",
                value: 20210522);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210529,
                column: "ACC_WKEND2_N",
                value: 20210529);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210605,
                column: "ACC_WKEND2_N",
                value: 20210605);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210612,
                column: "ACC_WKEND2_N",
                value: 20210612);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210619,
                column: "ACC_WKEND2_N",
                value: 20210619);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210626,
                column: "ACC_WKEND2_N",
                value: 20210626);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210703,
                column: "ACC_WKEND2_N",
                value: 20210703);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210710,
                column: "ACC_WKEND2_N",
                value: 20210710);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210717,
                column: "ACC_WKEND2_N",
                value: 20210717);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210724,
                column: "ACC_WKEND2_N",
                value: 20210724);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210731,
                column: "ACC_WKEND2_N",
                value: 20210731);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210807,
                column: "ACC_WKEND2_N",
                value: 20210807);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210814,
                column: "ACC_WKEND2_N",
                value: 20210814);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210821,
                column: "ACC_WKEND2_N",
                value: 20210821);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210828,
                column: "ACC_WKEND2_N",
                value: 20210828);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210904,
                column: "ACC_WKEND2_N",
                value: 20210904);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210911,
                column: "ACC_WKEND2_N",
                value: 20210911);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210918,
                column: "ACC_WKEND2_N",
                value: 20210918);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210925,
                column: "ACC_WKEND2_N",
                value: 20210925);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211002,
                column: "ACC_WKEND2_N",
                value: 20211002);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211009,
                column: "ACC_WKEND2_N",
                value: 20211009);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211016,
                column: "ACC_WKEND2_N",
                value: 20211016);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211023,
                column: "ACC_WKEND2_N",
                value: 20211023);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211030,
                column: "ACC_WKEND2_N",
                value: 20211030);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211106,
                column: "ACC_WKEND2_N",
                value: 20211106);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211113,
                column: "ACC_WKEND2_N",
                value: 20211113);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211120,
                column: "ACC_WKEND2_N",
                value: 20211120);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211127,
                column: "ACC_WKEND2_N",
                value: 20211127);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211204,
                column: "ACC_WKEND2_N",
                value: 20211204);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211211,
                column: "ACC_WKEND2_N",
                value: 20211211);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211218,
                column: "ACC_WKEND2_N",
                value: 20211218);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211225,
                column: "ACC_WKEND2_N",
                value: 20211225);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220101,
                column: "ACC_WKEND2_N",
                value: 20220101);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220108,
                column: "ACC_WKEND2_N",
                value: 20220108);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220115,
                column: "ACC_WKEND2_N",
                value: 20220115);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220122,
                column: "ACC_WKEND2_N",
                value: 20220122);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220129,
                column: "ACC_WKEND2_N",
                value: 20220129);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220205,
                column: "ACC_WKEND2_N",
                value: 20220205);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220212,
                column: "ACC_WKEND2_N",
                value: 20220212);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220219,
                column: "ACC_WKEND2_N",
                value: 20220219);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220226,
                column: "ACC_WKEND2_N",
                value: 20220226);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220305,
                column: "ACC_WKEND2_N",
                value: 20220305);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220312,
                column: "ACC_WKEND2_N",
                value: 20220312);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220319,
                column: "ACC_WKEND2_N",
                value: 20220319);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220326,
                column: "ACC_WKEND2_N",
                value: 20220326);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220402,
                column: "ACC_WKEND2_N",
                value: 20220402);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220409,
                column: "ACC_WKEND2_N",
                value: 20220409);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220416,
                column: "ACC_WKEND2_N",
                value: 20220416);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220423,
                column: "ACC_WKEND2_N",
                value: 20220423);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220430,
                column: "ACC_WKEND2_N",
                value: 20220430);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220507,
                column: "ACC_WKEND2_N",
                value: 20220507);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220514,
                column: "ACC_WKEND2_N",
                value: 20220514);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220521,
                column: "ACC_WKEND2_N",
                value: 20220521);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220528,
                column: "ACC_WKEND2_N",
                value: 20220528);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220604,
                column: "ACC_WKEND2_N",
                value: 20220604);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220611,
                column: "ACC_WKEND2_N",
                value: 20220611);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220618,
                column: "ACC_WKEND2_N",
                value: 20220618);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220625,
                column: "ACC_WKEND2_N",
                value: 20220625);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220702,
                column: "ACC_WKEND2_N",
                value: 20220702);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220709,
                column: "ACC_WKEND2_N",
                value: 20220709);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220716,
                column: "ACC_WKEND2_N",
                value: 20220716);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220723,
                column: "ACC_WKEND2_N",
                value: 20220723);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220730,
                column: "ACC_WKEND2_N",
                value: 20220730);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220806,
                column: "ACC_WKEND2_N",
                value: 20220806);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220813,
                column: "ACC_WKEND2_N",
                value: 20220813);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220820,
                column: "ACC_WKEND2_N",
                value: 20220820);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220827,
                column: "ACC_WKEND2_N",
                value: 20220827);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220903,
                column: "ACC_WKEND2_N",
                value: 20220903);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220910,
                column: "ACC_WKEND2_N",
                value: 20220910);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220917,
                column: "ACC_WKEND2_N",
                value: 20220917);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220924,
                column: "ACC_WKEND2_N",
                value: 20220924);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221001,
                column: "ACC_WKEND2_N",
                value: 20221001);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221008,
                column: "ACC_WKEND2_N",
                value: 20221008);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221015,
                column: "ACC_WKEND2_N",
                value: 20221015);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221022,
                column: "ACC_WKEND2_N",
                value: 20221022);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221029,
                column: "ACC_WKEND2_N",
                value: 20221029);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221105,
                column: "ACC_WKEND2_N",
                value: 20221105);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221112,
                column: "ACC_WKEND2_N",
                value: 20221112);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221119,
                column: "ACC_WKEND2_N",
                value: 20221119);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221126,
                column: "ACC_WKEND2_N",
                value: 20221126);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221203,
                column: "ACC_WKEND2_N",
                value: 20221203);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221210,
                column: "ACC_WKEND2_N",
                value: 20221210);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221217,
                column: "ACC_WKEND2_N",
                value: 20221217);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221224,
                column: "ACC_WKEND2_N",
                value: 20221224);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221231,
                column: "ACC_WKEND2_N",
                value: 20221231);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230107,
                column: "ACC_WKEND2_N",
                value: 20230107);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230114,
                column: "ACC_WKEND2_N",
                value: 20230114);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230121,
                column: "ACC_WKEND2_N",
                value: 20230121);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230128,
                column: "ACC_WKEND2_N",
                value: 20230128);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230204,
                column: "ACC_WKEND2_N",
                value: 20230204);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230211,
                column: "ACC_WKEND2_N",
                value: 20230211);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230218,
                column: "ACC_WKEND2_N",
                value: 20230218);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230225,
                column: "ACC_WKEND2_N",
                value: 20230225);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230304,
                column: "ACC_WKEND2_N",
                value: 20230304);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230311,
                column: "ACC_WKEND2_N",
                value: 20230311);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230318,
                column: "ACC_WKEND2_N",
                value: 20230318);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230325,
                column: "ACC_WKEND2_N",
                value: 20230325);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230401,
                column: "ACC_WKEND2_N",
                value: 20230401);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230408,
                column: "ACC_WKEND2_N",
                value: 20230408);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230415,
                column: "ACC_WKEND2_N",
                value: 20230415);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230422,
                column: "ACC_WKEND2_N",
                value: 20230422);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230429,
                column: "ACC_WKEND2_N",
                value: 20230429);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230506,
                column: "ACC_WKEND2_N",
                value: 20230506);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230513,
                column: "ACC_WKEND2_N",
                value: 20230513);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230520,
                column: "ACC_WKEND2_N",
                value: 20230520);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230527,
                column: "ACC_WKEND2_N",
                value: 20230527);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230603,
                column: "ACC_WKEND2_N",
                value: 20230603);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230610,
                column: "ACC_WKEND2_N",
                value: 20230610);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230617,
                column: "ACC_WKEND2_N",
                value: 20230617);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230624,
                column: "ACC_WKEND2_N",
                value: 20230624);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230701,
                column: "ACC_WKEND2_N",
                value: 20230701);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230708,
                column: "ACC_WKEND2_N",
                value: 20230708);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230715,
                column: "ACC_WKEND2_N",
                value: 20230715);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230722,
                column: "ACC_WKEND2_N",
                value: 20230722);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230729,
                column: "ACC_WKEND2_N",
                value: 20230729);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230805,
                column: "ACC_WKEND2_N",
                value: 20230805);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230812,
                column: "ACC_WKEND2_N",
                value: 20230812);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230819,
                column: "ACC_WKEND2_N",
                value: 20230819);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230826,
                column: "ACC_WKEND2_N",
                value: 20230826);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230902,
                column: "ACC_WKEND2_N",
                value: 20230902);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230909,
                column: "ACC_WKEND2_N",
                value: 20230909);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230916,
                column: "ACC_WKEND2_N",
                value: 20230916);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230923,
                column: "ACC_WKEND2_N",
                value: 20230923);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230930,
                column: "ACC_WKEND2_N",
                value: 20230930);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231007,
                column: "ACC_WKEND2_N",
                value: 20231007);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231014,
                column: "ACC_WKEND2_N",
                value: 20231014);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231021,
                column: "ACC_WKEND2_N",
                value: 20231021);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231028,
                column: "ACC_WKEND2_N",
                value: 20231028);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231104,
                column: "ACC_WKEND2_N",
                value: 20231104);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231111,
                column: "ACC_WKEND2_N",
                value: 20231111);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231118,
                column: "ACC_WKEND2_N",
                value: 20231118);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231125,
                column: "ACC_WKEND2_N",
                value: 20231125);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231202,
                column: "ACC_WKEND2_N",
                value: 20231202);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231209,
                column: "ACC_WKEND2_N",
                value: 20231209);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231216,
                column: "ACC_WKEND2_N",
                value: 20231216);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231223,
                column: "ACC_WKEND2_N",
                value: 20231223);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231230,
                column: "ACC_WKEND2_N",
                value: 20231230);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240106,
                column: "ACC_WKEND2_N",
                value: 20240106);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240113,
                column: "ACC_WKEND2_N",
                value: 20240113);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240120,
                column: "ACC_WKEND2_N",
                value: 20240120);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240127,
                column: "ACC_WKEND2_N",
                value: 20240127);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240203,
                column: "ACC_WKEND2_N",
                value: 20240203);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240210,
                column: "ACC_WKEND2_N",
                value: 20240210);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240217,
                column: "ACC_WKEND2_N",
                value: 20240217);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240224,
                column: "ACC_WKEND2_N",
                value: 20240224);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240302,
                column: "ACC_WKEND2_N",
                value: 20240302);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240309,
                column: "ACC_WKEND2_N",
                value: 20240309);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240316,
                column: "ACC_WKEND2_N",
                value: 20240316);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240323,
                column: "ACC_WKEND2_N",
                value: 20240323);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240330,
                column: "ACC_WKEND2_N",
                value: 20240330);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240406,
                column: "ACC_WKEND2_N",
                value: 20240406);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240413,
                column: "ACC_WKEND2_N",
                value: 20240413);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240420,
                column: "ACC_WKEND2_N",
                value: 20240420);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240427,
                column: "ACC_WKEND2_N",
                value: 20240427);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240504,
                column: "ACC_WKEND2_N",
                value: 20240504);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240511,
                column: "ACC_WKEND2_N",
                value: 20240511);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240518,
                column: "ACC_WKEND2_N",
                value: 20240518);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240525,
                column: "ACC_WKEND2_N",
                value: 20240525);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240601,
                column: "ACC_WKEND2_N",
                value: 20240601);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240608,
                column: "ACC_WKEND2_N",
                value: 20240608);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240615,
                column: "ACC_WKEND2_N",
                value: 20240615);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240622,
                column: "ACC_WKEND2_N",
                value: 20240622);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240629,
                column: "ACC_WKEND2_N",
                value: 20240629);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240706,
                column: "ACC_WKEND2_N",
                value: 20240706);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240713,
                column: "ACC_WKEND2_N",
                value: 20240713);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240720,
                column: "ACC_WKEND2_N",
                value: 20240720);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240727,
                column: "ACC_WKEND2_N",
                value: 20240727);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240803,
                column: "ACC_WKEND2_N",
                value: 20240803);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240810,
                column: "ACC_WKEND2_N",
                value: 20240810);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240817,
                column: "ACC_WKEND2_N",
                value: 20240817);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240824,
                column: "ACC_WKEND2_N",
                value: 20240824);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240831,
                column: "ACC_WKEND2_N",
                value: 20240831);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240907,
                column: "ACC_WKEND2_N",
                value: 20240907);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240914,
                column: "ACC_WKEND2_N",
                value: 20240914);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240921,
                column: "ACC_WKEND2_N",
                value: 20240921);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240928,
                column: "ACC_WKEND2_N",
                value: 20240928);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241005,
                column: "ACC_WKEND2_N",
                value: 20241005);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241012,
                column: "ACC_WKEND2_N",
                value: 20241012);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241019,
                column: "ACC_WKEND2_N",
                value: 20241019);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241026,
                column: "ACC_WKEND2_N",
                value: 20241026);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241102,
                column: "ACC_WKEND2_N",
                value: 20241102);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241109,
                column: "ACC_WKEND2_N",
                value: 20241109);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241116,
                column: "ACC_WKEND2_N",
                value: 20241116);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241123,
                column: "ACC_WKEND2_N",
                value: 20241123);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241130,
                column: "ACC_WKEND2_N",
                value: 20241130);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241207,
                column: "ACC_WKEND2_N",
                value: 20241207);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241214,
                column: "ACC_WKEND2_N",
                value: 20241214);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241221,
                column: "ACC_WKEND2_N",
                value: 20241221);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241228,
                column: "ACC_WKEND2_N",
                value: 20241228);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990109,
                column: "ACC_WKEND2_N",
                value: 19990109);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990116,
                column: "ACC_WKEND2_N",
                value: 19990116);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990123,
                column: "ACC_WKEND2_N",
                value: 19990123);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990130,
                column: "ACC_WKEND2_N",
                value: 19990130);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990206,
                column: "ACC_WKEND2_N",
                value: 19990206);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990213,
                column: "ACC_WKEND2_N",
                value: 19990213);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990220,
                column: "ACC_WKEND2_N",
                value: 19990220);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990227,
                column: "ACC_WKEND2_N",
                value: 19990227);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990306,
                column: "ACC_WKEND2_N",
                value: 19990306);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990313,
                column: "ACC_WKEND2_N",
                value: 19990313);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990320,
                column: "ACC_WKEND2_N",
                value: 19990320);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990327,
                column: "ACC_WKEND2_N",
                value: 19990327);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990403,
                column: "ACC_WKEND2_N",
                value: 19990403);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990410,
                column: "ACC_WKEND2_N",
                value: 19990410);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990417,
                column: "ACC_WKEND2_N",
                value: 19990417);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990424,
                column: "ACC_WKEND2_N",
                value: 19990424);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990501,
                column: "ACC_WKEND2_N",
                value: 19990501);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990508,
                column: "ACC_WKEND2_N",
                value: 19990508);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990515,
                column: "ACC_WKEND2_N",
                value: 19990515);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990522,
                column: "ACC_WKEND2_N",
                value: 19990522);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990529,
                column: "ACC_WKEND2_N",
                value: 19990529);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990605,
                column: "ACC_WKEND2_N",
                value: 19990605);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990612,
                column: "ACC_WKEND2_N",
                value: 19990612);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990619,
                column: "ACC_WKEND2_N",
                value: 19990619);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990626,
                column: "ACC_WKEND2_N",
                value: 19990626);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990703,
                column: "ACC_WKEND2_N",
                value: 19990703);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990710,
                column: "ACC_WKEND2_N",
                value: 19990710);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990717,
                column: "ACC_WKEND2_N",
                value: 19990717);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990724,
                column: "ACC_WKEND2_N",
                value: 19990724);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990731,
                column: "ACC_WKEND2_N",
                value: 19990731);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990807,
                column: "ACC_WKEND2_N",
                value: 19990807);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990814,
                column: "ACC_WKEND2_N",
                value: 19990814);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990821,
                column: "ACC_WKEND2_N",
                value: 19990821);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990828,
                column: "ACC_WKEND2_N",
                value: 19990828);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990904,
                column: "ACC_WKEND2_N",
                value: 19990904);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990911,
                column: "ACC_WKEND2_N",
                value: 19990911);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990918,
                column: "ACC_WKEND2_N",
                value: 19990918);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990925,
                column: "ACC_WKEND2_N",
                value: 19990925);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991002,
                column: "ACC_WKEND2_N",
                value: 19991002);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991009,
                column: "ACC_WKEND2_N",
                value: 19991009);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991016,
                column: "ACC_WKEND2_N",
                value: 19991016);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991023,
                column: "ACC_WKEND2_N",
                value: 19991023);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991030,
                column: "ACC_WKEND2_N",
                value: 19991030);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991106,
                column: "ACC_WKEND2_N",
                value: 19991106);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991113,
                column: "ACC_WKEND2_N",
                value: 19991113);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991120,
                column: "ACC_WKEND2_N",
                value: 19991120);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991127,
                column: "ACC_WKEND2_N",
                value: 19991127);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991204,
                column: "ACC_WKEND2_N",
                value: 19991204);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991211,
                column: "ACC_WKEND2_N",
                value: 19991211);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991218,
                column: "ACC_WKEND2_N",
                value: 19991218);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991225,
                column: "ACC_WKEND2_N",
                value: 19991225);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AD",
                column: "ID",
                value: (short)4);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AE",
                column: "ID",
                value: (short)184);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AF",
                column: "ID",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AG",
                column: "ID",
                value: (short)6);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AL",
                column: "ID",
                value: (short)2);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AM",
                column: "ID",
                value: (short)8);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AO",
                column: "ID",
                value: (short)5);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AR",
                column: "ID",
                value: (short)7);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AT",
                column: "ID",
                value: (short)10);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AU",
                column: "ID",
                value: (short)9);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AZ",
                column: "ID",
                value: (short)11);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BA",
                column: "ID",
                value: (short)22);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BB",
                column: "ID",
                value: (short)15);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BD",
                column: "ID",
                value: (short)14);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BE",
                column: "ID",
                value: (short)17);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BF",
                column: "ID",
                value: (short)27);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BG",
                column: "ID",
                value: (short)26);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BH",
                column: "ID",
                value: (short)13);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BI",
                column: "ID",
                value: (short)28);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BJ",
                column: "ID",
                value: (short)19);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BN",
                column: "ID",
                value: (short)25);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BO",
                column: "ID",
                value: (short)21);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BR",
                column: "ID",
                value: (short)24);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BS",
                column: "ID",
                value: (short)12);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BT",
                column: "ID",
                value: (short)20);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BW",
                column: "ID",
                value: (short)23);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BY",
                column: "ID",
                value: (short)16);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BZ",
                column: "ID",
                value: (short)18);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CA",
                column: "ID",
                value: (short)32);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CD",
                column: "ID",
                value: (short)45);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CF",
                column: "ID",
                value: (short)33);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CG",
                column: "ID",
                value: (short)39);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CH",
                column: "ID",
                value: (short)168);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CL",
                column: "ID",
                value: (short)35);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CM",
                column: "ID",
                value: (short)31);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CN",
                column: "ID",
                value: (short)36);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CO",
                column: "ID",
                value: (short)37);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CR",
                column: "ID",
                value: (short)40);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CU",
                column: "ID",
                value: (short)42);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CV",
                column: "ID",
                value: (short)29);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CY",
                column: "ID",
                value: (short)43);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CZ",
                column: "ID",
                value: (short)44);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DE",
                column: "ID",
                value: (short)64);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DJ",
                column: "ID",
                value: (short)47);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DK",
                column: "ID",
                value: (short)46);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DM",
                column: "ID",
                value: (short)48);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DO",
                column: "ID",
                value: (short)49);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DZ",
                column: "ID",
                value: (short)3);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EC",
                column: "ID",
                value: (short)50);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EE",
                column: "ID",
                value: (short)55);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EG",
                column: "ID",
                value: (short)51);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ER",
                column: "ID",
                value: (short)54);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ES",
                column: "ID",
                value: (short)163);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ET",
                column: "ID",
                value: (short)57);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FI",
                column: "ID",
                value: (short)59);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FJ",
                column: "ID",
                value: (short)58);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FM",
                column: "ID",
                value: (short)110);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FR",
                column: "ID",
                value: (short)60);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GA",
                column: "ID",
                value: (short)61);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GB",
                column: "ID",
                value: (short)185);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GD",
                column: "ID",
                value: (short)67);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GE",
                column: "ID",
                value: (short)63);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GH",
                column: "ID",
                value: (short)65);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GM",
                column: "ID",
                value: (short)62);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GN",
                column: "ID",
                value: (short)69);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GQ",
                column: "ID",
                value: (short)53);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GR",
                column: "ID",
                value: (short)66);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GT",
                column: "ID",
                value: (short)68);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GW",
                column: "ID",
                value: (short)70);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GY",
                column: "ID",
                value: (short)71);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HN",
                column: "ID",
                value: (short)73);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HR",
                column: "ID",
                value: (short)41);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HT",
                column: "ID",
                value: (short)72);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HU",
                column: "ID",
                value: (short)74);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ID",
                column: "ID",
                value: (short)77);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IE",
                column: "ID",
                value: (short)80);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IL",
                column: "ID",
                value: (short)81);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IN",
                column: "ID",
                value: (short)76);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IQ",
                column: "ID",
                value: (short)79);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IR",
                column: "ID",
                value: (short)78);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IS",
                column: "ID",
                value: (short)75);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IT",
                column: "ID",
                value: (short)82);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JM",
                column: "ID",
                value: (short)83);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JO",
                column: "ID",
                value: (short)85);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JP",
                column: "ID",
                value: (short)84);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KE",
                column: "ID",
                value: (short)87);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KG",
                column: "ID",
                value: (short)90);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KH",
                column: "ID",
                value: (short)30);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KI",
                column: "ID",
                value: (short)88);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KM",
                column: "ID",
                value: (short)38);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KN",
                column: "ID",
                value: (short)144);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KP",
                column: "ID",
                value: (short)126);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KR",
                column: "ID",
                value: (short)161);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KW",
                column: "ID",
                value: (short)89);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KZ",
                column: "ID",
                value: (short)86);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LA",
                column: "ID",
                value: (short)91);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LB",
                column: "ID",
                value: (short)93);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LC",
                column: "ID",
                value: (short)145);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LI",
                column: "ID",
                value: (short)97);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LK",
                column: "ID",
                value: (short)164);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LR",
                column: "ID",
                value: (short)95);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LS",
                column: "ID",
                value: (short)94);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LT",
                column: "ID",
                value: (short)98);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LU",
                column: "ID",
                value: (short)99);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LV",
                column: "ID",
                value: (short)92);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LY",
                column: "ID",
                value: (short)96);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MA",
                column: "ID",
                value: (short)115);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MC",
                column: "ID",
                value: (short)112);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MD",
                column: "ID",
                value: (short)111);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ME",
                column: "ID",
                value: (short)114);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MG",
                column: "ID",
                value: (short)100);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MH",
                column: "ID",
                value: (short)106);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MK",
                column: "ID",
                value: (short)127);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ML",
                column: "ID",
                value: (short)104);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MM",
                column: "ID",
                value: (short)117);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MN",
                column: "ID",
                value: (short)113);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MR",
                column: "ID",
                value: (short)107);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MT",
                column: "ID",
                value: (short)105);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MU",
                column: "ID",
                value: (short)108);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MV",
                column: "ID",
                value: (short)103);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MW",
                column: "ID",
                value: (short)101);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MX",
                column: "ID",
                value: (short)109);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MY",
                column: "ID",
                value: (short)102);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MZ",
                column: "ID",
                value: (short)116);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NA",
                column: "ID",
                value: (short)118);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NE",
                column: "ID",
                value: (short)124);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NG",
                column: "ID",
                value: (short)125);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NI",
                column: "ID",
                value: (short)123);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NL",
                column: "ID",
                value: (short)121);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NO",
                column: "ID",
                value: (short)128);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NP",
                column: "ID",
                value: (short)120);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NR",
                column: "ID",
                value: (short)119);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NZ",
                column: "ID",
                value: (short)122);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "OM",
                column: "ID",
                value: (short)129);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PA",
                column: "ID",
                value: (short)133);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PE",
                column: "ID",
                value: (short)136);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PG",
                column: "ID",
                value: (short)134);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PH",
                column: "ID",
                value: (short)137);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PK",
                column: "ID",
                value: (short)130);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PL",
                column: "ID",
                value: (short)138);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PS",
                column: "ID",
                value: (short)132);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PT",
                column: "ID",
                value: (short)139);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PW",
                column: "ID",
                value: (short)131);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PY",
                column: "ID",
                value: (short)135);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "QA",
                column: "ID",
                value: (short)140);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RO",
                column: "ID",
                value: (short)141);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RS",
                column: "ID",
                value: (short)152);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RU",
                column: "ID",
                value: (short)142);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RW",
                column: "ID",
                value: (short)143);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SA",
                column: "ID",
                value: (short)150);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SB",
                column: "ID",
                value: (short)158);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SC",
                column: "ID",
                value: (short)153);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SD",
                column: "ID",
                value: (short)165);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SE",
                column: "ID",
                value: (short)167);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SG",
                column: "ID",
                value: (short)155);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SI",
                column: "ID",
                value: (short)157);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SK",
                column: "ID",
                value: (short)156);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SL",
                column: "ID",
                value: (short)154);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SM",
                column: "ID",
                value: (short)148);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SN",
                column: "ID",
                value: (short)151);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SO",
                column: "ID",
                value: (short)159);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SR",
                column: "ID",
                value: (short)166);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SS",
                column: "ID",
                value: (short)162);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ST",
                column: "ID",
                value: (short)149);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SV",
                column: "ID",
                value: (short)52);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SY",
                column: "ID",
                value: (short)169);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SZ",
                column: "ID",
                value: (short)56);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TD",
                column: "ID",
                value: (short)34);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TG",
                column: "ID",
                value: (short)175);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TH",
                column: "ID",
                value: (short)173);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TJ",
                column: "ID",
                value: (short)171);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TL",
                column: "ID",
                value: (short)174);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TM",
                column: "ID",
                value: (short)180);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TN",
                column: "ID",
                value: (short)178);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TO",
                column: "ID",
                value: (short)176);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TR",
                column: "ID",
                value: (short)179);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TT",
                column: "ID",
                value: (short)177);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TV",
                column: "ID",
                value: (short)181);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TW",
                column: "ID",
                value: (short)170);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TZ",
                column: "ID",
                value: (short)172);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UA",
                column: "ID",
                value: (short)183);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UG",
                column: "ID",
                value: (short)182);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "US",
                column: "ID",
                value: (short)186);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UY",
                column: "ID",
                value: (short)187);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UZ",
                column: "ID",
                value: (short)188);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VC",
                column: "ID",
                value: (short)146);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VE",
                column: "ID",
                value: (short)190);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VN",
                column: "ID",
                value: (short)191);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VU",
                column: "ID",
                value: (short)189);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "WS",
                column: "ID",
                value: (short)147);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "YE",
                column: "ID",
                value: (short)192);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZA",
                column: "ID",
                value: (short)160);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZM",
                column: "ID",
                value: (short)193);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZW",
                column: "ID",
                value: (short)194);

            migrationBuilder.InsertData(
                table: "PROFIT_CODE",
                columns: new[] { "ID", "FREQUENCY", "NAME" },
                values: new object[,]
                {
                    { (byte)0, "Year-end only", "Incoming contributions, forfeitures, earnings" },
                    { (byte)1, "Multiple Times", "Outgoing payments (not rollovers or direct payments) - Partial withdrawal" },
                    { (byte)2, "Multiple Times", "Outgoing forfeitures" },
                    { (byte)3, "Multiple Times", "Outgoing direct payments / rollover payments" },
                    { (byte)5, "Once", "Outgoing XFER beneficiary / QDRO allocation (beneficiary payment)" },
                    { (byte)6, "Once", "Incoming QDRO beneficiary allocation (beneficiary receipt)" },
                    { (byte)8, "Usually year-end, unless there is special processing – i.e. Class Action settlement. Earnings are 100% vested.", "Incoming \"100% vested\" earnings" },
                    { (byte)9, "Multiple Times", "Outgoing payment from 100% vesting amount (payment of ETVA funds)" }
                });

            migrationBuilder.CreateIndex(
                name: "CALDAR_RECORD_ACC_APWKEND_N",
                table: "CALDAR_RECORD",
                column: "ACC_APWKEND",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "CALDAR_RECORD_ACC_WEDATE2",
                table: "CALDAR_RECORD",
                column: "ACC_WKEND2_N",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PROFIT_DETAIL_PROFITCODES_PROFITCODEID",
                table: "PROFIT_DETAIL",
                column: "PROFIT_CODE_ID",
                principalTable: "PROFIT_CODE",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PROFIT_DETAIL_PROFITCODES_PROFITCODEID",
                table: "PROFIT_DETAIL");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PROFIT_CODE",
                table: "PROFIT_CODE");

            migrationBuilder.DropIndex(
                name: "CALDAR_RECORD_ACC_APWKEND_N",
                table: "CALDAR_RECORD");

            migrationBuilder.DropIndex(
                name: "CALDAR_RECORD_ACC_WEDATE2",
                table: "CALDAR_RECORD");

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "ID",
                keyColumnType: "NUMBER(3)",
                keyValue: (byte)0);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "ID",
                keyColumnType: "NUMBER(3)",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "ID",
                keyColumnType: "NUMBER(3)",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "ID",
                keyColumnType: "NUMBER(3)",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "ID",
                keyColumnType: "NUMBER(3)",
                keyValue: (byte)5);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "ID",
                keyColumnType: "NUMBER(3)",
                keyValue: (byte)6);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "ID",
                keyColumnType: "NUMBER(3)",
                keyValue: (byte)8);

            migrationBuilder.DeleteData(
                table: "PROFIT_CODE",
                keyColumn: "ID",
                keyColumnType: "NUMBER(3)",
                keyValue: (byte)9);

            migrationBuilder.DropColumn(
                name: "ID",
                table: "PROFIT_CODE");

            migrationBuilder.RenameIndex(
                name: "IX_PROFIT_DETAIL_PROFITCODEID",
                table: "PROFIT_DETAIL",
                newName: "IX_PROFIT_DETAIL_PROFIT_CODE_ID");

            migrationBuilder.RenameColumn(
                name: "NAME",
                table: "PROFIT_CODE",
                newName: "DEFINITION");

            migrationBuilder.AlterColumn<short>(
                name: "PROFIT_CODE_ID",
                table: "PROFIT_DETAIL",
                type: "NUMBER(5)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "NUMBER(3)");

            migrationBuilder.AddColumn<short>(
                name: "CODE",
                table: "PROFIT_CODE",
                type: "NUMBER(5)",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<long>(
                name: "ACC_WKEND2_N",
                table: "CALDAR_RECORD",
                type: "NUMBER(19)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<int>(
                name: "ACC_APWKEND",
                table: "CALDAR_RECORD",
                type: "NUMBER(10)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)");

            migrationBuilder.AlterColumn<int>(
                name: "ACC_WKEND_N",
                table: "CALDAR_RECORD",
                type: "NUMBER(10)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "NUMBER(10)")
                .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PROFIT_CODE",
                table: "PROFIT_CODE",
                column: "CODE");

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101,
                column: "ACC_WKEND2_N",
                value: 20000101L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 108,
                column: "ACC_WKEND2_N",
                value: 20000108L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 115,
                column: "ACC_WKEND2_N",
                value: 20000115L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 122,
                column: "ACC_WKEND2_N",
                value: 20000122L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 129,
                column: "ACC_WKEND2_N",
                value: 20000129L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 205,
                column: "ACC_WKEND2_N",
                value: 20000205L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 212,
                column: "ACC_WKEND2_N",
                value: 20000212L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 219,
                column: "ACC_WKEND2_N",
                value: 20000219L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 226,
                column: "ACC_WKEND2_N",
                value: 20000226L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 304,
                column: "ACC_WKEND2_N",
                value: 20000304L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 311,
                column: "ACC_WKEND2_N",
                value: 20000311L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 318,
                column: "ACC_WKEND2_N",
                value: 20000318L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 325,
                column: "ACC_WKEND2_N",
                value: 20000325L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 401,
                column: "ACC_WKEND2_N",
                value: 20000401L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 408,
                column: "ACC_WKEND2_N",
                value: 20000408L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 415,
                column: "ACC_WKEND2_N",
                value: 20000415L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 422,
                column: "ACC_WKEND2_N",
                value: 20000422L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 429,
                column: "ACC_WKEND2_N",
                value: 20000429L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 506,
                column: "ACC_WKEND2_N",
                value: 20000506L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 513,
                column: "ACC_WKEND2_N",
                value: 20000513L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 520,
                column: "ACC_WKEND2_N",
                value: 20000520L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 527,
                column: "ACC_WKEND2_N",
                value: 20000527L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 603,
                column: "ACC_WKEND2_N",
                value: 20000603L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 610,
                column: "ACC_WKEND2_N",
                value: 20000610L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 617,
                column: "ACC_WKEND2_N",
                value: 20000617L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 624,
                column: "ACC_WKEND2_N",
                value: 20000624L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 701,
                column: "ACC_WKEND2_N",
                value: 20000701L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 708,
                column: "ACC_WKEND2_N",
                value: 20000708L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 715,
                column: "ACC_WKEND2_N",
                value: 20000715L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 722,
                column: "ACC_WKEND2_N",
                value: 20000722L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 729,
                column: "ACC_WKEND2_N",
                value: 20000729L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 805,
                column: "ACC_WKEND2_N",
                value: 20000805L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 812,
                column: "ACC_WKEND2_N",
                value: 20000812L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 819,
                column: "ACC_WKEND2_N",
                value: 20000819L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 826,
                column: "ACC_WKEND2_N",
                value: 20000826L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 902,
                column: "ACC_WKEND2_N",
                value: 20000902L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 909,
                column: "ACC_WKEND2_N",
                value: 20000909L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 916,
                column: "ACC_WKEND2_N",
                value: 20000916L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 923,
                column: "ACC_WKEND2_N",
                value: 20000923L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 930,
                column: "ACC_WKEND2_N",
                value: 20000930L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1007,
                column: "ACC_WKEND2_N",
                value: 20001007L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1014,
                column: "ACC_WKEND2_N",
                value: 20001014L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1021,
                column: "ACC_WKEND2_N",
                value: 20001021L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1028,
                column: "ACC_WKEND2_N",
                value: 20001028L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1104,
                column: "ACC_WKEND2_N",
                value: 20001104L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1111,
                column: "ACC_WKEND2_N",
                value: 20001111L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1118,
                column: "ACC_WKEND2_N",
                value: 20001118L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1125,
                column: "ACC_WKEND2_N",
                value: 20001125L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1202,
                column: "ACC_WKEND2_N",
                value: 20001202L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1209,
                column: "ACC_WKEND2_N",
                value: 20001209L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1216,
                column: "ACC_WKEND2_N",
                value: 20001216L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1223,
                column: "ACC_WKEND2_N",
                value: 20001223L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 1230,
                column: "ACC_WKEND2_N",
                value: 20001230L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10106,
                column: "ACC_WKEND2_N",
                value: 20010106L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10113,
                column: "ACC_WKEND2_N",
                value: 20010113L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10120,
                column: "ACC_WKEND2_N",
                value: 20010120L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10127,
                column: "ACC_WKEND2_N",
                value: 20010127L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10203,
                column: "ACC_WKEND2_N",
                value: 20010203L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10210,
                column: "ACC_WKEND2_N",
                value: 20010210L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10217,
                column: "ACC_WKEND2_N",
                value: 20010217L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10224,
                column: "ACC_WKEND2_N",
                value: 20010224L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10303,
                column: "ACC_WKEND2_N",
                value: 20010303L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10310,
                column: "ACC_WKEND2_N",
                value: 20010310L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10317,
                column: "ACC_WKEND2_N",
                value: 20010317L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10324,
                column: "ACC_WKEND2_N",
                value: 20010324L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10331,
                column: "ACC_WKEND2_N",
                value: 20010331L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10407,
                column: "ACC_WKEND2_N",
                value: 20010407L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10414,
                column: "ACC_WKEND2_N",
                value: 20010414L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10421,
                column: "ACC_WKEND2_N",
                value: 20010421L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10428,
                column: "ACC_WKEND2_N",
                value: 20010428L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10505,
                column: "ACC_WKEND2_N",
                value: 20010505L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10512,
                column: "ACC_WKEND2_N",
                value: 20010512L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10519,
                column: "ACC_WKEND2_N",
                value: 20010519L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10526,
                column: "ACC_WKEND2_N",
                value: 20010526L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10602,
                column: "ACC_WKEND2_N",
                value: 20010602L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10609,
                column: "ACC_WKEND2_N",
                value: 20010609L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10616,
                column: "ACC_WKEND2_N",
                value: 20010616L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10623,
                column: "ACC_WKEND2_N",
                value: 20010623L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10630,
                column: "ACC_WKEND2_N",
                value: 20010630L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10707,
                column: "ACC_WKEND2_N",
                value: 20010707L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10714,
                column: "ACC_WKEND2_N",
                value: 20010714L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10721,
                column: "ACC_WKEND2_N",
                value: 20010721L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10728,
                column: "ACC_WKEND2_N",
                value: 20010728L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10804,
                column: "ACC_WKEND2_N",
                value: 20010804L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10811,
                column: "ACC_WKEND2_N",
                value: 20010811L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10818,
                column: "ACC_WKEND2_N",
                value: 20010818L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10825,
                column: "ACC_WKEND2_N",
                value: 20010825L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10901,
                column: "ACC_WKEND2_N",
                value: 20010901L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10908,
                column: "ACC_WKEND2_N",
                value: 20010908L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10915,
                column: "ACC_WKEND2_N",
                value: 20010915L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10922,
                column: "ACC_WKEND2_N",
                value: 20010922L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 10929,
                column: "ACC_WKEND2_N",
                value: 20010929L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11006,
                column: "ACC_WKEND2_N",
                value: 20011006L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11013,
                column: "ACC_WKEND2_N",
                value: 20011013L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11020,
                column: "ACC_WKEND2_N",
                value: 20011020L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11027,
                column: "ACC_WKEND2_N",
                value: 20011027L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11103,
                column: "ACC_WKEND2_N",
                value: 20011103L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11110,
                column: "ACC_WKEND2_N",
                value: 20011110L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11117,
                column: "ACC_WKEND2_N",
                value: 20011117L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11124,
                column: "ACC_WKEND2_N",
                value: 20011124L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11201,
                column: "ACC_WKEND2_N",
                value: 20011201L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11208,
                column: "ACC_WKEND2_N",
                value: 20011208L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11215,
                column: "ACC_WKEND2_N",
                value: 20011215L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11222,
                column: "ACC_WKEND2_N",
                value: 20011222L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 11229,
                column: "ACC_WKEND2_N",
                value: 20011229L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20105,
                column: "ACC_WKEND2_N",
                value: 20020105L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20112,
                column: "ACC_WKEND2_N",
                value: 20020112L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20119,
                column: "ACC_WKEND2_N",
                value: 20020119L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20126,
                column: "ACC_WKEND2_N",
                value: 20020126L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20202,
                column: "ACC_WKEND2_N",
                value: 20020202L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20209,
                column: "ACC_WKEND2_N",
                value: 20020209L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20216,
                column: "ACC_WKEND2_N",
                value: 20020216L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20223,
                column: "ACC_WKEND2_N",
                value: 20020223L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20302,
                column: "ACC_WKEND2_N",
                value: 20020302L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20309,
                column: "ACC_WKEND2_N",
                value: 20020309L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20316,
                column: "ACC_WKEND2_N",
                value: 20020316L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20323,
                column: "ACC_WKEND2_N",
                value: 20020323L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20330,
                column: "ACC_WKEND2_N",
                value: 20020330L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20406,
                column: "ACC_WKEND2_N",
                value: 20020406L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20413,
                column: "ACC_WKEND2_N",
                value: 20020413L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20420,
                column: "ACC_WKEND2_N",
                value: 20020420L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20427,
                column: "ACC_WKEND2_N",
                value: 20020427L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20504,
                column: "ACC_WKEND2_N",
                value: 20020504L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20511,
                column: "ACC_WKEND2_N",
                value: 20020511L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20518,
                column: "ACC_WKEND2_N",
                value: 20020518L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20525,
                column: "ACC_WKEND2_N",
                value: 20020525L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20601,
                column: "ACC_WKEND2_N",
                value: 20020601L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20608,
                column: "ACC_WKEND2_N",
                value: 20020608L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20615,
                column: "ACC_WKEND2_N",
                value: 20020615L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20622,
                column: "ACC_WKEND2_N",
                value: 20020622L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20629,
                column: "ACC_WKEND2_N",
                value: 20020629L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20706,
                column: "ACC_WKEND2_N",
                value: 20020706L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20713,
                column: "ACC_WKEND2_N",
                value: 20020713L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20720,
                column: "ACC_WKEND2_N",
                value: 20020720L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20727,
                column: "ACC_WKEND2_N",
                value: 20020727L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20803,
                column: "ACC_WKEND2_N",
                value: 20020803L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20810,
                column: "ACC_WKEND2_N",
                value: 20020810L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20817,
                column: "ACC_WKEND2_N",
                value: 20020817L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20824,
                column: "ACC_WKEND2_N",
                value: 20020824L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20831,
                column: "ACC_WKEND2_N",
                value: 20020831L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20907,
                column: "ACC_WKEND2_N",
                value: 20020907L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20914,
                column: "ACC_WKEND2_N",
                value: 20020914L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20921,
                column: "ACC_WKEND2_N",
                value: 20020921L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 20928,
                column: "ACC_WKEND2_N",
                value: 20020928L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21005,
                column: "ACC_WKEND2_N",
                value: 20021005L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21012,
                column: "ACC_WKEND2_N",
                value: 20021012L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21019,
                column: "ACC_WKEND2_N",
                value: 20021019L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21026,
                column: "ACC_WKEND2_N",
                value: 20021026L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21102,
                column: "ACC_WKEND2_N",
                value: 20021102L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21109,
                column: "ACC_WKEND2_N",
                value: 20021109L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21116,
                column: "ACC_WKEND2_N",
                value: 20021116L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21123,
                column: "ACC_WKEND2_N",
                value: 20021123L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21130,
                column: "ACC_WKEND2_N",
                value: 20021130L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21207,
                column: "ACC_WKEND2_N",
                value: 20021207L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21214,
                column: "ACC_WKEND2_N",
                value: 20021214L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21221,
                column: "ACC_WKEND2_N",
                value: 20021221L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 21228,
                column: "ACC_WKEND2_N",
                value: 20021228L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30104,
                column: "ACC_WKEND2_N",
                value: 20030104L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30111,
                column: "ACC_WKEND2_N",
                value: 20030111L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30118,
                column: "ACC_WKEND2_N",
                value: 20030118L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30125,
                column: "ACC_WKEND2_N",
                value: 20030125L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30201,
                column: "ACC_WKEND2_N",
                value: 20030201L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30208,
                column: "ACC_WKEND2_N",
                value: 20030208L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30215,
                column: "ACC_WKEND2_N",
                value: 20030215L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30222,
                column: "ACC_WKEND2_N",
                value: 20030222L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30301,
                column: "ACC_WKEND2_N",
                value: 20030301L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30308,
                column: "ACC_WKEND2_N",
                value: 20030308L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30315,
                column: "ACC_WKEND2_N",
                value: 20030315L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30322,
                column: "ACC_WKEND2_N",
                value: 20030322L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30329,
                column: "ACC_WKEND2_N",
                value: 20030329L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30405,
                column: "ACC_WKEND2_N",
                value: 20030405L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30412,
                column: "ACC_WKEND2_N",
                value: 20030412L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30419,
                column: "ACC_WKEND2_N",
                value: 20030419L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30426,
                column: "ACC_WKEND2_N",
                value: 20030426L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30503,
                column: "ACC_WKEND2_N",
                value: 20030503L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30510,
                column: "ACC_WKEND2_N",
                value: 20030510L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30517,
                column: "ACC_WKEND2_N",
                value: 20030517L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30524,
                column: "ACC_WKEND2_N",
                value: 20030524L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30531,
                column: "ACC_WKEND2_N",
                value: 20030531L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30607,
                column: "ACC_WKEND2_N",
                value: 20030607L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30614,
                column: "ACC_WKEND2_N",
                value: 20030614L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30621,
                column: "ACC_WKEND2_N",
                value: 20030621L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30628,
                column: "ACC_WKEND2_N",
                value: 20030628L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30705,
                column: "ACC_WKEND2_N",
                value: 20030705L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30712,
                column: "ACC_WKEND2_N",
                value: 20030712L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30719,
                column: "ACC_WKEND2_N",
                value: 20030719L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30726,
                column: "ACC_WKEND2_N",
                value: 20030726L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30802,
                column: "ACC_WKEND2_N",
                value: 20030802L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30809,
                column: "ACC_WKEND2_N",
                value: 20030809L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30816,
                column: "ACC_WKEND2_N",
                value: 20030816L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30823,
                column: "ACC_WKEND2_N",
                value: 20030823L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30830,
                column: "ACC_WKEND2_N",
                value: 20030830L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30906,
                column: "ACC_WKEND2_N",
                value: 20030906L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30913,
                column: "ACC_WKEND2_N",
                value: 20030913L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30920,
                column: "ACC_WKEND2_N",
                value: 20030920L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 30927,
                column: "ACC_WKEND2_N",
                value: 20030927L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31004,
                column: "ACC_WKEND2_N",
                value: 20031004L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31011,
                column: "ACC_WKEND2_N",
                value: 20031011L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31018,
                column: "ACC_WKEND2_N",
                value: 20031018L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31025,
                column: "ACC_WKEND2_N",
                value: 20031025L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31101,
                column: "ACC_WKEND2_N",
                value: 20031101L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31108,
                column: "ACC_WKEND2_N",
                value: 20031108L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31115,
                column: "ACC_WKEND2_N",
                value: 20031115L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31122,
                column: "ACC_WKEND2_N",
                value: 20031122L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31129,
                column: "ACC_WKEND2_N",
                value: 20031129L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31206,
                column: "ACC_WKEND2_N",
                value: 20031206L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31213,
                column: "ACC_WKEND2_N",
                value: 20031213L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31220,
                column: "ACC_WKEND2_N",
                value: 20031220L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 31227,
                column: "ACC_WKEND2_N",
                value: 20031227L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40103,
                column: "ACC_WKEND2_N",
                value: 20040103L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40110,
                column: "ACC_WKEND2_N",
                value: 20040110L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40117,
                column: "ACC_WKEND2_N",
                value: 20040117L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40124,
                column: "ACC_WKEND2_N",
                value: 20040124L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40131,
                column: "ACC_WKEND2_N",
                value: 20040131L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40207,
                column: "ACC_WKEND2_N",
                value: 20040207L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40214,
                column: "ACC_WKEND2_N",
                value: 20040214L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40221,
                column: "ACC_WKEND2_N",
                value: 20040221L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40228,
                column: "ACC_WKEND2_N",
                value: 20040228L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40306,
                column: "ACC_WKEND2_N",
                value: 20040306L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40313,
                column: "ACC_WKEND2_N",
                value: 20040313L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40320,
                column: "ACC_WKEND2_N",
                value: 20040320L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40327,
                column: "ACC_WKEND2_N",
                value: 20040327L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40403,
                column: "ACC_WKEND2_N",
                value: 20040403L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40410,
                column: "ACC_WKEND2_N",
                value: 20040410L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40417,
                column: "ACC_WKEND2_N",
                value: 20040417L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40424,
                column: "ACC_WKEND2_N",
                value: 20040424L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40501,
                column: "ACC_WKEND2_N",
                value: 20040501L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40508,
                column: "ACC_WKEND2_N",
                value: 20040508L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40515,
                column: "ACC_WKEND2_N",
                value: 20040515L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40522,
                column: "ACC_WKEND2_N",
                value: 20040522L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40529,
                column: "ACC_WKEND2_N",
                value: 20040529L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40605,
                column: "ACC_WKEND2_N",
                value: 20040605L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40612,
                column: "ACC_WKEND2_N",
                value: 20040612L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40619,
                column: "ACC_WKEND2_N",
                value: 20040619L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40626,
                column: "ACC_WKEND2_N",
                value: 20040626L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40703,
                column: "ACC_WKEND2_N",
                value: 20040703L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40710,
                column: "ACC_WKEND2_N",
                value: 20040710L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40717,
                column: "ACC_WKEND2_N",
                value: 20040717L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40724,
                column: "ACC_WKEND2_N",
                value: 20040724L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40731,
                column: "ACC_WKEND2_N",
                value: 20040731L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40807,
                column: "ACC_WKEND2_N",
                value: 20040807L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40814,
                column: "ACC_WKEND2_N",
                value: 20040814L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40821,
                column: "ACC_WKEND2_N",
                value: 20040821L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40828,
                column: "ACC_WKEND2_N",
                value: 20040828L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40904,
                column: "ACC_WKEND2_N",
                value: 20040904L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40911,
                column: "ACC_WKEND2_N",
                value: 20040911L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40918,
                column: "ACC_WKEND2_N",
                value: 20040918L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 40925,
                column: "ACC_WKEND2_N",
                value: 20040925L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41002,
                column: "ACC_WKEND2_N",
                value: 20041002L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41009,
                column: "ACC_WKEND2_N",
                value: 20041009L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41016,
                column: "ACC_WKEND2_N",
                value: 20041016L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41023,
                column: "ACC_WKEND2_N",
                value: 20041023L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41030,
                column: "ACC_WKEND2_N",
                value: 20041030L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41106,
                column: "ACC_WKEND2_N",
                value: 20041106L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41113,
                column: "ACC_WKEND2_N",
                value: 20041113L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41120,
                column: "ACC_WKEND2_N",
                value: 20041120L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41127,
                column: "ACC_WKEND2_N",
                value: 20041127L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41204,
                column: "ACC_WKEND2_N",
                value: 20041204L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41211,
                column: "ACC_WKEND2_N",
                value: 20041211L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41218,
                column: "ACC_WKEND2_N",
                value: 20041218L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 41225,
                column: "ACC_WKEND2_N",
                value: 20041225L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50101,
                column: "ACC_WKEND2_N",
                value: 20050101L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50108,
                column: "ACC_WKEND2_N",
                value: 20050108L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50115,
                column: "ACC_WKEND2_N",
                value: 20050115L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50122,
                column: "ACC_WKEND2_N",
                value: 20050122L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50129,
                column: "ACC_WKEND2_N",
                value: 20050129L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50205,
                column: "ACC_WKEND2_N",
                value: 20050205L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50212,
                column: "ACC_WKEND2_N",
                value: 20050212L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50219,
                column: "ACC_WKEND2_N",
                value: 20050219L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50226,
                column: "ACC_WKEND2_N",
                value: 20050226L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50305,
                column: "ACC_WKEND2_N",
                value: 20050305L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50312,
                column: "ACC_WKEND2_N",
                value: 20050312L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50319,
                column: "ACC_WKEND2_N",
                value: 20050319L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50326,
                column: "ACC_WKEND2_N",
                value: 20050326L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50402,
                column: "ACC_WKEND2_N",
                value: 20050402L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50409,
                column: "ACC_WKEND2_N",
                value: 20050409L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50416,
                column: "ACC_WKEND2_N",
                value: 20050416L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50423,
                column: "ACC_WKEND2_N",
                value: 20050423L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50430,
                column: "ACC_WKEND2_N",
                value: 20050430L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50507,
                column: "ACC_WKEND2_N",
                value: 20050507L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50514,
                column: "ACC_WKEND2_N",
                value: 20050514L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50521,
                column: "ACC_WKEND2_N",
                value: 20050521L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50528,
                column: "ACC_WKEND2_N",
                value: 20050528L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50604,
                column: "ACC_WKEND2_N",
                value: 20050604L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50611,
                column: "ACC_WKEND2_N",
                value: 20050611L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50618,
                column: "ACC_WKEND2_N",
                value: 20050618L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50625,
                column: "ACC_WKEND2_N",
                value: 20050625L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50702,
                column: "ACC_WKEND2_N",
                value: 20050702L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50709,
                column: "ACC_WKEND2_N",
                value: 20050709L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50716,
                column: "ACC_WKEND2_N",
                value: 20050716L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50723,
                column: "ACC_WKEND2_N",
                value: 20050723L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50730,
                column: "ACC_WKEND2_N",
                value: 20050730L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50806,
                column: "ACC_WKEND2_N",
                value: 20050806L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50813,
                column: "ACC_WKEND2_N",
                value: 20050813L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50820,
                column: "ACC_WKEND2_N",
                value: 20050820L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50827,
                column: "ACC_WKEND2_N",
                value: 20050827L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50903,
                column: "ACC_WKEND2_N",
                value: 20050903L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50910,
                column: "ACC_WKEND2_N",
                value: 20050910L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50917,
                column: "ACC_WKEND2_N",
                value: 20050917L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 50924,
                column: "ACC_WKEND2_N",
                value: 20050924L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51001,
                column: "ACC_WKEND2_N",
                value: 20051001L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51008,
                column: "ACC_WKEND2_N",
                value: 20051008L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51015,
                column: "ACC_WKEND2_N",
                value: 20051015L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51022,
                column: "ACC_WKEND2_N",
                value: 20051022L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51029,
                column: "ACC_WKEND2_N",
                value: 20051029L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51105,
                column: "ACC_WKEND2_N",
                value: 20051105L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51112,
                column: "ACC_WKEND2_N",
                value: 20051112L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51119,
                column: "ACC_WKEND2_N",
                value: 20051119L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51126,
                column: "ACC_WKEND2_N",
                value: 20051126L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51203,
                column: "ACC_WKEND2_N",
                value: 20051203L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51210,
                column: "ACC_WKEND2_N",
                value: 20051210L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51217,
                column: "ACC_WKEND2_N",
                value: 20051217L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51224,
                column: "ACC_WKEND2_N",
                value: 20051224L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 51231,
                column: "ACC_WKEND2_N",
                value: 20051231L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60107,
                column: "ACC_WKEND2_N",
                value: 20060107L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60114,
                column: "ACC_WKEND2_N",
                value: 20060114L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60121,
                column: "ACC_WKEND2_N",
                value: 20060121L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60128,
                column: "ACC_WKEND2_N",
                value: 20060128L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60204,
                column: "ACC_WKEND2_N",
                value: 20060204L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60211,
                column: "ACC_WKEND2_N",
                value: 20060211L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60218,
                column: "ACC_WKEND2_N",
                value: 20060218L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60225,
                column: "ACC_WKEND2_N",
                value: 20060225L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60304,
                column: "ACC_WKEND2_N",
                value: 20060304L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60311,
                column: "ACC_WKEND2_N",
                value: 20060311L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60318,
                column: "ACC_WKEND2_N",
                value: 20060318L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60325,
                column: "ACC_WKEND2_N",
                value: 20060325L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60401,
                column: "ACC_WKEND2_N",
                value: 20060401L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60408,
                column: "ACC_WKEND2_N",
                value: 20060408L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60415,
                column: "ACC_WKEND2_N",
                value: 20060415L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60422,
                column: "ACC_WKEND2_N",
                value: 20060422L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60429,
                column: "ACC_WKEND2_N",
                value: 20060429L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60506,
                column: "ACC_WKEND2_N",
                value: 20060506L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60513,
                column: "ACC_WKEND2_N",
                value: 20060513L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60520,
                column: "ACC_WKEND2_N",
                value: 20060520L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60527,
                column: "ACC_WKEND2_N",
                value: 20060527L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60603,
                column: "ACC_WKEND2_N",
                value: 20060603L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60610,
                column: "ACC_WKEND2_N",
                value: 20060610L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60617,
                column: "ACC_WKEND2_N",
                value: 20060617L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60624,
                column: "ACC_WKEND2_N",
                value: 20060624L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60701,
                column: "ACC_WKEND2_N",
                value: 20060701L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60708,
                column: "ACC_WKEND2_N",
                value: 20060708L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60715,
                column: "ACC_WKEND2_N",
                value: 20060715L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60722,
                column: "ACC_WKEND2_N",
                value: 20060722L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60729,
                column: "ACC_WKEND2_N",
                value: 20060729L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60805,
                column: "ACC_WKEND2_N",
                value: 20060805L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60812,
                column: "ACC_WKEND2_N",
                value: 20060812L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60819,
                column: "ACC_WKEND2_N",
                value: 20060819L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60826,
                column: "ACC_WKEND2_N",
                value: 20060826L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60902,
                column: "ACC_WKEND2_N",
                value: 20060902L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60909,
                column: "ACC_WKEND2_N",
                value: 20060909L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60916,
                column: "ACC_WKEND2_N",
                value: 20060916L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60923,
                column: "ACC_WKEND2_N",
                value: 20060923L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 60930,
                column: "ACC_WKEND2_N",
                value: 20060930L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61007,
                column: "ACC_WKEND2_N",
                value: 20061007L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61014,
                column: "ACC_WKEND2_N",
                value: 20061014L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61021,
                column: "ACC_WKEND2_N",
                value: 20061021L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61028,
                column: "ACC_WKEND2_N",
                value: 20061028L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61104,
                column: "ACC_WKEND2_N",
                value: 20061104L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61111,
                column: "ACC_WKEND2_N",
                value: 20061111L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61118,
                column: "ACC_WKEND2_N",
                value: 20061118L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61125,
                column: "ACC_WKEND2_N",
                value: 20061125L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61202,
                column: "ACC_WKEND2_N",
                value: 20061202L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61209,
                column: "ACC_WKEND2_N",
                value: 20061209L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61216,
                column: "ACC_WKEND2_N",
                value: 20061216L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61223,
                column: "ACC_WKEND2_N",
                value: 20061223L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 61230,
                column: "ACC_WKEND2_N",
                value: 20061230L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70106,
                column: "ACC_WKEND2_N",
                value: 20070106L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70113,
                column: "ACC_WKEND2_N",
                value: 20070113L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70120,
                column: "ACC_WKEND2_N",
                value: 20070120L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70127,
                column: "ACC_WKEND2_N",
                value: 20070127L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70203,
                column: "ACC_WKEND2_N",
                value: 20070203L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70210,
                column: "ACC_WKEND2_N",
                value: 20070210L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70217,
                column: "ACC_WKEND2_N",
                value: 20070217L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70224,
                column: "ACC_WKEND2_N",
                value: 20070224L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70303,
                column: "ACC_WKEND2_N",
                value: 20070303L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70310,
                column: "ACC_WKEND2_N",
                value: 20070310L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70317,
                column: "ACC_WKEND2_N",
                value: 20070317L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70324,
                column: "ACC_WKEND2_N",
                value: 20070324L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70331,
                column: "ACC_WKEND2_N",
                value: 20070331L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70407,
                column: "ACC_WKEND2_N",
                value: 20070407L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70414,
                column: "ACC_WKEND2_N",
                value: 20070414L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70421,
                column: "ACC_WKEND2_N",
                value: 20070421L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70428,
                column: "ACC_WKEND2_N",
                value: 20070428L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70505,
                column: "ACC_WKEND2_N",
                value: 20070505L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70512,
                column: "ACC_WKEND2_N",
                value: 20070512L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70519,
                column: "ACC_WKEND2_N",
                value: 20070519L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70526,
                column: "ACC_WKEND2_N",
                value: 20070526L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70602,
                column: "ACC_WKEND2_N",
                value: 20070602L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70609,
                column: "ACC_WKEND2_N",
                value: 20070609L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70616,
                column: "ACC_WKEND2_N",
                value: 20070616L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70623,
                column: "ACC_WKEND2_N",
                value: 20070623L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70630,
                column: "ACC_WKEND2_N",
                value: 20070630L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70707,
                column: "ACC_WKEND2_N",
                value: 20070707L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70714,
                column: "ACC_WKEND2_N",
                value: 20070714L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70721,
                column: "ACC_WKEND2_N",
                value: 20070721L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70728,
                column: "ACC_WKEND2_N",
                value: 20070728L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70804,
                column: "ACC_WKEND2_N",
                value: 20070804L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70811,
                column: "ACC_WKEND2_N",
                value: 20070811L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70818,
                column: "ACC_WKEND2_N",
                value: 20070818L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70825,
                column: "ACC_WKEND2_N",
                value: 20070825L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70901,
                column: "ACC_WKEND2_N",
                value: 20070901L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70908,
                column: "ACC_WKEND2_N",
                value: 20070908L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70915,
                column: "ACC_WKEND2_N",
                value: 20070915L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70922,
                column: "ACC_WKEND2_N",
                value: 20070922L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 70929,
                column: "ACC_WKEND2_N",
                value: 20070929L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71006,
                column: "ACC_WKEND2_N",
                value: 20071006L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71013,
                column: "ACC_WKEND2_N",
                value: 20071013L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71020,
                column: "ACC_WKEND2_N",
                value: 20071020L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71027,
                column: "ACC_WKEND2_N",
                value: 20071027L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71103,
                column: "ACC_WKEND2_N",
                value: 20071103L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71110,
                column: "ACC_WKEND2_N",
                value: 20071110L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71117,
                column: "ACC_WKEND2_N",
                value: 20071117L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71124,
                column: "ACC_WKEND2_N",
                value: 20071124L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71201,
                column: "ACC_WKEND2_N",
                value: 20071201L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71208,
                column: "ACC_WKEND2_N",
                value: 20071208L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71215,
                column: "ACC_WKEND2_N",
                value: 20071215L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71222,
                column: "ACC_WKEND2_N",
                value: 20071222L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 71229,
                column: "ACC_WKEND2_N",
                value: 20071229L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80105,
                column: "ACC_WKEND2_N",
                value: 20080105L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80112,
                column: "ACC_WKEND2_N",
                value: 20080112L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80119,
                column: "ACC_WKEND2_N",
                value: 20080119L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80126,
                column: "ACC_WKEND2_N",
                value: 20080126L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80202,
                column: "ACC_WKEND2_N",
                value: 20080202L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80209,
                column: "ACC_WKEND2_N",
                value: 20080209L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80216,
                column: "ACC_WKEND2_N",
                value: 20080216L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80223,
                column: "ACC_WKEND2_N",
                value: 20080223L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80301,
                column: "ACC_WKEND2_N",
                value: 20080301L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80308,
                column: "ACC_WKEND2_N",
                value: 20080308L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80315,
                column: "ACC_WKEND2_N",
                value: 20080315L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80322,
                column: "ACC_WKEND2_N",
                value: 20080322L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80329,
                column: "ACC_WKEND2_N",
                value: 20080329L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80405,
                column: "ACC_WKEND2_N",
                value: 20080405L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80412,
                column: "ACC_WKEND2_N",
                value: 20080412L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80419,
                column: "ACC_WKEND2_N",
                value: 20080419L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80426,
                column: "ACC_WKEND2_N",
                value: 20080426L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80503,
                column: "ACC_WKEND2_N",
                value: 20080503L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80510,
                column: "ACC_WKEND2_N",
                value: 20080510L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80517,
                column: "ACC_WKEND2_N",
                value: 20080517L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80524,
                column: "ACC_WKEND2_N",
                value: 20080524L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80531,
                column: "ACC_WKEND2_N",
                value: 20080531L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80607,
                column: "ACC_WKEND2_N",
                value: 20080607L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80614,
                column: "ACC_WKEND2_N",
                value: 20080614L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80621,
                column: "ACC_WKEND2_N",
                value: 20080621L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80628,
                column: "ACC_WKEND2_N",
                value: 20080628L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80705,
                column: "ACC_WKEND2_N",
                value: 20080705L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80712,
                column: "ACC_WKEND2_N",
                value: 20080712L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80719,
                column: "ACC_WKEND2_N",
                value: 20080719L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80726,
                column: "ACC_WKEND2_N",
                value: 20080726L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80802,
                column: "ACC_WKEND2_N",
                value: 20080802L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80809,
                column: "ACC_WKEND2_N",
                value: 20080809L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80816,
                column: "ACC_WKEND2_N",
                value: 20080816L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80823,
                column: "ACC_WKEND2_N",
                value: 20080823L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80830,
                column: "ACC_WKEND2_N",
                value: 20080830L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80906,
                column: "ACC_WKEND2_N",
                value: 20080906L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80913,
                column: "ACC_WKEND2_N",
                value: 20080913L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80920,
                column: "ACC_WKEND2_N",
                value: 20080920L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 80927,
                column: "ACC_WKEND2_N",
                value: 20080927L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81004,
                column: "ACC_WKEND2_N",
                value: 20081004L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81011,
                column: "ACC_WKEND2_N",
                value: 20081011L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81018,
                column: "ACC_WKEND2_N",
                value: 20081018L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81025,
                column: "ACC_WKEND2_N",
                value: 20081025L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81101,
                column: "ACC_WKEND2_N",
                value: 20081101L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81108,
                column: "ACC_WKEND2_N",
                value: 20081108L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81115,
                column: "ACC_WKEND2_N",
                value: 20081115L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81122,
                column: "ACC_WKEND2_N",
                value: 20081122L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81129,
                column: "ACC_WKEND2_N",
                value: 20081129L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81206,
                column: "ACC_WKEND2_N",
                value: 20081206L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81213,
                column: "ACC_WKEND2_N",
                value: 20081213L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81220,
                column: "ACC_WKEND2_N",
                value: 20081220L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 81227,
                column: "ACC_WKEND2_N",
                value: 20081227L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90103,
                column: "ACC_WKEND2_N",
                value: 20090103L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90110,
                column: "ACC_WKEND2_N",
                value: 20090110L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90117,
                column: "ACC_WKEND2_N",
                value: 20090117L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90124,
                column: "ACC_WKEND2_N",
                value: 20090124L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90131,
                column: "ACC_WKEND2_N",
                value: 20090131L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90207,
                column: "ACC_WKEND2_N",
                value: 20090207L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90214,
                column: "ACC_WKEND2_N",
                value: 20090214L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90221,
                column: "ACC_WKEND2_N",
                value: 20090221L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90228,
                column: "ACC_WKEND2_N",
                value: 20090228L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90307,
                column: "ACC_WKEND2_N",
                value: 20090307L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90314,
                column: "ACC_WKEND2_N",
                value: 20090314L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90321,
                column: "ACC_WKEND2_N",
                value: 20090321L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90328,
                column: "ACC_WKEND2_N",
                value: 20090328L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90404,
                column: "ACC_WKEND2_N",
                value: 20090404L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90411,
                column: "ACC_WKEND2_N",
                value: 20090411L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90418,
                column: "ACC_WKEND2_N",
                value: 20090418L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90425,
                column: "ACC_WKEND2_N",
                value: 20090425L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90502,
                column: "ACC_WKEND2_N",
                value: 20090502L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90509,
                column: "ACC_WKEND2_N",
                value: 20090509L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90516,
                column: "ACC_WKEND2_N",
                value: 20090516L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90523,
                column: "ACC_WKEND2_N",
                value: 20090523L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90530,
                column: "ACC_WKEND2_N",
                value: 20090530L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90606,
                column: "ACC_WKEND2_N",
                value: 20090606L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90613,
                column: "ACC_WKEND2_N",
                value: 20090613L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90620,
                column: "ACC_WKEND2_N",
                value: 20090620L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90627,
                column: "ACC_WKEND2_N",
                value: 20090627L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90704,
                column: "ACC_WKEND2_N",
                value: 20090704L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90711,
                column: "ACC_WKEND2_N",
                value: 20090711L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90718,
                column: "ACC_WKEND2_N",
                value: 20090718L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90725,
                column: "ACC_WKEND2_N",
                value: 20090725L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90801,
                column: "ACC_WKEND2_N",
                value: 20090801L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90808,
                column: "ACC_WKEND2_N",
                value: 20090808L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90815,
                column: "ACC_WKEND2_N",
                value: 20090815L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90822,
                column: "ACC_WKEND2_N",
                value: 20090822L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90829,
                column: "ACC_WKEND2_N",
                value: 20090829L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90905,
                column: "ACC_WKEND2_N",
                value: 20090905L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90912,
                column: "ACC_WKEND2_N",
                value: 20090912L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90919,
                column: "ACC_WKEND2_N",
                value: 20090919L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 90926,
                column: "ACC_WKEND2_N",
                value: 20090926L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91003,
                column: "ACC_WKEND2_N",
                value: 20091003L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91010,
                column: "ACC_WKEND2_N",
                value: 20091010L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91017,
                column: "ACC_WKEND2_N",
                value: 20091017L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91024,
                column: "ACC_WKEND2_N",
                value: 20091024L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91031,
                column: "ACC_WKEND2_N",
                value: 20091031L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91107,
                column: "ACC_WKEND2_N",
                value: 20091107L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91114,
                column: "ACC_WKEND2_N",
                value: 20091114L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91121,
                column: "ACC_WKEND2_N",
                value: 20091121L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91128,
                column: "ACC_WKEND2_N",
                value: 20091128L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91205,
                column: "ACC_WKEND2_N",
                value: 20091205L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91212,
                column: "ACC_WKEND2_N",
                value: 20091212L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91219,
                column: "ACC_WKEND2_N",
                value: 20091219L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 91226,
                column: "ACC_WKEND2_N",
                value: 20091226L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100102,
                column: "ACC_WKEND2_N",
                value: 20100102L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100109,
                column: "ACC_WKEND2_N",
                value: 20100109L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100116,
                column: "ACC_WKEND2_N",
                value: 20100116L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100123,
                column: "ACC_WKEND2_N",
                value: 20100123L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100130,
                column: "ACC_WKEND2_N",
                value: 20100130L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100206,
                column: "ACC_WKEND2_N",
                value: 20100206L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100213,
                column: "ACC_WKEND2_N",
                value: 20100213L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100220,
                column: "ACC_WKEND2_N",
                value: 20100220L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100227,
                column: "ACC_WKEND2_N",
                value: 20100227L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100306,
                column: "ACC_WKEND2_N",
                value: 20100306L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100313,
                column: "ACC_WKEND2_N",
                value: 20100313L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100320,
                column: "ACC_WKEND2_N",
                value: 20100320L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100327,
                column: "ACC_WKEND2_N",
                value: 20100327L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100403,
                column: "ACC_WKEND2_N",
                value: 20100403L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100410,
                column: "ACC_WKEND2_N",
                value: 20100410L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100417,
                column: "ACC_WKEND2_N",
                value: 20100417L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100424,
                column: "ACC_WKEND2_N",
                value: 20100424L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100501,
                column: "ACC_WKEND2_N",
                value: 20100501L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100508,
                column: "ACC_WKEND2_N",
                value: 20100508L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100515,
                column: "ACC_WKEND2_N",
                value: 20100515L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100522,
                column: "ACC_WKEND2_N",
                value: 20100522L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100529,
                column: "ACC_WKEND2_N",
                value: 20100529L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100605,
                column: "ACC_WKEND2_N",
                value: 20100605L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100612,
                column: "ACC_WKEND2_N",
                value: 20100612L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100619,
                column: "ACC_WKEND2_N",
                value: 20100619L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100626,
                column: "ACC_WKEND2_N",
                value: 20100626L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100703,
                column: "ACC_WKEND2_N",
                value: 20100703L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100710,
                column: "ACC_WKEND2_N",
                value: 20100710L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100717,
                column: "ACC_WKEND2_N",
                value: 20100717L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100724,
                column: "ACC_WKEND2_N",
                value: 20100724L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100731,
                column: "ACC_WKEND2_N",
                value: 20100731L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100807,
                column: "ACC_WKEND2_N",
                value: 20100807L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100814,
                column: "ACC_WKEND2_N",
                value: 20100814L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100821,
                column: "ACC_WKEND2_N",
                value: 20100821L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100828,
                column: "ACC_WKEND2_N",
                value: 20100828L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100904,
                column: "ACC_WKEND2_N",
                value: 20100904L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100911,
                column: "ACC_WKEND2_N",
                value: 20100911L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100918,
                column: "ACC_WKEND2_N",
                value: 20100918L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 100925,
                column: "ACC_WKEND2_N",
                value: 20100925L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101002,
                column: "ACC_WKEND2_N",
                value: 20101002L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101009,
                column: "ACC_WKEND2_N",
                value: 20101009L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101016,
                column: "ACC_WKEND2_N",
                value: 20101016L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101023,
                column: "ACC_WKEND2_N",
                value: 20101023L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101030,
                column: "ACC_WKEND2_N",
                value: 20101030L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101106,
                column: "ACC_WKEND2_N",
                value: 20101106L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101113,
                column: "ACC_WKEND2_N",
                value: 20101113L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101120,
                column: "ACC_WKEND2_N",
                value: 20101120L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101127,
                column: "ACC_WKEND2_N",
                value: 20101127L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101204,
                column: "ACC_WKEND2_N",
                value: 20101204L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101211,
                column: "ACC_WKEND2_N",
                value: 20101211L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101218,
                column: "ACC_WKEND2_N",
                value: 20101218L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 101225,
                column: "ACC_WKEND2_N",
                value: 20101225L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110101,
                column: "ACC_WKEND2_N",
                value: 20110101L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110108,
                column: "ACC_WKEND2_N",
                value: 20110108L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110115,
                column: "ACC_WKEND2_N",
                value: 20110115L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110122,
                column: "ACC_WKEND2_N",
                value: 20110122L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110129,
                column: "ACC_WKEND2_N",
                value: 20110129L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110205,
                column: "ACC_WKEND2_N",
                value: 20110205L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110212,
                column: "ACC_WKEND2_N",
                value: 20110212L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110219,
                column: "ACC_WKEND2_N",
                value: 20110219L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110226,
                column: "ACC_WKEND2_N",
                value: 20110226L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110305,
                column: "ACC_WKEND2_N",
                value: 20110305L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110312,
                column: "ACC_WKEND2_N",
                value: 20110312L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110319,
                column: "ACC_WKEND2_N",
                value: 20110319L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110326,
                column: "ACC_WKEND2_N",
                value: 20110326L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110402,
                column: "ACC_WKEND2_N",
                value: 20110402L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110409,
                column: "ACC_WKEND2_N",
                value: 20110409L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110416,
                column: "ACC_WKEND2_N",
                value: 20110416L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110423,
                column: "ACC_WKEND2_N",
                value: 20110423L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110430,
                column: "ACC_WKEND2_N",
                value: 20110430L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110507,
                column: "ACC_WKEND2_N",
                value: 20110507L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110514,
                column: "ACC_WKEND2_N",
                value: 20110514L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110521,
                column: "ACC_WKEND2_N",
                value: 20110521L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110528,
                column: "ACC_WKEND2_N",
                value: 20110528L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110604,
                column: "ACC_WKEND2_N",
                value: 20110604L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110611,
                column: "ACC_WKEND2_N",
                value: 20110611L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110618,
                column: "ACC_WKEND2_N",
                value: 20110618L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110625,
                column: "ACC_WKEND2_N",
                value: 20110625L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110702,
                column: "ACC_WKEND2_N",
                value: 20110702L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110709,
                column: "ACC_WKEND2_N",
                value: 20110709L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110716,
                column: "ACC_WKEND2_N",
                value: 20110716L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110723,
                column: "ACC_WKEND2_N",
                value: 20110723L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110730,
                column: "ACC_WKEND2_N",
                value: 20110730L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110806,
                column: "ACC_WKEND2_N",
                value: 20110806L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110813,
                column: "ACC_WKEND2_N",
                value: 20110813L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110820,
                column: "ACC_WKEND2_N",
                value: 20110820L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110827,
                column: "ACC_WKEND2_N",
                value: 20110827L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110903,
                column: "ACC_WKEND2_N",
                value: 20110903L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110910,
                column: "ACC_WKEND2_N",
                value: 20110910L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110917,
                column: "ACC_WKEND2_N",
                value: 20110917L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 110924,
                column: "ACC_WKEND2_N",
                value: 20110924L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111001,
                column: "ACC_WKEND2_N",
                value: 20111001L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111008,
                column: "ACC_WKEND2_N",
                value: 20111008L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111015,
                column: "ACC_WKEND2_N",
                value: 20111015L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111022,
                column: "ACC_WKEND2_N",
                value: 20111022L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111029,
                column: "ACC_WKEND2_N",
                value: 20111029L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111105,
                column: "ACC_WKEND2_N",
                value: 20111105L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111112,
                column: "ACC_WKEND2_N",
                value: 20111112L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111119,
                column: "ACC_WKEND2_N",
                value: 20111119L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111126,
                column: "ACC_WKEND2_N",
                value: 20111126L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111203,
                column: "ACC_WKEND2_N",
                value: 20111203L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111210,
                column: "ACC_WKEND2_N",
                value: 20111210L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111217,
                column: "ACC_WKEND2_N",
                value: 20111217L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111224,
                column: "ACC_WKEND2_N",
                value: 20111224L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 111231,
                column: "ACC_WKEND2_N",
                value: 20111231L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120107,
                column: "ACC_WKEND2_N",
                value: 20120107L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120114,
                column: "ACC_WKEND2_N",
                value: 20120114L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120121,
                column: "ACC_WKEND2_N",
                value: 20120121L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120128,
                column: "ACC_WKEND2_N",
                value: 20120128L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120204,
                column: "ACC_WKEND2_N",
                value: 20120204L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120211,
                column: "ACC_WKEND2_N",
                value: 20120211L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120218,
                column: "ACC_WKEND2_N",
                value: 20120218L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120225,
                column: "ACC_WKEND2_N",
                value: 20120225L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120303,
                column: "ACC_WKEND2_N",
                value: 20120303L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120310,
                column: "ACC_WKEND2_N",
                value: 20120310L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120317,
                column: "ACC_WKEND2_N",
                value: 20120317L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120324,
                column: "ACC_WKEND2_N",
                value: 20120324L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120331,
                column: "ACC_WKEND2_N",
                value: 20120331L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120407,
                column: "ACC_WKEND2_N",
                value: 20120407L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120414,
                column: "ACC_WKEND2_N",
                value: 20120414L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120421,
                column: "ACC_WKEND2_N",
                value: 20120421L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120428,
                column: "ACC_WKEND2_N",
                value: 20120428L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120505,
                column: "ACC_WKEND2_N",
                value: 20120505L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120512,
                column: "ACC_WKEND2_N",
                value: 20120512L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120519,
                column: "ACC_WKEND2_N",
                value: 20120519L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120526,
                column: "ACC_WKEND2_N",
                value: 20120526L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120602,
                column: "ACC_WKEND2_N",
                value: 20120602L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120609,
                column: "ACC_WKEND2_N",
                value: 20120609L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120616,
                column: "ACC_WKEND2_N",
                value: 20120616L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120623,
                column: "ACC_WKEND2_N",
                value: 20120623L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120630,
                column: "ACC_WKEND2_N",
                value: 20120630L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120707,
                column: "ACC_WKEND2_N",
                value: 20120707L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120714,
                column: "ACC_WKEND2_N",
                value: 20120714L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120721,
                column: "ACC_WKEND2_N",
                value: 20120721L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120728,
                column: "ACC_WKEND2_N",
                value: 20120728L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120804,
                column: "ACC_WKEND2_N",
                value: 20120804L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120811,
                column: "ACC_WKEND2_N",
                value: 20120811L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120818,
                column: "ACC_WKEND2_N",
                value: 20120818L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120825,
                column: "ACC_WKEND2_N",
                value: 20120825L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120901,
                column: "ACC_WKEND2_N",
                value: 20120901L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120908,
                column: "ACC_WKEND2_N",
                value: 20120908L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120915,
                column: "ACC_WKEND2_N",
                value: 20120915L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120922,
                column: "ACC_WKEND2_N",
                value: 20120922L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 120929,
                column: "ACC_WKEND2_N",
                value: 20120929L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121006,
                column: "ACC_WKEND2_N",
                value: 20121006L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121013,
                column: "ACC_WKEND2_N",
                value: 20121013L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121020,
                column: "ACC_WKEND2_N",
                value: 20121020L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121027,
                column: "ACC_WKEND2_N",
                value: 20121027L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121103,
                column: "ACC_WKEND2_N",
                value: 20121103L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121110,
                column: "ACC_WKEND2_N",
                value: 20121110L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121117,
                column: "ACC_WKEND2_N",
                value: 20121117L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121124,
                column: "ACC_WKEND2_N",
                value: 20121124L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121201,
                column: "ACC_WKEND2_N",
                value: 20121201L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121208,
                column: "ACC_WKEND2_N",
                value: 20121208L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121215,
                column: "ACC_WKEND2_N",
                value: 20121215L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121222,
                column: "ACC_WKEND2_N",
                value: 20121222L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 121229,
                column: "ACC_WKEND2_N",
                value: 20121229L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130105,
                column: "ACC_WKEND2_N",
                value: 20130105L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130112,
                column: "ACC_WKEND2_N",
                value: 20130112L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130119,
                column: "ACC_WKEND2_N",
                value: 20130119L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130126,
                column: "ACC_WKEND2_N",
                value: 20130126L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130202,
                column: "ACC_WKEND2_N",
                value: 20130202L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130209,
                column: "ACC_WKEND2_N",
                value: 20130209L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130216,
                column: "ACC_WKEND2_N",
                value: 20130216L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130223,
                column: "ACC_WKEND2_N",
                value: 20130223L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130302,
                column: "ACC_WKEND2_N",
                value: 20130302L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130309,
                column: "ACC_WKEND2_N",
                value: 20130309L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130316,
                column: "ACC_WKEND2_N",
                value: 20130316L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130323,
                column: "ACC_WKEND2_N",
                value: 20130323L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130330,
                column: "ACC_WKEND2_N",
                value: 20130330L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130406,
                column: "ACC_WKEND2_N",
                value: 20130406L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130413,
                column: "ACC_WKEND2_N",
                value: 20130413L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130420,
                column: "ACC_WKEND2_N",
                value: 20130420L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130427,
                column: "ACC_WKEND2_N",
                value: 20130427L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130504,
                column: "ACC_WKEND2_N",
                value: 20130504L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130511,
                column: "ACC_WKEND2_N",
                value: 20130511L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130518,
                column: "ACC_WKEND2_N",
                value: 20130518L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130525,
                column: "ACC_WKEND2_N",
                value: 20130525L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130601,
                column: "ACC_WKEND2_N",
                value: 20130601L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130608,
                column: "ACC_WKEND2_N",
                value: 20130608L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130615,
                column: "ACC_WKEND2_N",
                value: 20130615L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130622,
                column: "ACC_WKEND2_N",
                value: 20130622L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130629,
                column: "ACC_WKEND2_N",
                value: 20130629L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130706,
                column: "ACC_WKEND2_N",
                value: 20130706L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130713,
                column: "ACC_WKEND2_N",
                value: 20130713L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130720,
                column: "ACC_WKEND2_N",
                value: 20130720L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130727,
                column: "ACC_WKEND2_N",
                value: 20130727L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130803,
                column: "ACC_WKEND2_N",
                value: 20130803L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130810,
                column: "ACC_WKEND2_N",
                value: 20130810L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130817,
                column: "ACC_WKEND2_N",
                value: 20130817L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130824,
                column: "ACC_WKEND2_N",
                value: 20130824L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130831,
                column: "ACC_WKEND2_N",
                value: 20130831L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130907,
                column: "ACC_WKEND2_N",
                value: 20130907L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130914,
                column: "ACC_WKEND2_N",
                value: 20130914L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130921,
                column: "ACC_WKEND2_N",
                value: 20130921L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 130928,
                column: "ACC_WKEND2_N",
                value: 20130928L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131005,
                column: "ACC_WKEND2_N",
                value: 20131005L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131012,
                column: "ACC_WKEND2_N",
                value: 20131012L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131019,
                column: "ACC_WKEND2_N",
                value: 20131019L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131026,
                column: "ACC_WKEND2_N",
                value: 20131026L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131102,
                column: "ACC_WKEND2_N",
                value: 20131102L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131109,
                column: "ACC_WKEND2_N",
                value: 20131109L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131116,
                column: "ACC_WKEND2_N",
                value: 20131116L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131123,
                column: "ACC_WKEND2_N",
                value: 20131123L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131130,
                column: "ACC_WKEND2_N",
                value: 20131130L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131207,
                column: "ACC_WKEND2_N",
                value: 20131207L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131214,
                column: "ACC_WKEND2_N",
                value: 20131214L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131221,
                column: "ACC_WKEND2_N",
                value: 20131221L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 131228,
                column: "ACC_WKEND2_N",
                value: 20131228L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140104,
                column: "ACC_WKEND2_N",
                value: 20140104L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140111,
                column: "ACC_WKEND2_N",
                value: 20140111L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140118,
                column: "ACC_WKEND2_N",
                value: 20140118L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140125,
                column: "ACC_WKEND2_N",
                value: 20140125L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140201,
                column: "ACC_WKEND2_N",
                value: 20140201L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140208,
                column: "ACC_WKEND2_N",
                value: 20140208L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140215,
                column: "ACC_WKEND2_N",
                value: 20140215L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140222,
                column: "ACC_WKEND2_N",
                value: 20140222L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140301,
                column: "ACC_WKEND2_N",
                value: 20140301L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140308,
                column: "ACC_WKEND2_N",
                value: 20140308L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140315,
                column: "ACC_WKEND2_N",
                value: 20140315L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140322,
                column: "ACC_WKEND2_N",
                value: 20140322L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140329,
                column: "ACC_WKEND2_N",
                value: 20140329L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140405,
                column: "ACC_WKEND2_N",
                value: 20140405L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140412,
                column: "ACC_WKEND2_N",
                value: 20140412L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140419,
                column: "ACC_WKEND2_N",
                value: 20140419L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140426,
                column: "ACC_WKEND2_N",
                value: 20140426L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140503,
                column: "ACC_WKEND2_N",
                value: 20140503L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140510,
                column: "ACC_WKEND2_N",
                value: 20140510L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140517,
                column: "ACC_WKEND2_N",
                value: 20140517L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140524,
                column: "ACC_WKEND2_N",
                value: 20140524L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140531,
                column: "ACC_WKEND2_N",
                value: 20140531L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140607,
                column: "ACC_WKEND2_N",
                value: 20140607L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140614,
                column: "ACC_WKEND2_N",
                value: 20140614L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140621,
                column: "ACC_WKEND2_N",
                value: 20140621L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140628,
                column: "ACC_WKEND2_N",
                value: 20140628L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140705,
                column: "ACC_WKEND2_N",
                value: 20140705L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140712,
                column: "ACC_WKEND2_N",
                value: 20140712L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140719,
                column: "ACC_WKEND2_N",
                value: 20140719L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140726,
                column: "ACC_WKEND2_N",
                value: 20140726L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140802,
                column: "ACC_WKEND2_N",
                value: 20140802L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140809,
                column: "ACC_WKEND2_N",
                value: 20140809L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140816,
                column: "ACC_WKEND2_N",
                value: 20140816L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140823,
                column: "ACC_WKEND2_N",
                value: 20140823L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140830,
                column: "ACC_WKEND2_N",
                value: 20140830L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140906,
                column: "ACC_WKEND2_N",
                value: 20140906L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140913,
                column: "ACC_WKEND2_N",
                value: 20140913L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140920,
                column: "ACC_WKEND2_N",
                value: 20140920L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 140927,
                column: "ACC_WKEND2_N",
                value: 20140927L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141004,
                column: "ACC_WKEND2_N",
                value: 20141004L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141011,
                column: "ACC_WKEND2_N",
                value: 20141011L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141018,
                column: "ACC_WKEND2_N",
                value: 20141018L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141025,
                column: "ACC_WKEND2_N",
                value: 20141025L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141101,
                column: "ACC_WKEND2_N",
                value: 20141101L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141108,
                column: "ACC_WKEND2_N",
                value: 20141108L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141115,
                column: "ACC_WKEND2_N",
                value: 20141115L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141122,
                column: "ACC_WKEND2_N",
                value: 20141122L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141129,
                column: "ACC_WKEND2_N",
                value: 20141129L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141206,
                column: "ACC_WKEND2_N",
                value: 20141206L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141213,
                column: "ACC_WKEND2_N",
                value: 20141213L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141220,
                column: "ACC_WKEND2_N",
                value: 20141220L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 141227,
                column: "ACC_WKEND2_N",
                value: 20141227L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150103,
                column: "ACC_WKEND2_N",
                value: 20150103L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150110,
                column: "ACC_WKEND2_N",
                value: 20150110L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150117,
                column: "ACC_WKEND2_N",
                value: 20150117L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150124,
                column: "ACC_WKEND2_N",
                value: 20150124L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150131,
                column: "ACC_WKEND2_N",
                value: 20150131L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150207,
                column: "ACC_WKEND2_N",
                value: 20150207L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150214,
                column: "ACC_WKEND2_N",
                value: 20150214L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150221,
                column: "ACC_WKEND2_N",
                value: 20150221L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150228,
                column: "ACC_WKEND2_N",
                value: 20150228L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150307,
                column: "ACC_WKEND2_N",
                value: 20150307L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150314,
                column: "ACC_WKEND2_N",
                value: 20150314L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150321,
                column: "ACC_WKEND2_N",
                value: 20150321L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150328,
                column: "ACC_WKEND2_N",
                value: 20150328L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150404,
                column: "ACC_WKEND2_N",
                value: 20150404L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150411,
                column: "ACC_WKEND2_N",
                value: 20150411L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150418,
                column: "ACC_WKEND2_N",
                value: 20150418L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150425,
                column: "ACC_WKEND2_N",
                value: 20150425L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150502,
                column: "ACC_WKEND2_N",
                value: 20150502L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150509,
                column: "ACC_WKEND2_N",
                value: 20150509L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150516,
                column: "ACC_WKEND2_N",
                value: 20150516L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150523,
                column: "ACC_WKEND2_N",
                value: 20150523L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150530,
                column: "ACC_WKEND2_N",
                value: 20150530L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150606,
                column: "ACC_WKEND2_N",
                value: 20150606L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150613,
                column: "ACC_WKEND2_N",
                value: 20150613L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150620,
                column: "ACC_WKEND2_N",
                value: 20150620L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150627,
                column: "ACC_WKEND2_N",
                value: 20150627L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150704,
                column: "ACC_WKEND2_N",
                value: 20150704L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150711,
                column: "ACC_WKEND2_N",
                value: 20150711L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150718,
                column: "ACC_WKEND2_N",
                value: 20150718L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150725,
                column: "ACC_WKEND2_N",
                value: 20150725L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150801,
                column: "ACC_WKEND2_N",
                value: 20150801L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150808,
                column: "ACC_WKEND2_N",
                value: 20150808L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150815,
                column: "ACC_WKEND2_N",
                value: 20150815L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150822,
                column: "ACC_WKEND2_N",
                value: 20150822L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150829,
                column: "ACC_WKEND2_N",
                value: 20150829L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150905,
                column: "ACC_WKEND2_N",
                value: 20150905L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150912,
                column: "ACC_WKEND2_N",
                value: 20150912L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150919,
                column: "ACC_WKEND2_N",
                value: 20150919L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 150926,
                column: "ACC_WKEND2_N",
                value: 20150926L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151003,
                column: "ACC_WKEND2_N",
                value: 20151003L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151010,
                column: "ACC_WKEND2_N",
                value: 20151010L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151017,
                column: "ACC_WKEND2_N",
                value: 20151017L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151024,
                column: "ACC_WKEND2_N",
                value: 20151024L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151031,
                column: "ACC_WKEND2_N",
                value: 20151031L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151107,
                column: "ACC_WKEND2_N",
                value: 20151107L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151114,
                column: "ACC_WKEND2_N",
                value: 20151114L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151121,
                column: "ACC_WKEND2_N",
                value: 20151121L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151128,
                column: "ACC_WKEND2_N",
                value: 20151128L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151205,
                column: "ACC_WKEND2_N",
                value: 20151205L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151212,
                column: "ACC_WKEND2_N",
                value: 20151212L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151219,
                column: "ACC_WKEND2_N",
                value: 20151219L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 151226,
                column: "ACC_WKEND2_N",
                value: 20151226L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160102,
                column: "ACC_WKEND2_N",
                value: 20160102L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160109,
                column: "ACC_WKEND2_N",
                value: 20160109L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160116,
                column: "ACC_WKEND2_N",
                value: 20160116L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160123,
                column: "ACC_WKEND2_N",
                value: 20160123L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160130,
                column: "ACC_WKEND2_N",
                value: 20160130L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160206,
                column: "ACC_WKEND2_N",
                value: 20160206L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160213,
                column: "ACC_WKEND2_N",
                value: 20160213L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160220,
                column: "ACC_WKEND2_N",
                value: 20160220L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160227,
                column: "ACC_WKEND2_N",
                value: 20160227L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160305,
                column: "ACC_WKEND2_N",
                value: 20160305L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160312,
                column: "ACC_WKEND2_N",
                value: 20160312L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160319,
                column: "ACC_WKEND2_N",
                value: 20160319L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160326,
                column: "ACC_WKEND2_N",
                value: 20160326L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160402,
                column: "ACC_WKEND2_N",
                value: 20160402L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160409,
                column: "ACC_WKEND2_N",
                value: 20160409L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160416,
                column: "ACC_WKEND2_N",
                value: 20160416L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160423,
                column: "ACC_WKEND2_N",
                value: 20160423L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160430,
                column: "ACC_WKEND2_N",
                value: 20160430L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160507,
                column: "ACC_WKEND2_N",
                value: 20160507L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160514,
                column: "ACC_WKEND2_N",
                value: 20160514L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160521,
                column: "ACC_WKEND2_N",
                value: 20160521L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160528,
                column: "ACC_WKEND2_N",
                value: 20160528L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160604,
                column: "ACC_WKEND2_N",
                value: 20160604L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160611,
                column: "ACC_WKEND2_N",
                value: 20160611L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160618,
                column: "ACC_WKEND2_N",
                value: 20160618L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160625,
                column: "ACC_WKEND2_N",
                value: 20160625L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160702,
                column: "ACC_WKEND2_N",
                value: 20160702L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160709,
                column: "ACC_WKEND2_N",
                value: 20160709L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160716,
                column: "ACC_WKEND2_N",
                value: 20160716L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160723,
                column: "ACC_WKEND2_N",
                value: 20160723L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160730,
                column: "ACC_WKEND2_N",
                value: 20160730L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160806,
                column: "ACC_WKEND2_N",
                value: 20160806L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160813,
                column: "ACC_WKEND2_N",
                value: 20160813L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160820,
                column: "ACC_WKEND2_N",
                value: 20160820L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160827,
                column: "ACC_WKEND2_N",
                value: 20160827L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160903,
                column: "ACC_WKEND2_N",
                value: 20160903L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160910,
                column: "ACC_WKEND2_N",
                value: 20160910L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160917,
                column: "ACC_WKEND2_N",
                value: 20160917L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 160924,
                column: "ACC_WKEND2_N",
                value: 20160924L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161001,
                column: "ACC_WKEND2_N",
                value: 20161001L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161008,
                column: "ACC_WKEND2_N",
                value: 20161008L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161015,
                column: "ACC_WKEND2_N",
                value: 20161015L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161022,
                column: "ACC_WKEND2_N",
                value: 20161022L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161029,
                column: "ACC_WKEND2_N",
                value: 20161029L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161105,
                column: "ACC_WKEND2_N",
                value: 20161105L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161112,
                column: "ACC_WKEND2_N",
                value: 20161112L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161119,
                column: "ACC_WKEND2_N",
                value: 20161119L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161126,
                column: "ACC_WKEND2_N",
                value: 20161126L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161203,
                column: "ACC_WKEND2_N",
                value: 20161203L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161210,
                column: "ACC_WKEND2_N",
                value: 20161210L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161217,
                column: "ACC_WKEND2_N",
                value: 20161217L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161224,
                column: "ACC_WKEND2_N",
                value: 20161224L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 161231,
                column: "ACC_WKEND2_N",
                value: 20161231L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170107,
                column: "ACC_WKEND2_N",
                value: 20170107L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170114,
                column: "ACC_WKEND2_N",
                value: 20170114L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170121,
                column: "ACC_WKEND2_N",
                value: 20170121L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170128,
                column: "ACC_WKEND2_N",
                value: 20170128L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170204,
                column: "ACC_WKEND2_N",
                value: 20170204L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170211,
                column: "ACC_WKEND2_N",
                value: 20170211L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170218,
                column: "ACC_WKEND2_N",
                value: 20170218L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170225,
                column: "ACC_WKEND2_N",
                value: 20170225L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170304,
                column: "ACC_WKEND2_N",
                value: 20170304L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170311,
                column: "ACC_WKEND2_N",
                value: 20170311L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170318,
                column: "ACC_WKEND2_N",
                value: 20170318L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170325,
                column: "ACC_WKEND2_N",
                value: 20170325L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170401,
                column: "ACC_WKEND2_N",
                value: 20170401L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170408,
                column: "ACC_WKEND2_N",
                value: 20170408L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170415,
                column: "ACC_WKEND2_N",
                value: 20170415L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170422,
                column: "ACC_WKEND2_N",
                value: 20170422L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170429,
                column: "ACC_WKEND2_N",
                value: 20170429L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170506,
                column: "ACC_WKEND2_N",
                value: 20170506L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170513,
                column: "ACC_WKEND2_N",
                value: 20170513L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170520,
                column: "ACC_WKEND2_N",
                value: 20170520L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170527,
                column: "ACC_WKEND2_N",
                value: 20170527L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170603,
                column: "ACC_WKEND2_N",
                value: 20170603L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170610,
                column: "ACC_WKEND2_N",
                value: 20170610L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170617,
                column: "ACC_WKEND2_N",
                value: 20170617L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170624,
                column: "ACC_WKEND2_N",
                value: 20170624L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170701,
                column: "ACC_WKEND2_N",
                value: 20170701L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170708,
                column: "ACC_WKEND2_N",
                value: 20170708L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170715,
                column: "ACC_WKEND2_N",
                value: 20170715L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170722,
                column: "ACC_WKEND2_N",
                value: 20170722L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170729,
                column: "ACC_WKEND2_N",
                value: 20170729L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170805,
                column: "ACC_WKEND2_N",
                value: 20170805L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170812,
                column: "ACC_WKEND2_N",
                value: 20170812L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170819,
                column: "ACC_WKEND2_N",
                value: 20170819L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170826,
                column: "ACC_WKEND2_N",
                value: 20170826L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170902,
                column: "ACC_WKEND2_N",
                value: 20170902L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170909,
                column: "ACC_WKEND2_N",
                value: 20170909L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170916,
                column: "ACC_WKEND2_N",
                value: 20170916L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170923,
                column: "ACC_WKEND2_N",
                value: 20170923L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 170930,
                column: "ACC_WKEND2_N",
                value: 20170930L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171007,
                column: "ACC_WKEND2_N",
                value: 20171007L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171014,
                column: "ACC_WKEND2_N",
                value: 20171014L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171021,
                column: "ACC_WKEND2_N",
                value: 20171021L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171028,
                column: "ACC_WKEND2_N",
                value: 20171028L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171104,
                column: "ACC_WKEND2_N",
                value: 20171104L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171111,
                column: "ACC_WKEND2_N",
                value: 20171111L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171118,
                column: "ACC_WKEND2_N",
                value: 20171118L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171125,
                column: "ACC_WKEND2_N",
                value: 20171125L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171202,
                column: "ACC_WKEND2_N",
                value: 20171202L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171209,
                column: "ACC_WKEND2_N",
                value: 20171209L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171216,
                column: "ACC_WKEND2_N",
                value: 20171216L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171223,
                column: "ACC_WKEND2_N",
                value: 20171223L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 171230,
                column: "ACC_WKEND2_N",
                value: 20171230L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180106,
                column: "ACC_WKEND2_N",
                value: 20180106L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180113,
                column: "ACC_WKEND2_N",
                value: 20180113L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180120,
                column: "ACC_WKEND2_N",
                value: 20180120L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180127,
                column: "ACC_WKEND2_N",
                value: 20180127L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180203,
                column: "ACC_WKEND2_N",
                value: 20180203L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180210,
                column: "ACC_WKEND2_N",
                value: 20180210L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180217,
                column: "ACC_WKEND2_N",
                value: 20180217L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180224,
                column: "ACC_WKEND2_N",
                value: 20180224L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180303,
                column: "ACC_WKEND2_N",
                value: 20180303L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180310,
                column: "ACC_WKEND2_N",
                value: 20180310L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180317,
                column: "ACC_WKEND2_N",
                value: 20180317L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180324,
                column: "ACC_WKEND2_N",
                value: 20180324L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180331,
                column: "ACC_WKEND2_N",
                value: 20180331L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180407,
                column: "ACC_WKEND2_N",
                value: 20180407L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180414,
                column: "ACC_WKEND2_N",
                value: 20180414L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180421,
                column: "ACC_WKEND2_N",
                value: 20180421L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180428,
                column: "ACC_WKEND2_N",
                value: 20180428L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180505,
                column: "ACC_WKEND2_N",
                value: 20180505L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180512,
                column: "ACC_WKEND2_N",
                value: 20180512L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180519,
                column: "ACC_WKEND2_N",
                value: 20180519L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180526,
                column: "ACC_WKEND2_N",
                value: 20180526L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180602,
                column: "ACC_WKEND2_N",
                value: 20180602L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180609,
                column: "ACC_WKEND2_N",
                value: 20180609L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180616,
                column: "ACC_WKEND2_N",
                value: 20180616L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180623,
                column: "ACC_WKEND2_N",
                value: 20180623L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180630,
                column: "ACC_WKEND2_N",
                value: 20180630L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180707,
                column: "ACC_WKEND2_N",
                value: 20180707L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180714,
                column: "ACC_WKEND2_N",
                value: 20180714L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180721,
                column: "ACC_WKEND2_N",
                value: 20180721L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180728,
                column: "ACC_WKEND2_N",
                value: 20180728L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180804,
                column: "ACC_WKEND2_N",
                value: 20180804L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180811,
                column: "ACC_WKEND2_N",
                value: 20180811L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180818,
                column: "ACC_WKEND2_N",
                value: 20180818L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180825,
                column: "ACC_WKEND2_N",
                value: 20180825L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180901,
                column: "ACC_WKEND2_N",
                value: 20180901L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180908,
                column: "ACC_WKEND2_N",
                value: 20180908L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180915,
                column: "ACC_WKEND2_N",
                value: 20180915L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180922,
                column: "ACC_WKEND2_N",
                value: 20180922L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 180929,
                column: "ACC_WKEND2_N",
                value: 20180929L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181006,
                column: "ACC_WKEND2_N",
                value: 20181006L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181013,
                column: "ACC_WKEND2_N",
                value: 20181013L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181020,
                column: "ACC_WKEND2_N",
                value: 20181020L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181027,
                column: "ACC_WKEND2_N",
                value: 20181027L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181103,
                column: "ACC_WKEND2_N",
                value: 20181103L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181110,
                column: "ACC_WKEND2_N",
                value: 20181110L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181117,
                column: "ACC_WKEND2_N",
                value: 20181117L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181124,
                column: "ACC_WKEND2_N",
                value: 20181124L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181201,
                column: "ACC_WKEND2_N",
                value: 20181201L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181208,
                column: "ACC_WKEND2_N",
                value: 20181208L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181215,
                column: "ACC_WKEND2_N",
                value: 20181215L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181222,
                column: "ACC_WKEND2_N",
                value: 20181222L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 181229,
                column: "ACC_WKEND2_N",
                value: 20181229L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190105,
                column: "ACC_WKEND2_N",
                value: 20190105L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190112,
                column: "ACC_WKEND2_N",
                value: 20190112L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190119,
                column: "ACC_WKEND2_N",
                value: 20190119L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190126,
                column: "ACC_WKEND2_N",
                value: 20190126L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190202,
                column: "ACC_WKEND2_N",
                value: 20190202L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190209,
                column: "ACC_WKEND2_N",
                value: 20190209L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190216,
                column: "ACC_WKEND2_N",
                value: 20190216L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190223,
                column: "ACC_WKEND2_N",
                value: 20190223L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190302,
                column: "ACC_WKEND2_N",
                value: 20190302L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190309,
                column: "ACC_WKEND2_N",
                value: 20190309L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190316,
                column: "ACC_WKEND2_N",
                value: 20190316L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190323,
                column: "ACC_WKEND2_N",
                value: 20190323L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190330,
                column: "ACC_WKEND2_N",
                value: 20190330L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190406,
                column: "ACC_WKEND2_N",
                value: 20190406L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190413,
                column: "ACC_WKEND2_N",
                value: 20190413L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190420,
                column: "ACC_WKEND2_N",
                value: 20190420L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190427,
                column: "ACC_WKEND2_N",
                value: 20190427L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190504,
                column: "ACC_WKEND2_N",
                value: 20190504L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190511,
                column: "ACC_WKEND2_N",
                value: 20190511L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190518,
                column: "ACC_WKEND2_N",
                value: 20190518L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190525,
                column: "ACC_WKEND2_N",
                value: 20190525L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190601,
                column: "ACC_WKEND2_N",
                value: 20190601L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190608,
                column: "ACC_WKEND2_N",
                value: 20190608L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190615,
                column: "ACC_WKEND2_N",
                value: 20190615L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190622,
                column: "ACC_WKEND2_N",
                value: 20190622L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190629,
                column: "ACC_WKEND2_N",
                value: 20190629L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190706,
                column: "ACC_WKEND2_N",
                value: 20190706L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190713,
                column: "ACC_WKEND2_N",
                value: 20190713L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190720,
                column: "ACC_WKEND2_N",
                value: 20190720L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190727,
                column: "ACC_WKEND2_N",
                value: 20190727L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190803,
                column: "ACC_WKEND2_N",
                value: 20190803L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190810,
                column: "ACC_WKEND2_N",
                value: 20190810L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190817,
                column: "ACC_WKEND2_N",
                value: 20190817L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190824,
                column: "ACC_WKEND2_N",
                value: 20190824L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190831,
                column: "ACC_WKEND2_N",
                value: 20190831L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190907,
                column: "ACC_WKEND2_N",
                value: 20190907L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190914,
                column: "ACC_WKEND2_N",
                value: 20190914L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190921,
                column: "ACC_WKEND2_N",
                value: 20190921L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 190928,
                column: "ACC_WKEND2_N",
                value: 20190928L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191005,
                column: "ACC_WKEND2_N",
                value: 20191005L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191012,
                column: "ACC_WKEND2_N",
                value: 20191012L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191019,
                column: "ACC_WKEND2_N",
                value: 20191019L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191026,
                column: "ACC_WKEND2_N",
                value: 20191026L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191102,
                column: "ACC_WKEND2_N",
                value: 20191102L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191109,
                column: "ACC_WKEND2_N",
                value: 20191109L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191116,
                column: "ACC_WKEND2_N",
                value: 20191116L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191123,
                column: "ACC_WKEND2_N",
                value: 20191123L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191130,
                column: "ACC_WKEND2_N",
                value: 20191130L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191207,
                column: "ACC_WKEND2_N",
                value: 20191207L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191214,
                column: "ACC_WKEND2_N",
                value: 20191214L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191221,
                column: "ACC_WKEND2_N",
                value: 20191221L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 191228,
                column: "ACC_WKEND2_N",
                value: 20191228L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200104,
                column: "ACC_WKEND2_N",
                value: 20200104L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200111,
                column: "ACC_WKEND2_N",
                value: 20200111L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200118,
                column: "ACC_WKEND2_N",
                value: 20200118L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200125,
                column: "ACC_WKEND2_N",
                value: 20200125L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200201,
                column: "ACC_WKEND2_N",
                value: 20200201L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200208,
                column: "ACC_WKEND2_N",
                value: 20200208L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200215,
                column: "ACC_WKEND2_N",
                value: 20200215L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200222,
                column: "ACC_WKEND2_N",
                value: 20200222L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200229,
                column: "ACC_WKEND2_N",
                value: 20200229L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200307,
                column: "ACC_WKEND2_N",
                value: 20200307L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200314,
                column: "ACC_WKEND2_N",
                value: 20200314L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200321,
                column: "ACC_WKEND2_N",
                value: 20200321L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200328,
                column: "ACC_WKEND2_N",
                value: 20200328L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200404,
                column: "ACC_WKEND2_N",
                value: 20200404L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200411,
                column: "ACC_WKEND2_N",
                value: 20200411L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200418,
                column: "ACC_WKEND2_N",
                value: 20200418L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200425,
                column: "ACC_WKEND2_N",
                value: 20200425L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200502,
                column: "ACC_WKEND2_N",
                value: 20200502L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200509,
                column: "ACC_WKEND2_N",
                value: 20200509L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200516,
                column: "ACC_WKEND2_N",
                value: 20200516L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200523,
                column: "ACC_WKEND2_N",
                value: 20200523L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200530,
                column: "ACC_WKEND2_N",
                value: 20200530L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200606,
                column: "ACC_WKEND2_N",
                value: 20200606L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200613,
                column: "ACC_WKEND2_N",
                value: 20200613L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200620,
                column: "ACC_WKEND2_N",
                value: 20200620L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200627,
                column: "ACC_WKEND2_N",
                value: 20200627L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200704,
                column: "ACC_WKEND2_N",
                value: 20200704L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200711,
                column: "ACC_WKEND2_N",
                value: 20200711L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200718,
                column: "ACC_WKEND2_N",
                value: 20200718L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200725,
                column: "ACC_WKEND2_N",
                value: 20200725L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200801,
                column: "ACC_WKEND2_N",
                value: 20200801L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200808,
                column: "ACC_WKEND2_N",
                value: 20200808L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200815,
                column: "ACC_WKEND2_N",
                value: 20200815L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200822,
                column: "ACC_WKEND2_N",
                value: 20200822L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200829,
                column: "ACC_WKEND2_N",
                value: 20200829L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200905,
                column: "ACC_WKEND2_N",
                value: 20200905L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200912,
                column: "ACC_WKEND2_N",
                value: 20200912L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200919,
                column: "ACC_WKEND2_N",
                value: 20200919L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 200926,
                column: "ACC_WKEND2_N",
                value: 20200926L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201003,
                column: "ACC_WKEND2_N",
                value: 20201003L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201010,
                column: "ACC_WKEND2_N",
                value: 20201010L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201017,
                column: "ACC_WKEND2_N",
                value: 20201017L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201024,
                column: "ACC_WKEND2_N",
                value: 20201024L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201031,
                column: "ACC_WKEND2_N",
                value: 20201031L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201107,
                column: "ACC_WKEND2_N",
                value: 20201107L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201114,
                column: "ACC_WKEND2_N",
                value: 20201114L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201121,
                column: "ACC_WKEND2_N",
                value: 20201121L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201128,
                column: "ACC_WKEND2_N",
                value: 20201128L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201205,
                column: "ACC_WKEND2_N",
                value: 20201205L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201212,
                column: "ACC_WKEND2_N",
                value: 20201212L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201219,
                column: "ACC_WKEND2_N",
                value: 20201219L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 201226,
                column: "ACC_WKEND2_N",
                value: 20201226L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210102,
                column: "ACC_WKEND2_N",
                value: 20210102L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210109,
                column: "ACC_WKEND2_N",
                value: 20210109L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210116,
                column: "ACC_WKEND2_N",
                value: 20210116L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210123,
                column: "ACC_WKEND2_N",
                value: 20210123L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210130,
                column: "ACC_WKEND2_N",
                value: 20210130L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210206,
                column: "ACC_WKEND2_N",
                value: 20210206L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210213,
                column: "ACC_WKEND2_N",
                value: 20210213L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210220,
                column: "ACC_WKEND2_N",
                value: 20210220L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210227,
                column: "ACC_WKEND2_N",
                value: 20210227L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210306,
                column: "ACC_WKEND2_N",
                value: 20210306L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210313,
                column: "ACC_WKEND2_N",
                value: 20210313L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210320,
                column: "ACC_WKEND2_N",
                value: 20210320L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210327,
                column: "ACC_WKEND2_N",
                value: 20210327L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210403,
                column: "ACC_WKEND2_N",
                value: 20210403L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210410,
                column: "ACC_WKEND2_N",
                value: 20210410L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210417,
                column: "ACC_WKEND2_N",
                value: 20210417L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210424,
                column: "ACC_WKEND2_N",
                value: 20210424L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210501,
                column: "ACC_WKEND2_N",
                value: 20210501L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210508,
                column: "ACC_WKEND2_N",
                value: 20210508L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210515,
                column: "ACC_WKEND2_N",
                value: 20210515L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210522,
                column: "ACC_WKEND2_N",
                value: 20210522L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210529,
                column: "ACC_WKEND2_N",
                value: 20210529L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210605,
                column: "ACC_WKEND2_N",
                value: 20210605L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210612,
                column: "ACC_WKEND2_N",
                value: 20210612L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210619,
                column: "ACC_WKEND2_N",
                value: 20210619L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210626,
                column: "ACC_WKEND2_N",
                value: 20210626L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210703,
                column: "ACC_WKEND2_N",
                value: 20210703L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210710,
                column: "ACC_WKEND2_N",
                value: 20210710L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210717,
                column: "ACC_WKEND2_N",
                value: 20210717L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210724,
                column: "ACC_WKEND2_N",
                value: 20210724L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210731,
                column: "ACC_WKEND2_N",
                value: 20210731L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210807,
                column: "ACC_WKEND2_N",
                value: 20210807L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210814,
                column: "ACC_WKEND2_N",
                value: 20210814L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210821,
                column: "ACC_WKEND2_N",
                value: 20210821L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210828,
                column: "ACC_WKEND2_N",
                value: 20210828L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210904,
                column: "ACC_WKEND2_N",
                value: 20210904L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210911,
                column: "ACC_WKEND2_N",
                value: 20210911L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210918,
                column: "ACC_WKEND2_N",
                value: 20210918L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 210925,
                column: "ACC_WKEND2_N",
                value: 20210925L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211002,
                column: "ACC_WKEND2_N",
                value: 20211002L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211009,
                column: "ACC_WKEND2_N",
                value: 20211009L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211016,
                column: "ACC_WKEND2_N",
                value: 20211016L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211023,
                column: "ACC_WKEND2_N",
                value: 20211023L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211030,
                column: "ACC_WKEND2_N",
                value: 20211030L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211106,
                column: "ACC_WKEND2_N",
                value: 20211106L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211113,
                column: "ACC_WKEND2_N",
                value: 20211113L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211120,
                column: "ACC_WKEND2_N",
                value: 20211120L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211127,
                column: "ACC_WKEND2_N",
                value: 20211127L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211204,
                column: "ACC_WKEND2_N",
                value: 20211204L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211211,
                column: "ACC_WKEND2_N",
                value: 20211211L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211218,
                column: "ACC_WKEND2_N",
                value: 20211218L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 211225,
                column: "ACC_WKEND2_N",
                value: 20211225L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220101,
                column: "ACC_WKEND2_N",
                value: 20220101L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220108,
                column: "ACC_WKEND2_N",
                value: 20220108L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220115,
                column: "ACC_WKEND2_N",
                value: 20220115L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220122,
                column: "ACC_WKEND2_N",
                value: 20220122L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220129,
                column: "ACC_WKEND2_N",
                value: 20220129L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220205,
                column: "ACC_WKEND2_N",
                value: 20220205L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220212,
                column: "ACC_WKEND2_N",
                value: 20220212L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220219,
                column: "ACC_WKEND2_N",
                value: 20220219L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220226,
                column: "ACC_WKEND2_N",
                value: 20220226L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220305,
                column: "ACC_WKEND2_N",
                value: 20220305L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220312,
                column: "ACC_WKEND2_N",
                value: 20220312L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220319,
                column: "ACC_WKEND2_N",
                value: 20220319L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220326,
                column: "ACC_WKEND2_N",
                value: 20220326L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220402,
                column: "ACC_WKEND2_N",
                value: 20220402L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220409,
                column: "ACC_WKEND2_N",
                value: 20220409L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220416,
                column: "ACC_WKEND2_N",
                value: 20220416L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220423,
                column: "ACC_WKEND2_N",
                value: 20220423L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220430,
                column: "ACC_WKEND2_N",
                value: 20220430L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220507,
                column: "ACC_WKEND2_N",
                value: 20220507L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220514,
                column: "ACC_WKEND2_N",
                value: 20220514L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220521,
                column: "ACC_WKEND2_N",
                value: 20220521L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220528,
                column: "ACC_WKEND2_N",
                value: 20220528L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220604,
                column: "ACC_WKEND2_N",
                value: 20220604L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220611,
                column: "ACC_WKEND2_N",
                value: 20220611L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220618,
                column: "ACC_WKEND2_N",
                value: 20220618L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220625,
                column: "ACC_WKEND2_N",
                value: 20220625L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220702,
                column: "ACC_WKEND2_N",
                value: 20220702L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220709,
                column: "ACC_WKEND2_N",
                value: 20220709L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220716,
                column: "ACC_WKEND2_N",
                value: 20220716L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220723,
                column: "ACC_WKEND2_N",
                value: 20220723L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220730,
                column: "ACC_WKEND2_N",
                value: 20220730L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220806,
                column: "ACC_WKEND2_N",
                value: 20220806L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220813,
                column: "ACC_WKEND2_N",
                value: 20220813L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220820,
                column: "ACC_WKEND2_N",
                value: 20220820L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220827,
                column: "ACC_WKEND2_N",
                value: 20220827L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220903,
                column: "ACC_WKEND2_N",
                value: 20220903L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220910,
                column: "ACC_WKEND2_N",
                value: 20220910L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220917,
                column: "ACC_WKEND2_N",
                value: 20220917L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 220924,
                column: "ACC_WKEND2_N",
                value: 20220924L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221001,
                column: "ACC_WKEND2_N",
                value: 20221001L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221008,
                column: "ACC_WKEND2_N",
                value: 20221008L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221015,
                column: "ACC_WKEND2_N",
                value: 20221015L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221022,
                column: "ACC_WKEND2_N",
                value: 20221022L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221029,
                column: "ACC_WKEND2_N",
                value: 20221029L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221105,
                column: "ACC_WKEND2_N",
                value: 20221105L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221112,
                column: "ACC_WKEND2_N",
                value: 20221112L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221119,
                column: "ACC_WKEND2_N",
                value: 20221119L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221126,
                column: "ACC_WKEND2_N",
                value: 20221126L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221203,
                column: "ACC_WKEND2_N",
                value: 20221203L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221210,
                column: "ACC_WKEND2_N",
                value: 20221210L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221217,
                column: "ACC_WKEND2_N",
                value: 20221217L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221224,
                column: "ACC_WKEND2_N",
                value: 20221224L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 221231,
                column: "ACC_WKEND2_N",
                value: 20221231L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230107,
                column: "ACC_WKEND2_N",
                value: 20230107L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230114,
                column: "ACC_WKEND2_N",
                value: 20230114L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230121,
                column: "ACC_WKEND2_N",
                value: 20230121L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230128,
                column: "ACC_WKEND2_N",
                value: 20230128L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230204,
                column: "ACC_WKEND2_N",
                value: 20230204L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230211,
                column: "ACC_WKEND2_N",
                value: 20230211L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230218,
                column: "ACC_WKEND2_N",
                value: 20230218L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230225,
                column: "ACC_WKEND2_N",
                value: 20230225L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230304,
                column: "ACC_WKEND2_N",
                value: 20230304L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230311,
                column: "ACC_WKEND2_N",
                value: 20230311L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230318,
                column: "ACC_WKEND2_N",
                value: 20230318L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230325,
                column: "ACC_WKEND2_N",
                value: 20230325L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230401,
                column: "ACC_WKEND2_N",
                value: 20230401L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230408,
                column: "ACC_WKEND2_N",
                value: 20230408L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230415,
                column: "ACC_WKEND2_N",
                value: 20230415L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230422,
                column: "ACC_WKEND2_N",
                value: 20230422L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230429,
                column: "ACC_WKEND2_N",
                value: 20230429L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230506,
                column: "ACC_WKEND2_N",
                value: 20230506L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230513,
                column: "ACC_WKEND2_N",
                value: 20230513L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230520,
                column: "ACC_WKEND2_N",
                value: 20230520L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230527,
                column: "ACC_WKEND2_N",
                value: 20230527L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230603,
                column: "ACC_WKEND2_N",
                value: 20230603L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230610,
                column: "ACC_WKEND2_N",
                value: 20230610L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230617,
                column: "ACC_WKEND2_N",
                value: 20230617L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230624,
                column: "ACC_WKEND2_N",
                value: 20230624L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230701,
                column: "ACC_WKEND2_N",
                value: 20230701L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230708,
                column: "ACC_WKEND2_N",
                value: 20230708L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230715,
                column: "ACC_WKEND2_N",
                value: 20230715L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230722,
                column: "ACC_WKEND2_N",
                value: 20230722L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230729,
                column: "ACC_WKEND2_N",
                value: 20230729L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230805,
                column: "ACC_WKEND2_N",
                value: 20230805L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230812,
                column: "ACC_WKEND2_N",
                value: 20230812L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230819,
                column: "ACC_WKEND2_N",
                value: 20230819L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230826,
                column: "ACC_WKEND2_N",
                value: 20230826L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230902,
                column: "ACC_WKEND2_N",
                value: 20230902L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230909,
                column: "ACC_WKEND2_N",
                value: 20230909L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230916,
                column: "ACC_WKEND2_N",
                value: 20230916L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230923,
                column: "ACC_WKEND2_N",
                value: 20230923L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 230930,
                column: "ACC_WKEND2_N",
                value: 20230930L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231007,
                column: "ACC_WKEND2_N",
                value: 20231007L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231014,
                column: "ACC_WKEND2_N",
                value: 20231014L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231021,
                column: "ACC_WKEND2_N",
                value: 20231021L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231028,
                column: "ACC_WKEND2_N",
                value: 20231028L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231104,
                column: "ACC_WKEND2_N",
                value: 20231104L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231111,
                column: "ACC_WKEND2_N",
                value: 20231111L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231118,
                column: "ACC_WKEND2_N",
                value: 20231118L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231125,
                column: "ACC_WKEND2_N",
                value: 20231125L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231202,
                column: "ACC_WKEND2_N",
                value: 20231202L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231209,
                column: "ACC_WKEND2_N",
                value: 20231209L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231216,
                column: "ACC_WKEND2_N",
                value: 20231216L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231223,
                column: "ACC_WKEND2_N",
                value: 20231223L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 231230,
                column: "ACC_WKEND2_N",
                value: 20231230L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240106,
                column: "ACC_WKEND2_N",
                value: 20240106L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240113,
                column: "ACC_WKEND2_N",
                value: 20240113L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240120,
                column: "ACC_WKEND2_N",
                value: 20240120L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240127,
                column: "ACC_WKEND2_N",
                value: 20240127L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240203,
                column: "ACC_WKEND2_N",
                value: 20240203L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240210,
                column: "ACC_WKEND2_N",
                value: 20240210L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240217,
                column: "ACC_WKEND2_N",
                value: 20240217L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240224,
                column: "ACC_WKEND2_N",
                value: 20240224L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240302,
                column: "ACC_WKEND2_N",
                value: 20240302L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240309,
                column: "ACC_WKEND2_N",
                value: 20240309L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240316,
                column: "ACC_WKEND2_N",
                value: 20240316L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240323,
                column: "ACC_WKEND2_N",
                value: 20240323L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240330,
                column: "ACC_WKEND2_N",
                value: 20240330L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240406,
                column: "ACC_WKEND2_N",
                value: 20240406L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240413,
                column: "ACC_WKEND2_N",
                value: 20240413L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240420,
                column: "ACC_WKEND2_N",
                value: 20240420L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240427,
                column: "ACC_WKEND2_N",
                value: 20240427L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240504,
                column: "ACC_WKEND2_N",
                value: 20240504L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240511,
                column: "ACC_WKEND2_N",
                value: 20240511L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240518,
                column: "ACC_WKEND2_N",
                value: 20240518L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240525,
                column: "ACC_WKEND2_N",
                value: 20240525L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240601,
                column: "ACC_WKEND2_N",
                value: 20240601L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240608,
                column: "ACC_WKEND2_N",
                value: 20240608L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240615,
                column: "ACC_WKEND2_N",
                value: 20240615L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240622,
                column: "ACC_WKEND2_N",
                value: 20240622L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240629,
                column: "ACC_WKEND2_N",
                value: 20240629L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240706,
                column: "ACC_WKEND2_N",
                value: 20240706L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240713,
                column: "ACC_WKEND2_N",
                value: 20240713L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240720,
                column: "ACC_WKEND2_N",
                value: 20240720L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240727,
                column: "ACC_WKEND2_N",
                value: 20240727L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240803,
                column: "ACC_WKEND2_N",
                value: 20240803L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240810,
                column: "ACC_WKEND2_N",
                value: 20240810L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240817,
                column: "ACC_WKEND2_N",
                value: 20240817L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240824,
                column: "ACC_WKEND2_N",
                value: 20240824L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240831,
                column: "ACC_WKEND2_N",
                value: 20240831L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240907,
                column: "ACC_WKEND2_N",
                value: 20240907L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240914,
                column: "ACC_WKEND2_N",
                value: 20240914L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240921,
                column: "ACC_WKEND2_N",
                value: 20240921L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 240928,
                column: "ACC_WKEND2_N",
                value: 20240928L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241005,
                column: "ACC_WKEND2_N",
                value: 20241005L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241012,
                column: "ACC_WKEND2_N",
                value: 20241012L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241019,
                column: "ACC_WKEND2_N",
                value: 20241019L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241026,
                column: "ACC_WKEND2_N",
                value: 20241026L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241102,
                column: "ACC_WKEND2_N",
                value: 20241102L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241109,
                column: "ACC_WKEND2_N",
                value: 20241109L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241116,
                column: "ACC_WKEND2_N",
                value: 20241116L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241123,
                column: "ACC_WKEND2_N",
                value: 20241123L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241130,
                column: "ACC_WKEND2_N",
                value: 20241130L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241207,
                column: "ACC_WKEND2_N",
                value: 20241207L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241214,
                column: "ACC_WKEND2_N",
                value: 20241214L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241221,
                column: "ACC_WKEND2_N",
                value: 20241221L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 241228,
                column: "ACC_WKEND2_N",
                value: 20241228L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990109,
                column: "ACC_WKEND2_N",
                value: 19990109L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990116,
                column: "ACC_WKEND2_N",
                value: 19990116L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990123,
                column: "ACC_WKEND2_N",
                value: 19990123L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990130,
                column: "ACC_WKEND2_N",
                value: 19990130L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990206,
                column: "ACC_WKEND2_N",
                value: 19990206L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990213,
                column: "ACC_WKEND2_N",
                value: 19990213L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990220,
                column: "ACC_WKEND2_N",
                value: 19990220L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990227,
                column: "ACC_WKEND2_N",
                value: 19990227L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990306,
                column: "ACC_WKEND2_N",
                value: 19990306L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990313,
                column: "ACC_WKEND2_N",
                value: 19990313L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990320,
                column: "ACC_WKEND2_N",
                value: 19990320L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990327,
                column: "ACC_WKEND2_N",
                value: 19990327L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990403,
                column: "ACC_WKEND2_N",
                value: 19990403L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990410,
                column: "ACC_WKEND2_N",
                value: 19990410L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990417,
                column: "ACC_WKEND2_N",
                value: 19990417L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990424,
                column: "ACC_WKEND2_N",
                value: 19990424L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990501,
                column: "ACC_WKEND2_N",
                value: 19990501L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990508,
                column: "ACC_WKEND2_N",
                value: 19990508L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990515,
                column: "ACC_WKEND2_N",
                value: 19990515L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990522,
                column: "ACC_WKEND2_N",
                value: 19990522L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990529,
                column: "ACC_WKEND2_N",
                value: 19990529L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990605,
                column: "ACC_WKEND2_N",
                value: 19990605L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990612,
                column: "ACC_WKEND2_N",
                value: 19990612L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990619,
                column: "ACC_WKEND2_N",
                value: 19990619L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990626,
                column: "ACC_WKEND2_N",
                value: 19990626L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990703,
                column: "ACC_WKEND2_N",
                value: 19990703L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990710,
                column: "ACC_WKEND2_N",
                value: 19990710L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990717,
                column: "ACC_WKEND2_N",
                value: 19990717L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990724,
                column: "ACC_WKEND2_N",
                value: 19990724L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990731,
                column: "ACC_WKEND2_N",
                value: 19990731L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990807,
                column: "ACC_WKEND2_N",
                value: 19990807L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990814,
                column: "ACC_WKEND2_N",
                value: 19990814L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990821,
                column: "ACC_WKEND2_N",
                value: 19990821L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990828,
                column: "ACC_WKEND2_N",
                value: 19990828L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990904,
                column: "ACC_WKEND2_N",
                value: 19990904L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990911,
                column: "ACC_WKEND2_N",
                value: 19990911L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990918,
                column: "ACC_WKEND2_N",
                value: 19990918L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 990925,
                column: "ACC_WKEND2_N",
                value: 19990925L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991002,
                column: "ACC_WKEND2_N",
                value: 19991002L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991009,
                column: "ACC_WKEND2_N",
                value: 19991009L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991016,
                column: "ACC_WKEND2_N",
                value: 19991016L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991023,
                column: "ACC_WKEND2_N",
                value: 19991023L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991030,
                column: "ACC_WKEND2_N",
                value: 19991030L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991106,
                column: "ACC_WKEND2_N",
                value: 19991106L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991113,
                column: "ACC_WKEND2_N",
                value: 19991113L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991120,
                column: "ACC_WKEND2_N",
                value: 19991120L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991127,
                column: "ACC_WKEND2_N",
                value: 19991127L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991204,
                column: "ACC_WKEND2_N",
                value: 19991204L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991211,
                column: "ACC_WKEND2_N",
                value: 19991211L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991218,
                column: "ACC_WKEND2_N",
                value: 19991218L);

            migrationBuilder.UpdateData(
                table: "CALDAR_RECORD",
                keyColumn: "ACC_WKEND_N",
                keyValue: 991225,
                column: "ACC_WKEND2_N",
                value: 19991225L);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AD",
                column: "ID",
                value: (byte)4);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AE",
                column: "ID",
                value: (byte)184);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AF",
                column: "ID",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AG",
                column: "ID",
                value: (byte)6);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AL",
                column: "ID",
                value: (byte)2);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AM",
                column: "ID",
                value: (byte)8);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AO",
                column: "ID",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AR",
                column: "ID",
                value: (byte)7);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AT",
                column: "ID",
                value: (byte)10);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AU",
                column: "ID",
                value: (byte)9);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "AZ",
                column: "ID",
                value: (byte)11);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BA",
                column: "ID",
                value: (byte)22);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BB",
                column: "ID",
                value: (byte)15);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BD",
                column: "ID",
                value: (byte)14);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BE",
                column: "ID",
                value: (byte)17);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BF",
                column: "ID",
                value: (byte)27);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BG",
                column: "ID",
                value: (byte)26);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BH",
                column: "ID",
                value: (byte)13);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BI",
                column: "ID",
                value: (byte)28);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BJ",
                column: "ID",
                value: (byte)19);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BN",
                column: "ID",
                value: (byte)25);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BO",
                column: "ID",
                value: (byte)21);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BR",
                column: "ID",
                value: (byte)24);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BS",
                column: "ID",
                value: (byte)12);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BT",
                column: "ID",
                value: (byte)20);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BW",
                column: "ID",
                value: (byte)23);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BY",
                column: "ID",
                value: (byte)16);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "BZ",
                column: "ID",
                value: (byte)18);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CA",
                column: "ID",
                value: (byte)32);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CD",
                column: "ID",
                value: (byte)45);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CF",
                column: "ID",
                value: (byte)33);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CG",
                column: "ID",
                value: (byte)39);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CH",
                column: "ID",
                value: (byte)168);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CL",
                column: "ID",
                value: (byte)35);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CM",
                column: "ID",
                value: (byte)31);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CN",
                column: "ID",
                value: (byte)36);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CO",
                column: "ID",
                value: (byte)37);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CR",
                column: "ID",
                value: (byte)40);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CU",
                column: "ID",
                value: (byte)42);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CV",
                column: "ID",
                value: (byte)29);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CY",
                column: "ID",
                value: (byte)43);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "CZ",
                column: "ID",
                value: (byte)44);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DE",
                column: "ID",
                value: (byte)64);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DJ",
                column: "ID",
                value: (byte)47);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DK",
                column: "ID",
                value: (byte)46);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DM",
                column: "ID",
                value: (byte)48);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DO",
                column: "ID",
                value: (byte)49);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "DZ",
                column: "ID",
                value: (byte)3);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EC",
                column: "ID",
                value: (byte)50);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EE",
                column: "ID",
                value: (byte)55);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "EG",
                column: "ID",
                value: (byte)51);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ER",
                column: "ID",
                value: (byte)54);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ES",
                column: "ID",
                value: (byte)163);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ET",
                column: "ID",
                value: (byte)57);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FI",
                column: "ID",
                value: (byte)59);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FJ",
                column: "ID",
                value: (byte)58);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FM",
                column: "ID",
                value: (byte)110);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "FR",
                column: "ID",
                value: (byte)60);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GA",
                column: "ID",
                value: (byte)61);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GB",
                column: "ID",
                value: (byte)185);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GD",
                column: "ID",
                value: (byte)67);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GE",
                column: "ID",
                value: (byte)63);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GH",
                column: "ID",
                value: (byte)65);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GM",
                column: "ID",
                value: (byte)62);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GN",
                column: "ID",
                value: (byte)69);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GQ",
                column: "ID",
                value: (byte)53);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GR",
                column: "ID",
                value: (byte)66);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GT",
                column: "ID",
                value: (byte)68);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GW",
                column: "ID",
                value: (byte)70);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "GY",
                column: "ID",
                value: (byte)71);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HN",
                column: "ID",
                value: (byte)73);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HR",
                column: "ID",
                value: (byte)41);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HT",
                column: "ID",
                value: (byte)72);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "HU",
                column: "ID",
                value: (byte)74);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ID",
                column: "ID",
                value: (byte)77);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IE",
                column: "ID",
                value: (byte)80);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IL",
                column: "ID",
                value: (byte)81);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IN",
                column: "ID",
                value: (byte)76);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IQ",
                column: "ID",
                value: (byte)79);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IR",
                column: "ID",
                value: (byte)78);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IS",
                column: "ID",
                value: (byte)75);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "IT",
                column: "ID",
                value: (byte)82);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JM",
                column: "ID",
                value: (byte)83);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JO",
                column: "ID",
                value: (byte)85);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "JP",
                column: "ID",
                value: (byte)84);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KE",
                column: "ID",
                value: (byte)87);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KG",
                column: "ID",
                value: (byte)90);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KH",
                column: "ID",
                value: (byte)30);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KI",
                column: "ID",
                value: (byte)88);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KM",
                column: "ID",
                value: (byte)38);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KN",
                column: "ID",
                value: (byte)144);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KP",
                column: "ID",
                value: (byte)126);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KR",
                column: "ID",
                value: (byte)161);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KW",
                column: "ID",
                value: (byte)89);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "KZ",
                column: "ID",
                value: (byte)86);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LA",
                column: "ID",
                value: (byte)91);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LB",
                column: "ID",
                value: (byte)93);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LC",
                column: "ID",
                value: (byte)145);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LI",
                column: "ID",
                value: (byte)97);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LK",
                column: "ID",
                value: (byte)164);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LR",
                column: "ID",
                value: (byte)95);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LS",
                column: "ID",
                value: (byte)94);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LT",
                column: "ID",
                value: (byte)98);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LU",
                column: "ID",
                value: (byte)99);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LV",
                column: "ID",
                value: (byte)92);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "LY",
                column: "ID",
                value: (byte)96);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MA",
                column: "ID",
                value: (byte)115);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MC",
                column: "ID",
                value: (byte)112);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MD",
                column: "ID",
                value: (byte)111);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ME",
                column: "ID",
                value: (byte)114);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MG",
                column: "ID",
                value: (byte)100);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MH",
                column: "ID",
                value: (byte)106);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MK",
                column: "ID",
                value: (byte)127);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ML",
                column: "ID",
                value: (byte)104);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MM",
                column: "ID",
                value: (byte)117);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MN",
                column: "ID",
                value: (byte)113);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MR",
                column: "ID",
                value: (byte)107);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MT",
                column: "ID",
                value: (byte)105);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MU",
                column: "ID",
                value: (byte)108);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MV",
                column: "ID",
                value: (byte)103);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MW",
                column: "ID",
                value: (byte)101);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MX",
                column: "ID",
                value: (byte)109);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MY",
                column: "ID",
                value: (byte)102);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "MZ",
                column: "ID",
                value: (byte)116);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NA",
                column: "ID",
                value: (byte)118);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NE",
                column: "ID",
                value: (byte)124);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NG",
                column: "ID",
                value: (byte)125);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NI",
                column: "ID",
                value: (byte)123);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NL",
                column: "ID",
                value: (byte)121);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NO",
                column: "ID",
                value: (byte)128);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NP",
                column: "ID",
                value: (byte)120);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NR",
                column: "ID",
                value: (byte)119);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "NZ",
                column: "ID",
                value: (byte)122);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "OM",
                column: "ID",
                value: (byte)129);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PA",
                column: "ID",
                value: (byte)133);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PE",
                column: "ID",
                value: (byte)136);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PG",
                column: "ID",
                value: (byte)134);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PH",
                column: "ID",
                value: (byte)137);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PK",
                column: "ID",
                value: (byte)130);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PL",
                column: "ID",
                value: (byte)138);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PS",
                column: "ID",
                value: (byte)132);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PT",
                column: "ID",
                value: (byte)139);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PW",
                column: "ID",
                value: (byte)131);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "PY",
                column: "ID",
                value: (byte)135);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "QA",
                column: "ID",
                value: (byte)140);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RO",
                column: "ID",
                value: (byte)141);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RS",
                column: "ID",
                value: (byte)152);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RU",
                column: "ID",
                value: (byte)142);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "RW",
                column: "ID",
                value: (byte)143);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SA",
                column: "ID",
                value: (byte)150);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SB",
                column: "ID",
                value: (byte)158);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SC",
                column: "ID",
                value: (byte)153);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SD",
                column: "ID",
                value: (byte)165);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SE",
                column: "ID",
                value: (byte)167);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SG",
                column: "ID",
                value: (byte)155);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SI",
                column: "ID",
                value: (byte)157);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SK",
                column: "ID",
                value: (byte)156);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SL",
                column: "ID",
                value: (byte)154);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SM",
                column: "ID",
                value: (byte)148);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SN",
                column: "ID",
                value: (byte)151);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SO",
                column: "ID",
                value: (byte)159);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SR",
                column: "ID",
                value: (byte)166);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SS",
                column: "ID",
                value: (byte)162);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ST",
                column: "ID",
                value: (byte)149);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SV",
                column: "ID",
                value: (byte)52);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SY",
                column: "ID",
                value: (byte)169);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "SZ",
                column: "ID",
                value: (byte)56);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TD",
                column: "ID",
                value: (byte)34);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TG",
                column: "ID",
                value: (byte)175);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TH",
                column: "ID",
                value: (byte)173);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TJ",
                column: "ID",
                value: (byte)171);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TL",
                column: "ID",
                value: (byte)174);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TM",
                column: "ID",
                value: (byte)180);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TN",
                column: "ID",
                value: (byte)178);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TO",
                column: "ID",
                value: (byte)176);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TR",
                column: "ID",
                value: (byte)179);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TT",
                column: "ID",
                value: (byte)177);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TV",
                column: "ID",
                value: (byte)181);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TW",
                column: "ID",
                value: (byte)170);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "TZ",
                column: "ID",
                value: (byte)172);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UA",
                column: "ID",
                value: (byte)183);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UG",
                column: "ID",
                value: (byte)182);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "US",
                column: "ID",
                value: (byte)186);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UY",
                column: "ID",
                value: (byte)187);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "UZ",
                column: "ID",
                value: (byte)188);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VC",
                column: "ID",
                value: (byte)146);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VE",
                column: "ID",
                value: (byte)190);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VN",
                column: "ID",
                value: (byte)191);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "VU",
                column: "ID",
                value: (byte)189);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "WS",
                column: "ID",
                value: (byte)147);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "YE",
                column: "ID",
                value: (byte)192);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZA",
                column: "ID",
                value: (byte)160);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZM",
                column: "ID",
                value: (byte)193);

            migrationBuilder.UpdateData(
                table: "COUNTRY",
                keyColumn: "ISO",
                keyValue: "ZW",
                column: "ID",
                value: (byte)194);

            migrationBuilder.InsertData(
                table: "PROFIT_CODE",
                columns: new[] { "CODE", "DEFINITION", "FREQUENCY" },
                values: new object[,]
                {
                    { (short)0, "Incoming contributions, forfeitures, earnings", "Year-end only" },
                    { (short)1, "Outgoing payments (not rollovers or direct payments) - Partial withdrawal", "Multiple Times" },
                    { (short)2, "Outgoing forfeitures", "Multiple Times" },
                    { (short)3, "Outgoing direct payments / rollover payments", "Multiple Times" },
                    { (short)5, "Outgoing XFER beneficiary / QDRO allocation (beneficiary payment)", "Once" },
                    { (short)6, "Incoming QDRO beneficiary allocation  (beneficiary receipt)", "Once" },
                    { (short)8, "Incoming \"100% vested\" earnings", "Usually year-end, unless there is special processing – i.e. Class Action settlement.  Earnings are 100% vested." },
                    { (short)9, "Outgoing payment from 100% vesting amount (payment of ETVA funds)", "Multiple Times" }
                });

            migrationBuilder.CreateIndex(
                name: "CALDAR_RECORD_ACC_APWKEND_N",
                table: "CALDAR_RECORD",
                column: "ACC_APWKEND",
                unique: true,
                filter: "\"ACC_APWKEND\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "CALDAR_RECORD_ACC_WEDATE2",
                table: "CALDAR_RECORD",
                column: "ACC_WKEND2_N",
                unique: true,
                filter: "\"ACC_WKEND2_N\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PROFIT_DETAIL_PROFITCODES_PROFIT_CODE_ID",
                table: "PROFIT_DETAIL",
                column: "PROFIT_CODE_ID",
                principalTable: "PROFIT_CODE",
                principalColumn: "CODE");
        }
    }
}
