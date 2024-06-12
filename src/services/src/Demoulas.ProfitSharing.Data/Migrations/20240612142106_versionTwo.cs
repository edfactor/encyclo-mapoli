using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class versionTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DEMOGRAPHICS",
                table: "DEMOGRAPHICS");

            migrationBuilder.AlterColumn<string>(
                name: "PY_EMP_TELNO",
                table: "DEMOGRAPHICS",
                type: "NVARCHAR2(15)",
                maxLength: 15,
                nullable: false,
                comment: "PhoneNumber",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(10)",
                oldMaxLength: 10,
                oldComment: "PhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "DEMOGRAPHICS",
                type: "NVARCHAR2(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DEM_SSN",
                table: "DEMOGRAPHICS",
                type: "NUMBER(9)",
                precision: 9,
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DEMOGRAPHICS",
                table: "DEMOGRAPHICS",
                column: "DEM_SSN");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AD",
                column: "Id",
                value: (short)4);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AE",
                column: "Id",
                value: (short)184);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AF",
                column: "Id",
                value: (short)1);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AG",
                column: "Id",
                value: (short)6);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AL",
                column: "Id",
                value: (short)2);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AM",
                column: "Id",
                value: (short)8);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AO",
                column: "Id",
                value: (short)5);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AR",
                column: "Id",
                value: (short)7);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AT",
                column: "Id",
                value: (short)10);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AU",
                column: "Id",
                value: (short)9);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AZ",
                column: "Id",
                value: (short)11);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BA",
                column: "Id",
                value: (short)22);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BB",
                column: "Id",
                value: (short)15);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BD",
                column: "Id",
                value: (short)14);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BE",
                column: "Id",
                value: (short)17);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BF",
                column: "Id",
                value: (short)27);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BG",
                column: "Id",
                value: (short)26);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BH",
                column: "Id",
                value: (short)13);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BI",
                column: "Id",
                value: (short)28);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BJ",
                column: "Id",
                value: (short)19);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BN",
                column: "Id",
                value: (short)25);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BO",
                column: "Id",
                value: (short)21);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BR",
                column: "Id",
                value: (short)24);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BS",
                column: "Id",
                value: (short)12);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BT",
                column: "Id",
                value: (short)20);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BW",
                column: "Id",
                value: (short)23);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BY",
                column: "Id",
                value: (short)16);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BZ",
                column: "Id",
                value: (short)18);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CA",
                column: "Id",
                value: (short)32);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CD",
                column: "Id",
                value: (short)45);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CF",
                column: "Id",
                value: (short)33);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CG",
                column: "Id",
                value: (short)39);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CH",
                column: "Id",
                value: (short)168);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CL",
                column: "Id",
                value: (short)35);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CM",
                column: "Id",
                value: (short)31);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CN",
                column: "Id",
                value: (short)36);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CO",
                column: "Id",
                value: (short)37);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CR",
                column: "Id",
                value: (short)40);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CU",
                column: "Id",
                value: (short)42);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CV",
                column: "Id",
                value: (short)29);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CY",
                column: "Id",
                value: (short)43);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CZ",
                column: "Id",
                value: (short)44);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DE",
                column: "Id",
                value: (short)64);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DJ",
                column: "Id",
                value: (short)47);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DK",
                column: "Id",
                value: (short)46);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DM",
                column: "Id",
                value: (short)48);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DO",
                column: "Id",
                value: (short)49);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DZ",
                column: "Id",
                value: (short)3);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "EC",
                column: "Id",
                value: (short)50);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "EE",
                column: "Id",
                value: (short)55);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "EG",
                column: "Id",
                value: (short)51);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ER",
                column: "Id",
                value: (short)54);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ES",
                column: "Id",
                value: (short)163);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ET",
                column: "Id",
                value: (short)57);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "FI",
                column: "Id",
                value: (short)59);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "FJ",
                column: "Id",
                value: (short)58);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "FM",
                column: "Id",
                value: (short)110);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "FR",
                column: "Id",
                value: (short)60);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GA",
                column: "Id",
                value: (short)61);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GB",
                column: "Id",
                value: (short)185);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GD",
                column: "Id",
                value: (short)67);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GE",
                column: "Id",
                value: (short)63);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GH",
                column: "Id",
                value: (short)65);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GM",
                column: "Id",
                value: (short)62);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GN",
                column: "Id",
                value: (short)69);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GQ",
                column: "Id",
                value: (short)53);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GR",
                column: "Id",
                value: (short)66);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GT",
                column: "Id",
                value: (short)68);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GW",
                column: "Id",
                value: (short)70);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GY",
                column: "Id",
                value: (short)71);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "HN",
                column: "Id",
                value: (short)73);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "HR",
                column: "Id",
                value: (short)41);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "HT",
                column: "Id",
                value: (short)72);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "HU",
                column: "Id",
                value: (short)74);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ID",
                column: "Id",
                value: (short)77);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IE",
                column: "Id",
                value: (short)80);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IL",
                column: "Id",
                value: (short)81);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IN",
                column: "Id",
                value: (short)76);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IQ",
                column: "Id",
                value: (short)79);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IR",
                column: "Id",
                value: (short)78);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IS",
                column: "Id",
                value: (short)75);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IT",
                column: "Id",
                value: (short)82);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "JM",
                column: "Id",
                value: (short)83);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "JO",
                column: "Id",
                value: (short)85);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "JP",
                column: "Id",
                value: (short)84);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KE",
                column: "Id",
                value: (short)87);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KG",
                column: "Id",
                value: (short)90);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KH",
                column: "Id",
                value: (short)30);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KI",
                column: "Id",
                value: (short)88);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KM",
                column: "Id",
                value: (short)38);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KN",
                column: "Id",
                value: (short)144);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KP",
                column: "Id",
                value: (short)126);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KR",
                column: "Id",
                value: (short)161);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KW",
                column: "Id",
                value: (short)89);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KZ",
                column: "Id",
                value: (short)86);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LA",
                column: "Id",
                value: (short)91);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LB",
                column: "Id",
                value: (short)93);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LC",
                column: "Id",
                value: (short)145);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LI",
                column: "Id",
                value: (short)97);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LK",
                column: "Id",
                value: (short)164);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LR",
                column: "Id",
                value: (short)95);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LS",
                column: "Id",
                value: (short)94);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LT",
                column: "Id",
                value: (short)98);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LU",
                column: "Id",
                value: (short)99);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LV",
                column: "Id",
                value: (short)92);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LY",
                column: "Id",
                value: (short)96);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MA",
                column: "Id",
                value: (short)115);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MC",
                column: "Id",
                value: (short)112);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MD",
                column: "Id",
                value: (short)111);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ME",
                column: "Id",
                value: (short)114);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MG",
                column: "Id",
                value: (short)100);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MH",
                column: "Id",
                value: (short)106);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MK",
                column: "Id",
                value: (short)127);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ML",
                column: "Id",
                value: (short)104);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MM",
                column: "Id",
                value: (short)117);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MN",
                column: "Id",
                value: (short)113);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MR",
                column: "Id",
                value: (short)107);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MT",
                column: "Id",
                value: (short)105);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MU",
                column: "Id",
                value: (short)108);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MV",
                column: "Id",
                value: (short)103);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MW",
                column: "Id",
                value: (short)101);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MX",
                column: "Id",
                value: (short)109);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MY",
                column: "Id",
                value: (short)102);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MZ",
                column: "Id",
                value: (short)116);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NA",
                column: "Id",
                value: (short)118);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NE",
                column: "Id",
                value: (short)124);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NG",
                column: "Id",
                value: (short)125);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NI",
                column: "Id",
                value: (short)123);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NL",
                column: "Id",
                value: (short)121);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NO",
                column: "Id",
                value: (short)128);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NP",
                column: "Id",
                value: (short)120);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NR",
                column: "Id",
                value: (short)119);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NZ",
                column: "Id",
                value: (short)122);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "OM",
                column: "Id",
                value: (short)129);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PA",
                column: "Id",
                value: (short)133);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PE",
                column: "Id",
                value: (short)136);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PG",
                column: "Id",
                value: (short)134);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PH",
                column: "Id",
                value: (short)137);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PK",
                column: "Id",
                value: (short)130);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PL",
                column: "Id",
                value: (short)138);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PS",
                column: "Id",
                value: (short)132);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PT",
                column: "Id",
                value: (short)139);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PW",
                column: "Id",
                value: (short)131);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PY",
                column: "Id",
                value: (short)135);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "QA",
                column: "Id",
                value: (short)140);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "RO",
                column: "Id",
                value: (short)141);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "RS",
                column: "Id",
                value: (short)152);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "RU",
                column: "Id",
                value: (short)142);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "RW",
                column: "Id",
                value: (short)143);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SA",
                column: "Id",
                value: (short)150);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SB",
                column: "Id",
                value: (short)158);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SC",
                column: "Id",
                value: (short)153);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SD",
                column: "Id",
                value: (short)165);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SE",
                column: "Id",
                value: (short)167);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SG",
                column: "Id",
                value: (short)155);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SI",
                column: "Id",
                value: (short)157);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SK",
                column: "Id",
                value: (short)156);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SL",
                column: "Id",
                value: (short)154);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SM",
                column: "Id",
                value: (short)148);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SN",
                column: "Id",
                value: (short)151);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SO",
                column: "Id",
                value: (short)159);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SR",
                column: "Id",
                value: (short)166);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SS",
                column: "Id",
                value: (short)162);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ST",
                column: "Id",
                value: (short)149);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SV",
                column: "Id",
                value: (short)52);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SY",
                column: "Id",
                value: (short)169);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SZ",
                column: "Id",
                value: (short)56);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TD",
                column: "Id",
                value: (short)34);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TG",
                column: "Id",
                value: (short)175);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TH",
                column: "Id",
                value: (short)173);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TJ",
                column: "Id",
                value: (short)171);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TL",
                column: "Id",
                value: (short)174);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TM",
                column: "Id",
                value: (short)180);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TN",
                column: "Id",
                value: (short)178);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TO",
                column: "Id",
                value: (short)176);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TR",
                column: "Id",
                value: (short)179);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TT",
                column: "Id",
                value: (short)177);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TV",
                column: "Id",
                value: (short)181);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TW",
                column: "Id",
                value: (short)170);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TZ",
                column: "Id",
                value: (short)172);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "UA",
                column: "Id",
                value: (short)183);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "UG",
                column: "Id",
                value: (short)182);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "US",
                column: "Id",
                value: (short)186);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "UY",
                column: "Id",
                value: (short)187);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "UZ",
                column: "Id",
                value: (short)188);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "VC",
                column: "Id",
                value: (short)146);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "VE",
                column: "Id",
                value: (short)190);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "VN",
                column: "Id",
                value: (short)191);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "VU",
                column: "Id",
                value: (short)189);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "WS",
                column: "Id",
                value: (short)147);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "YE",
                column: "Id",
                value: (short)192);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ZA",
                column: "Id",
                value: (short)160);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ZM",
                column: "Id",
                value: (short)193);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ZW",
                column: "Id",
                value: (short)194);

            migrationBuilder.UpdateData(
                table: "Definition",
                keyColumn: "Key",
                keyValue: "F",
                column: "Description",
                value: "Full time 8 paid holidays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DEMOGRAPHICS",
                table: "DEMOGRAPHICS");

            migrationBuilder.DropColumn(
                name: "DEM_SSN",
                table: "DEMOGRAPHICS");

            migrationBuilder.AlterColumn<string>(
                name: "PY_EMP_TELNO",
                table: "DEMOGRAPHICS",
                type: "NVARCHAR2(10)",
                maxLength: 10,
                nullable: false,
                comment: "PhoneNumber",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(15)",
                oldMaxLength: 15,
                oldComment: "PhoneNumber");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "DEMOGRAPHICS",
                type: "NVARCHAR2(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DEMOGRAPHICS",
                table: "DEMOGRAPHICS",
                column: "DEM_BADGE");

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AD",
                column: "Id",
                value: (byte)4);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AE",
                column: "Id",
                value: (byte)184);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AF",
                column: "Id",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AG",
                column: "Id",
                value: (byte)6);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AL",
                column: "Id",
                value: (byte)2);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AM",
                column: "Id",
                value: (byte)8);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AO",
                column: "Id",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AR",
                column: "Id",
                value: (byte)7);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AT",
                column: "Id",
                value: (byte)10);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AU",
                column: "Id",
                value: (byte)9);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "AZ",
                column: "Id",
                value: (byte)11);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BA",
                column: "Id",
                value: (byte)22);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BB",
                column: "Id",
                value: (byte)15);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BD",
                column: "Id",
                value: (byte)14);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BE",
                column: "Id",
                value: (byte)17);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BF",
                column: "Id",
                value: (byte)27);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BG",
                column: "Id",
                value: (byte)26);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BH",
                column: "Id",
                value: (byte)13);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BI",
                column: "Id",
                value: (byte)28);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BJ",
                column: "Id",
                value: (byte)19);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BN",
                column: "Id",
                value: (byte)25);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BO",
                column: "Id",
                value: (byte)21);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BR",
                column: "Id",
                value: (byte)24);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BS",
                column: "Id",
                value: (byte)12);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BT",
                column: "Id",
                value: (byte)20);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BW",
                column: "Id",
                value: (byte)23);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BY",
                column: "Id",
                value: (byte)16);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "BZ",
                column: "Id",
                value: (byte)18);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CA",
                column: "Id",
                value: (byte)32);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CD",
                column: "Id",
                value: (byte)45);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CF",
                column: "Id",
                value: (byte)33);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CG",
                column: "Id",
                value: (byte)39);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CH",
                column: "Id",
                value: (byte)168);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CL",
                column: "Id",
                value: (byte)35);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CM",
                column: "Id",
                value: (byte)31);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CN",
                column: "Id",
                value: (byte)36);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CO",
                column: "Id",
                value: (byte)37);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CR",
                column: "Id",
                value: (byte)40);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CU",
                column: "Id",
                value: (byte)42);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CV",
                column: "Id",
                value: (byte)29);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CY",
                column: "Id",
                value: (byte)43);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "CZ",
                column: "Id",
                value: (byte)44);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DE",
                column: "Id",
                value: (byte)64);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DJ",
                column: "Id",
                value: (byte)47);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DK",
                column: "Id",
                value: (byte)46);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DM",
                column: "Id",
                value: (byte)48);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DO",
                column: "Id",
                value: (byte)49);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "DZ",
                column: "Id",
                value: (byte)3);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "EC",
                column: "Id",
                value: (byte)50);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "EE",
                column: "Id",
                value: (byte)55);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "EG",
                column: "Id",
                value: (byte)51);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ER",
                column: "Id",
                value: (byte)54);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ES",
                column: "Id",
                value: (byte)163);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ET",
                column: "Id",
                value: (byte)57);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "FI",
                column: "Id",
                value: (byte)59);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "FJ",
                column: "Id",
                value: (byte)58);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "FM",
                column: "Id",
                value: (byte)110);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "FR",
                column: "Id",
                value: (byte)60);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GA",
                column: "Id",
                value: (byte)61);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GB",
                column: "Id",
                value: (byte)185);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GD",
                column: "Id",
                value: (byte)67);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GE",
                column: "Id",
                value: (byte)63);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GH",
                column: "Id",
                value: (byte)65);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GM",
                column: "Id",
                value: (byte)62);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GN",
                column: "Id",
                value: (byte)69);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GQ",
                column: "Id",
                value: (byte)53);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GR",
                column: "Id",
                value: (byte)66);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GT",
                column: "Id",
                value: (byte)68);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GW",
                column: "Id",
                value: (byte)70);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "GY",
                column: "Id",
                value: (byte)71);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "HN",
                column: "Id",
                value: (byte)73);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "HR",
                column: "Id",
                value: (byte)41);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "HT",
                column: "Id",
                value: (byte)72);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "HU",
                column: "Id",
                value: (byte)74);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ID",
                column: "Id",
                value: (byte)77);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IE",
                column: "Id",
                value: (byte)80);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IL",
                column: "Id",
                value: (byte)81);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IN",
                column: "Id",
                value: (byte)76);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IQ",
                column: "Id",
                value: (byte)79);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IR",
                column: "Id",
                value: (byte)78);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IS",
                column: "Id",
                value: (byte)75);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "IT",
                column: "Id",
                value: (byte)82);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "JM",
                column: "Id",
                value: (byte)83);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "JO",
                column: "Id",
                value: (byte)85);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "JP",
                column: "Id",
                value: (byte)84);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KE",
                column: "Id",
                value: (byte)87);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KG",
                column: "Id",
                value: (byte)90);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KH",
                column: "Id",
                value: (byte)30);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KI",
                column: "Id",
                value: (byte)88);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KM",
                column: "Id",
                value: (byte)38);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KN",
                column: "Id",
                value: (byte)144);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KP",
                column: "Id",
                value: (byte)126);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KR",
                column: "Id",
                value: (byte)161);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KW",
                column: "Id",
                value: (byte)89);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "KZ",
                column: "Id",
                value: (byte)86);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LA",
                column: "Id",
                value: (byte)91);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LB",
                column: "Id",
                value: (byte)93);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LC",
                column: "Id",
                value: (byte)145);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LI",
                column: "Id",
                value: (byte)97);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LK",
                column: "Id",
                value: (byte)164);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LR",
                column: "Id",
                value: (byte)95);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LS",
                column: "Id",
                value: (byte)94);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LT",
                column: "Id",
                value: (byte)98);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LU",
                column: "Id",
                value: (byte)99);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LV",
                column: "Id",
                value: (byte)92);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "LY",
                column: "Id",
                value: (byte)96);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MA",
                column: "Id",
                value: (byte)115);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MC",
                column: "Id",
                value: (byte)112);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MD",
                column: "Id",
                value: (byte)111);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ME",
                column: "Id",
                value: (byte)114);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MG",
                column: "Id",
                value: (byte)100);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MH",
                column: "Id",
                value: (byte)106);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MK",
                column: "Id",
                value: (byte)127);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ML",
                column: "Id",
                value: (byte)104);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MM",
                column: "Id",
                value: (byte)117);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MN",
                column: "Id",
                value: (byte)113);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MR",
                column: "Id",
                value: (byte)107);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MT",
                column: "Id",
                value: (byte)105);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MU",
                column: "Id",
                value: (byte)108);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MV",
                column: "Id",
                value: (byte)103);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MW",
                column: "Id",
                value: (byte)101);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MX",
                column: "Id",
                value: (byte)109);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MY",
                column: "Id",
                value: (byte)102);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "MZ",
                column: "Id",
                value: (byte)116);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NA",
                column: "Id",
                value: (byte)118);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NE",
                column: "Id",
                value: (byte)124);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NG",
                column: "Id",
                value: (byte)125);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NI",
                column: "Id",
                value: (byte)123);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NL",
                column: "Id",
                value: (byte)121);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NO",
                column: "Id",
                value: (byte)128);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NP",
                column: "Id",
                value: (byte)120);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NR",
                column: "Id",
                value: (byte)119);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "NZ",
                column: "Id",
                value: (byte)122);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "OM",
                column: "Id",
                value: (byte)129);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PA",
                column: "Id",
                value: (byte)133);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PE",
                column: "Id",
                value: (byte)136);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PG",
                column: "Id",
                value: (byte)134);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PH",
                column: "Id",
                value: (byte)137);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PK",
                column: "Id",
                value: (byte)130);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PL",
                column: "Id",
                value: (byte)138);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PS",
                column: "Id",
                value: (byte)132);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PT",
                column: "Id",
                value: (byte)139);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PW",
                column: "Id",
                value: (byte)131);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "PY",
                column: "Id",
                value: (byte)135);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "QA",
                column: "Id",
                value: (byte)140);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "RO",
                column: "Id",
                value: (byte)141);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "RS",
                column: "Id",
                value: (byte)152);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "RU",
                column: "Id",
                value: (byte)142);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "RW",
                column: "Id",
                value: (byte)143);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SA",
                column: "Id",
                value: (byte)150);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SB",
                column: "Id",
                value: (byte)158);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SC",
                column: "Id",
                value: (byte)153);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SD",
                column: "Id",
                value: (byte)165);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SE",
                column: "Id",
                value: (byte)167);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SG",
                column: "Id",
                value: (byte)155);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SI",
                column: "Id",
                value: (byte)157);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SK",
                column: "Id",
                value: (byte)156);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SL",
                column: "Id",
                value: (byte)154);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SM",
                column: "Id",
                value: (byte)148);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SN",
                column: "Id",
                value: (byte)151);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SO",
                column: "Id",
                value: (byte)159);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SR",
                column: "Id",
                value: (byte)166);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SS",
                column: "Id",
                value: (byte)162);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ST",
                column: "Id",
                value: (byte)149);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SV",
                column: "Id",
                value: (byte)52);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SY",
                column: "Id",
                value: (byte)169);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "SZ",
                column: "Id",
                value: (byte)56);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TD",
                column: "Id",
                value: (byte)34);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TG",
                column: "Id",
                value: (byte)175);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TH",
                column: "Id",
                value: (byte)173);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TJ",
                column: "Id",
                value: (byte)171);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TL",
                column: "Id",
                value: (byte)174);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TM",
                column: "Id",
                value: (byte)180);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TN",
                column: "Id",
                value: (byte)178);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TO",
                column: "Id",
                value: (byte)176);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TR",
                column: "Id",
                value: (byte)179);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TT",
                column: "Id",
                value: (byte)177);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TV",
                column: "Id",
                value: (byte)181);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TW",
                column: "Id",
                value: (byte)170);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "TZ",
                column: "Id",
                value: (byte)172);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "UA",
                column: "Id",
                value: (byte)183);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "UG",
                column: "Id",
                value: (byte)182);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "US",
                column: "Id",
                value: (byte)186);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "UY",
                column: "Id",
                value: (byte)187);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "UZ",
                column: "Id",
                value: (byte)188);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "VC",
                column: "Id",
                value: (byte)146);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "VE",
                column: "Id",
                value: (byte)190);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "VN",
                column: "Id",
                value: (byte)191);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "VU",
                column: "Id",
                value: (byte)189);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "WS",
                column: "Id",
                value: (byte)147);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "YE",
                column: "Id",
                value: (byte)192);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ZA",
                column: "Id",
                value: (byte)160);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ZM",
                column: "Id",
                value: (byte)193);

            migrationBuilder.UpdateData(
                table: "Country",
                keyColumn: "ISO",
                keyValue: "ZW",
                column: "Id",
                value: (byte)194);

            migrationBuilder.UpdateData(
                table: "Definition",
                keyColumn: "Key",
                keyValue: "F",
                column: "Description",
                value: "Full time 8 paid holidays ");
        }
    }
}
