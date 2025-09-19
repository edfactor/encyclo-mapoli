using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class PayClassificationMap : IEntityTypeConfiguration<PayClassification>
{
    public void Configure(EntityTypeBuilder<PayClassification> builder)
    {
        builder.ToTable("PAY_CLASSIFICATION");
        builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .HasMaxLength(4)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .IsRequired();

        _ = builder.Property(e => e.Name)
            .HasMaxLength(64)
            .IsRequired()
            .HasColumnName("NAME")
            .HasComment("Pay Classification");

        // Full seed (string IDs) migrated from legacy numeric + new alphanumeric codes.
        // Names intentionally without numeric prefixes for report grouping logic.
        builder.HasData(
            new PayClassification { Id = "0", Name = "01" },
            new PayClassification { Id = "1", Name = "MANAGER" },
            new PayClassification { Id = "2", Name = "ASSISTANT MANAGER" },
            new PayClassification { Id = "4", Name = "SPIRITS MANAGER" },
            new PayClassification { Id = "5", Name = "ASST SPIRITS MANAGER" },
            new PayClassification { Id = "6", Name = "SPIRITS CLERK - FT" },
            new PayClassification { Id = "7", Name = "SPIRITS CLERK - PT" },
            new PayClassification { Id = "10", Name = "FRONT END MANAGER" },
            new PayClassification { Id = "11", Name = "ASST HEAD CASHIER" },
            new PayClassification { Id = "13", Name = "CASHIERS - AM" },
            new PayClassification { Id = "14", Name = "CASHIERS - PM" },
            new PayClassification { Id = "15", Name = "CASHIER 14-15" },
            new PayClassification { Id = "16", Name = "SACKERS - AM" },
            new PayClassification { Id = "17", Name = "SACKERS - PM" },
            new PayClassification { Id = "18", Name = "SACKERS 14-15" },
            new PayClassification { Id = "19", Name = "AVAILABLE" },
            new PayClassification { Id = "20", Name = "OFFICE MANAGER" },
            new PayClassification { Id = "21", Name = "ASST OFFICE MANAGER" },
            new PayClassification { Id = "22", Name = "COURTESY BOOTH - FT" },
            new PayClassification { Id = "23", Name = "COURTESY BOOTH - PT" },
            new PayClassification { Id = "24", Name = "POS - FULL TIME" },
            new PayClassification { Id = "25", Name = "CLERK - FULL TIME AP" },
            new PayClassification { Id = "26", Name = "CLERKS - FULL TIME AR" },
            new PayClassification { Id = "27", Name = "CLERKS - FULL TIME GROC" },
            new PayClassification { Id = "28", Name = "CLERKS - FULL TIME PERSH" },
            new PayClassification { Id = "29", Name = "CLERKS F-T WAREHOUSE" },
            new PayClassification { Id = "30", Name = "MERCHANDISER" },
            new PayClassification { Id = "31", Name = "GROCERY MANAGER" },
            new PayClassification { Id = "32", Name = "ENDS - PART TIME" },
            new PayClassification { Id = "33", Name = "FIRST MEAT CUTTER" },
            new PayClassification { Id = "35", Name = "FT BAKER/BENCH" },
            new PayClassification { Id = "36", Name = "MARKETS KITCHEN PT 16-17" },
            new PayClassification { Id = "37", Name = "CAFE PART TIME" },
            new PayClassification { Id = "38", Name = "RECEIVER" },
            new PayClassification { Id = "39", Name = "LOSS PREVENTION" },
            new PayClassification { Id = "40", Name = "MEAT CUTTERS" },
            new PayClassification { Id = "41", Name = "APPR MEAT CUTTERS" },
            new PayClassification { Id = "42", Name = "MEAT CUTTER (PART TIME)" },
            new PayClassification { Id = "44", Name = "PART TIME SUBSHOP" },
            new PayClassification { Id = "45", Name = "ASST SUB SHOP MANAGER" },
            new PayClassification { Id = "46", Name = "SERVICE CASE - FULL TIME" },
            new PayClassification { Id = "47", Name = "WRAPPERS - FULL TIME" },
            new PayClassification { Id = "48", Name = "WRAPPERS - PART TIME AM" },
            new PayClassification { Id = "49", Name = "WRAPPERS - PART TIME PM" },
            new PayClassification { Id = "50", Name = "HEAD CLERK" },
            new PayClassification { Id = "51", Name = "SUB SHOP MANAGER" },
            new PayClassification { Id = "52", Name = "CLERKS - FULL TIME AM" },
            new PayClassification { Id = "53", Name = "CLERKS - PART TIME AM" },
            new PayClassification { Id = "54", Name = "CLERK - PART TIME PM" },
            new PayClassification { Id = "55", Name = "POS - PART TIME" },
            new PayClassification { Id = "56", Name = "MARKETS KITCHEN-ASST MGR" },
            new PayClassification { Id = "57", Name = "MARKETS KITCHEN FT" },
            new PayClassification { Id = "58", Name = "MARKETS KITCHEN PT" },
            new PayClassification { Id = "59", Name = "KITCHEN MANAGER" },
            new PayClassification { Id = "62", Name = "FT CAKE" },
            new PayClassification { Id = "63", Name = "PT CAKE" },
            new PayClassification { Id = "64", Name = "OVEN WORKER PT" },
            new PayClassification { Id = "65", Name = "BENCH WORKER PT" },
            new PayClassification { Id = "66", Name = "FORK LIFT OPR (REC)- AM" },
            new PayClassification { Id = "67", Name = "FORK LIFT OPR (REC)- PM" },
            new PayClassification { Id = "68", Name = "FORK LIFT OPR (SHIP)- AM" },
            new PayClassification { Id = "69", Name = "FORK LIFT OPR (SHIP)- PM" },
            new PayClassification { Id = "70", Name = "FORK LIFT OPR (MISC)- AM" },
            new PayClassification { Id = "71", Name = "FORK LIFT OPR (MISC)- PM" },
            new PayClassification { Id = "72", Name = "LOADER - AM" },
            new PayClassification { Id = "73", Name = "LOADER - PM" },
            new PayClassification { Id = "74", Name = "GENERAL WAREHOUSE - FT - AM" },
            new PayClassification { Id = "75", Name = "GENERAL WAREHOUSE - PT - AM" },
            new PayClassification { Id = "77", Name = "SELECTOR (PART-TIME) AM" },
            new PayClassification { Id = "78", Name = "SELECTOR (PART-TIME) PM" },
            new PayClassification { Id = "79", Name = "SELECTOR FULL TIME-AM" },
            new PayClassification { Id = "81", Name = "SELECTOR (FULL-TIME) PM" },
            new PayClassification { Id = "82", Name = "INSPECTOR" },
            new PayClassification { Id = "83", Name = "GENERAL WAREHOUSE - FT - PM" },
            new PayClassification { Id = "84", Name = "GENERAL WAREHOUSE - PT - PM" },
            new PayClassification { Id = "85", Name = "DRIVER (TRAILER)" },
            new PayClassification { Id = "86", Name = "EXCEL" },
            new PayClassification { Id = "87", Name = "MECHANIC" },
            new PayClassification { Id = "89", Name = "FACILITY OPERATIONS" },
            new PayClassification { Id = "90", Name = "COMPUTER OPERATIONS" },
            new PayClassification { Id = "91", Name = "SIGN SHOP" },
            new PayClassification { Id = "92", Name = "INVENTORY" },
            new PayClassification { Id = "93", Name = "PROGRAMMING" },
            new PayClassification { Id = "94", Name = "HELP DESK" },
            new PayClassification { Id = "96", Name = "TECHNICAL SUPPORT" },
            new PayClassification { Id = "97", Name = "EXECUTIVE OFFICE" },
            new PayClassification { Id = "98", Name = "TRAINING" },
            // Alphanumeric additions
            new PayClassification { Id = "AD1", Name = "AD1-MANAGER" },
            new PayClassification { Id = "AD2", Name = "AD2-RECEPTIONIST" },
            new PayClassification { Id = "DR1", Name = "DR1-BARTENDER" },
            new PayClassification { Id = "DR2", Name = "DR2-BUSSER" },
            new PayClassification { Id = "DR3", Name = "DR3-HOSTESS" },
            new PayClassification { Id = "DR4", Name = "DR4-MANAGER" },
            new PayClassification { Id = "DR5", Name = "DR5-SERVER" },
            new PayClassification { Id = "DR6", Name = "DR6-SERVER" },
            new PayClassification { Id = "FM1", Name = "FM1-MAINTENANCE ATTENDANT" },
            new PayClassification { Id = "FM2", Name = "FM2-MAINTENANCE ATTENDANT" },
            new PayClassification { Id = "FM3", Name = "FM3-MANAGER-FACILITY MAINTENANCE" },
            new PayClassification { Id = "FM4", Name = "FM4-MAINT ATTEND" },
            new PayClassification { Id = "FM5", Name = "FM5-MANAGER" },
            new PayClassification { Id = "FT1", Name = "FT1-BARTENDER" },
            new PayClassification { Id = "FT2", Name = "FT2-MANAGER" },
            new PayClassification { Id = "FT3", Name = "FT3-SERVER" },
            new PayClassification { Id = "GM1", Name = "GM1-GOLF CART MAINT" },
            new PayClassification { Id = "GM2", Name = "GM2-GOLF CART MAINT" },
            new PayClassification { Id = "GM3", Name = "GM3-GROUNDS MAINTENANCE" },
            new PayClassification { Id = "GM4", Name = "GM4-GROUNDS MAINTENANCE" },
            new PayClassification { Id = "GM5", Name = "GM5-MANAGER" },
            new PayClassification { Id = "GM6", Name = "GM6-MECHANIC" },
            new PayClassification { Id = "GR1", Name = "GR1-BUSSER" },
            new PayClassification { Id = "GR2", Name = "GR2-MANAGER" },
            new PayClassification { Id = "GR3", Name = "GR3-SERVER" },
            new PayClassification { Id = "GR4", Name = "GR4-SNACK SHACK" },
            new PayClassification { Id = "GR5", Name = "GR5-POOLSIDE-GRILLE ROOM" },
            new PayClassification { Id = "KT1", Name = "KT1-MANAGER" },
            new PayClassification { Id = "KT2", Name = "KT2-CHEF" },
            new PayClassification { Id = "KT3", Name = "KT3-CHEF-KITCHEN" },
            new PayClassification { Id = "KT4", Name = "KT4-DISHWASHER" },
            new PayClassification { Id = "KT5", Name = "KT5-DISHWASHER-KITCHEN" },
            new PayClassification { Id = "KT6", Name = "KT6-PREP CHEF" },
            new PayClassification { Id = "LG1", Name = "LG1-MANAGER" },
            new PayClassification { Id = "LG2", Name = "LG2-LIFEGUARD" },
            new PayClassification { Id = "PS1", Name = "PS1-PRO SHOP SERVICES" },
            new PayClassification { Id = "PS2", Name = "PS2-PRO SHOP SERVICES" },
            new PayClassification { Id = "PS3", Name = "PS3-MANAGER" },
            new PayClassification { Id = "PS4", Name = "PS4-OUTSIDE SERVICES" },
            new PayClassification { Id = "PS5", Name = "PS5-OUTSIDE SERVICES" },
            new PayClassification { Id = "PS6", Name = "PS6-STARTER" },
            new PayClassification { Id = "PS7", Name = "PS7-STARTER" },
            new PayClassification { Id = "TN1", Name = "TN1-MANAGER" },
            new PayClassification { Id = "TN2", Name = "TN2-TENNIS SERVICES" }
        );
    }
}
