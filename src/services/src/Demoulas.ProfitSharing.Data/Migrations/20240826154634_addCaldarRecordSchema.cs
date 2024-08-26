using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class addCaldarRecordSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CALDAR_RECORD",
                schema: "MTPR",
                newName: "CALDAR_RECORD");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MTPR");

            migrationBuilder.RenameTable(
                name: "CALDAR_RECORD",
                newName: "CALDAR_RECORD",
                newSchema: "MTPR");

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
        }
    }
}
