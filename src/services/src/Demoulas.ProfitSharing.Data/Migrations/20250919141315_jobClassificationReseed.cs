using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demoulas.ProfitSharing.Data.Migrations
{
    /// <inheritdoc />
    public partial class jobClassificationReseed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "1",
                column: "NAME",
                value: "MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "10",
                column: "NAME",
                value: "FRONT END MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "11",
                column: "NAME",
                value: "ASST HEAD CASHIER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "13",
                column: "NAME",
                value: "CASHIERS - AM");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "2",
                column: "NAME",
                value: "ASSISTANT MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "4",
                column: "NAME",
                value: "SPIRITS MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "5",
                column: "NAME",
                value: "ASST SPIRITS MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "6",
                column: "NAME",
                value: "SPIRITS CLERK - FT");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "7",
                column: "NAME",
                value: "SPIRITS CLERK - PT");

            migrationBuilder.InsertData(
                table: "PAY_CLASSIFICATION",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { "0", "01" },
                    { "14", "CASHIERS - PM" },
                    { "15", "CASHIER 14-15" },
                    { "16", "SACKERS - AM" },
                    { "17", "SACKERS - PM" },
                    { "18", "SACKERS 14-15" },
                    { "19", "AVAILABLE" },
                    { "20", "OFFICE MANAGER" },
                    { "21", "ASST OFFICE MANAGER" },
                    { "22", "COURTESY BOOTH - FT" },
                    { "23", "COURTESY BOOTH - PT" },
                    { "24", "POS - FULL TIME" },
                    { "25", "CLERK - FULL TIME AP" },
                    { "26", "CLERKS - FULL TIME AR" },
                    { "27", "CLERKS - FULL TIME GROC" },
                    { "28", "CLERKS - FULL TIME PERSH" },
                    { "29", "CLERKS F-T WAREHOUSE" },
                    { "30", "MERCHANDISER" },
                    { "31", "GROCERY MANAGER" },
                    { "32", "ENDS - PART TIME" },
                    { "33", "FIRST MEAT CUTTER" },
                    { "35", "FT BAKER/BENCH" },
                    { "36", "MARKETS KITCHEN PT 16-17" },
                    { "37", "CAFE PART TIME" },
                    { "38", "RECEIVER" },
                    { "39", "LOSS PREVENTION" },
                    { "40", "MEAT CUTTERS" },
                    { "41", "APPR MEAT CUTTERS" },
                    { "42", "MEAT CUTTER (PART TIME)" },
                    { "44", "PART TIME SUBSHOP" },
                    { "45", "ASST SUB SHOP MANAGER" },
                    { "46", "SERVICE CASE - FULL TIME" },
                    { "47", "WRAPPERS - FULL TIME" },
                    { "48", "WRAPPERS - PART TIME AM" },
                    { "49", "WRAPPERS - PART TIME PM" },
                    { "50", "HEAD CLERK" },
                    { "51", "SUB SHOP MANAGER" },
                    { "52", "CLERKS - FULL TIME AM" },
                    { "53", "CLERKS - PART TIME AM" },
                    { "54", "CLERK - PART TIME PM" },
                    { "55", "POS - PART TIME" },
                    { "56", "MARKETS KITCHEN-ASST MGR" },
                    { "57", "MARKETS KITCHEN FT" },
                    { "58", "MARKETS KITCHEN PT" },
                    { "59", "KITCHEN MANAGER" },
                    { "62", "FT CAKE" },
                    { "63", "PT CAKE" },
                    { "64", "OVEN WORKER PT" },
                    { "65", "BENCH WORKER PT" },
                    { "66", "FORK LIFT OPR (REC)- AM" },
                    { "67", "FORK LIFT OPR (REC)- PM" },
                    { "68", "FORK LIFT OPR (SHIP)- AM" },
                    { "69", "FORK LIFT OPR (SHIP)- PM" },
                    { "70", "FORK LIFT OPR (MISC)- AM" },
                    { "71", "FORK LIFT OPR (MISC)- PM" },
                    { "72", "LOADER - AM" },
                    { "73", "LOADER - PM" },
                    { "74", "GENERAL WAREHOUSE - FT - AM" },
                    { "75", "GENERAL WAREHOUSE - PT - AM" },
                    { "77", "SELECTOR (PART-TIME) AM" },
                    { "78", "SELECTOR (PART-TIME) PM" },
                    { "79", "SELECTOR FULL TIME-AM" },
                    { "81", "SELECTOR (FULL-TIME) PM" },
                    { "82", "INSPECTOR" },
                    { "83", "GENERAL WAREHOUSE - FT - PM" },
                    { "84", "GENERAL WAREHOUSE - PT - PM" },
                    { "85", "DRIVER (TRAILER)" },
                    { "86", "EXCEL" },
                    { "87", "MECHANIC" },
                    { "89", "FACILITY OPERATIONS" },
                    { "90", "COMPUTER OPERATIONS" },
                    { "91", "SIGN SHOP" },
                    { "92", "INVENTORY" },
                    { "93", "PROGRAMMING" },
                    { "94", "HELP DESK" },
                    { "96", "TECHNICAL SUPPORT" },
                    { "97", "EXECUTIVE OFFICE" },
                    { "98", "TRAINING" },
                    { "AD1", "AD1-MANAGER" },
                    { "AD2", "AD2-RECEPTIONIST" },
                    { "DR1", "DR1-BARTENDER" },
                    { "DR2", "DR2-BUSSER" },
                    { "DR3", "DR3-HOSTESS" },
                    { "DR4", "DR4-MANAGER" },
                    { "DR5", "DR5-SERVER" },
                    { "DR6", "DR6-SERVER" },
                    { "FM1", "FM1-MAINTENANCE ATTENDANT" },
                    { "FM2", "FM2-MAINTENANCE ATTENDANT" },
                    { "FM3", "FM3-MANAGER-FACILITY MAINTENANCE" },
                    { "FM4", "FM4-MAINT ATTEND" },
                    { "FM5", "FM5-MANAGER" },
                    { "FT1", "FT1-BARTENDER" },
                    { "FT2", "FT2-MANAGER" },
                    { "FT3", "FT3-SERVER" },
                    { "GM1", "GM1-GOLF CART MAINT" },
                    { "GM2", "GM2-GOLF CART MAINT" },
                    { "GM3", "GM3-GROUNDS MAINTENANCE" },
                    { "GM4", "GM4-GROUNDS MAINTENANCE" },
                    { "GM5", "GM5-MANAGER" },
                    { "GM6", "GM6-MECHANIC" },
                    { "GR1", "GR1-BUSSER" },
                    { "GR2", "GR2-MANAGER" },
                    { "GR3", "GR3-SERVER" },
                    { "GR4", "GR4-SNACK SHACK" },
                    { "GR5", "GR5-POOLSIDE-GRILLE ROOM" },
                    { "KT1", "KT1-MANAGER" },
                    { "KT2", "KT2-CHEF" },
                    { "KT3", "KT3-CHEF-KITCHEN" },
                    { "KT4", "KT4-DISHWASHER" },
                    { "KT5", "KT5-DISHWASHER-KITCHEN" },
                    { "KT6", "KT6-PREP CHEF" },
                    { "LG1", "LG1-MANAGER" },
                    { "LG2", "LG2-LIFEGUARD" },
                    { "PS1", "PS1-PRO SHOP SERVICES" },
                    { "PS2", "PS2-PRO SHOP SERVICES" },
                    { "PS3", "PS3-MANAGER" },
                    { "PS4", "PS4-OUTSIDE SERVICES" },
                    { "PS5", "PS5-OUTSIDE SERVICES" },
                    { "PS6", "PS6-STARTER" },
                    { "PS7", "PS7-STARTER" },
                    { "TN1", "TN1-MANAGER" },
                    { "TN2", "TN2-TENNIS SERVICES" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "0");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "14");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "15");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "16");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "17");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "18");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "19");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "20");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "21");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "22");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "23");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "24");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "25");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "26");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "27");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "28");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "29");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "30");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "31");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "32");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "33");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "35");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "36");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "37");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "38");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "39");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "40");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "41");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "42");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "44");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "45");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "46");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "47");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "48");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "49");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "50");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "51");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "52");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "53");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "54");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "55");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "56");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "57");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "58");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "59");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "62");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "63");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "64");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "65");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "66");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "67");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "68");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "69");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "70");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "71");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "72");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "73");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "74");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "75");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "77");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "78");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "79");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "81");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "82");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "83");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "84");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "85");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "86");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "87");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "89");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "90");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "91");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "92");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "93");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "94");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "96");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "97");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "98");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "AD1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "AD2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "DR1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "DR2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "DR3");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "DR4");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "DR5");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "DR6");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "FM1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "FM2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "FM3");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "FM4");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "FM5");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "FT1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "FT2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "FT3");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GM1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GM2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GM3");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GM4");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GM5");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GM6");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GR1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GR2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GR3");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GR4");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "GR5");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "KT1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "KT2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "KT3");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "KT4");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "KT5");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "KT6");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "LG1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "LG2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "PS1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "PS2");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "PS3");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "PS4");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "PS5");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "PS6");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "PS7");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "TN1");

            migrationBuilder.DeleteData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "TN2");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "1",
                column: "NAME",
                value: "1-MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "10",
                column: "NAME",
                value: "10-FRONT END MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "11",
                column: "NAME",
                value: "11-ASST HEAD CASHIER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "13",
                column: "NAME",
                value: "13-CASHIERS - AM");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "2",
                column: "NAME",
                value: "2-ASSISTANT MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "4",
                column: "NAME",
                value: "4-SPIRITS MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "5",
                column: "NAME",
                value: "5-ASST SPIRITS MANAGER");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "6",
                column: "NAME",
                value: "6-SPIRITS CLERK - FT");

            migrationBuilder.UpdateData(
                table: "PAY_CLASSIFICATION",
                keyColumn: "ID",
                keyValue: "7",
                column: "NAME",
                value: "7-SPIRITS CLERK - PT");
        }
    }
}
