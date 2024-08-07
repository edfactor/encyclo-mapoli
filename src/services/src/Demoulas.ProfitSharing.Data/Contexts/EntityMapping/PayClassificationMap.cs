using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class PayClassificationMap : IEntityTypeConfiguration<PayClassification>
{
    public void Configure(EntityTypeBuilder<PayClassification> builder)
    {
        builder.ToTable("PayClassification");
        builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.Id)
            .HasPrecision(2)
            .ValueGeneratedNever()
            .HasConversion<byte>()
            .HasColumnName("ID")
            .IsRequired();

        _ = builder.Property(e => e.Name)
            .HasMaxLength(64)
            .IsRequired()
            .HasColumnName("NAME")
            .HasComment("Pay Classification");

        builder.HasData(
            new PayClassification { Id = PayClassification.Constants.Manager, Name = "MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantManager, Name = "ASSISTANT MANAGER" },
            new PayClassification { Id = PayClassification.Constants.FrontEndManager, Name = "FRONT END MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantHeadCashier, Name = "ASSISTANT HEAD CASHIER" },
            new PayClassification { Id = PayClassification.Constants.CashiersAM, Name = "CASHIERS - AM" },
            new PayClassification { Id = PayClassification.Constants.CashiersPM, Name = "CASHIERS - PM" },
            new PayClassification { Id = PayClassification.Constants.Cashiers14_15, Name = "CASHIERS 14-15" },
            new PayClassification { Id = PayClassification.Constants.SackersAM, Name = "SACKERS - AM" },
            new PayClassification { Id = PayClassification.Constants.SackersPM, Name = "SACKERS - PM" },
            new PayClassification { Id = PayClassification.Constants.Sackers14_15, Name = "SACKERS 14-15" },
            new PayClassification { Id = PayClassification.Constants.StoreMaintenance, Name = "STORE MAINTENANCE" },
            new PayClassification { Id = PayClassification.Constants.OfficeManager, Name = "OFFICE MANAGER" },
            new PayClassification { Id = PayClassification.Constants.CourtesyBoothAM, Name = "COURTESY BOOTH - AM" },
            new PayClassification { Id = PayClassification.Constants.CourtesyBoothPM, Name = "COURTESY BOOTH - PM" },
            new PayClassification { Id = PayClassification.Constants.POSFullTime, Name = "POS - FULL TIME" },
            new PayClassification { Id = PayClassification.Constants.ClerkFullTimeAP, Name = "CLERK -FULL TIME AP" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeAR, Name = "CLERKS - FULL TIME AR" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeGroc, Name = "CLERKS - FULL TIME GROC" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimePerishables, Name = "CLERKS - FULL TIME PERISHABLES" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeWarehouse, Name = "CLERKS - FULL TIME WAREHOUSE" },
            new PayClassification { Id = PayClassification.Constants.Merchandiser, Name = "MERCHANDISER" },
            new PayClassification { Id = PayClassification.Constants.GroceryManager, Name = "GROCERY MANAGER" },
            new PayClassification { Id = PayClassification.Constants.EndsPartTime, Name = "ENDS - PART TIME" },
            new PayClassification { Id = PayClassification.Constants.FirstMeatCutter, Name = "FIRST MEAT CUTTER" },
            new PayClassification { Id = PayClassification.Constants.NotUsed35, Name = "NOT USED" },
            new PayClassification { Id = PayClassification.Constants.CafePartTime, Name = "CAFE PART TIME" },
            new PayClassification { Id = PayClassification.Constants.Receiver, Name = "RECEIVER" },
            new PayClassification { Id = PayClassification.Constants.NotUsed39, Name = "NOT USED" },
            new PayClassification { Id = PayClassification.Constants.MeatCutters, Name = "MEAT CUTTERS" },
            new PayClassification { Id = PayClassification.Constants.ApprMeatCutters, Name = "APPR MEAT CUTTERS" },
            new PayClassification { Id = PayClassification.Constants.MeatCutterPartTime, Name = "MEAT CUTTER PART TIME" },
            new PayClassification { Id = PayClassification.Constants.TraineeMeatCutter, Name = "TRAINEE MEAT CUTTER" },
            new PayClassification { Id = PayClassification.Constants.PartTimeSubshop, Name = "PART TIME SUBSHOP" },
            new PayClassification { Id = PayClassification.Constants.AsstSubShopManager, Name = "ASST SUB SHOP MANAGER" },
            new PayClassification { Id = PayClassification.Constants.ServiceCaseFullTime, Name = "SERVICE CASE - FULL TIME" },
            new PayClassification { Id = PayClassification.Constants.WrappersFullTime, Name = "WRAPPERS - FULL TIME" },
            new PayClassification { Id = PayClassification.Constants.WrappersPartTimeAM, Name = "WRAPPERS - PART TIME AM" },
            new PayClassification { Id = PayClassification.Constants.WrappersPartTimePM, Name = "WRAPPERS - PART TIME PM" },
            new PayClassification { Id = PayClassification.Constants.HeadClerk, Name = "HEAD CLERK" },
            new PayClassification { Id = PayClassification.Constants.SubShopManager, Name = "SUB SHOP MANAGER" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeAM, Name = "CLERKS - FULL TIME AM" },
            new PayClassification { Id = PayClassification.Constants.ClerksPartTimeAM, Name = "CLERKS - PART TIME AM" },
            new PayClassification { Id = PayClassification.Constants.ClerksPartTimePM, Name = "CLERKS - PART TIME PM" },
            new PayClassification { Id = PayClassification.Constants.POSPartTime, Name = "POS - PART TIME" },
            new PayClassification { Id = 56, Name = "MARKETS KITCHEN - ASST MGR" },
            new PayClassification { Id = 57, Name = "MARKETS KITCHEN FT" },
            new PayClassification { Id = 58, Name = "MARKETS KITCHEN PT" },
            new PayClassification { Id = 59, Name = "KITCHEN MANAGER" },
            new PayClassification { Id = 60, Name = "NOT USED" },
            new PayClassification { Id = 61, Name = "PT BAKERY MERCHANDISER" },
            new PayClassification { Id = 62, Name = "FT CAKE & CREAMS" },
            new PayClassification { Id = 63, Name = "CAKE & CREAM PT" },
            new PayClassification { Id = 64, Name = "OVER WORKER PT" },
            new PayClassification { Id = 65, Name = "BENCH WORKER PT" },
            new PayClassification { Id = 66, Name = "FORK LIFT OPR (REC) AM" },
            new PayClassification { Id = 67, Name = "FORK LIFT OPR (REC) PM" },
            new PayClassification { Id = 68, Name = "FORK LIFT OPR (SHIP) AM" },
            new PayClassification { Id = 69, Name = "FORK LIFT OPR (SHIP) PM" },
            new PayClassification { Id = 70, Name = "FORK LIFT OPR (MISC.) AM" },
            new PayClassification { Id = 71, Name = "FORK LIFT OPR (MISC.) PM" },
            new PayClassification { Id = 72, Name = "LOADER - AM" },
            new PayClassification { Id = 73, Name = "LOADER - PM" },
            new PayClassification { Id = 74, Name = "WHSE MAINTENANCE - AM" },
            new PayClassification { Id = 75, Name = "WHSE MAINTENANCE - PM" },
            new PayClassification { Id = 77, Name = "SELECTOR PART TIME - AM" },
            new PayClassification { Id = 78, Name = "SELECTOR PART TIME - PM" },
            new PayClassification { Id = 79, Name = "SELECTOR FULL TIME - AM" },
            new PayClassification { Id = 80, Name = "TEMP FULLTIME" },
            new PayClassification { Id = 81, Name = "SELECTOR FULL TIME - PM" },
            new PayClassification { Id = 82, Name = "INSPECTOR" },
            new PayClassification { Id = 83, Name = "GENERAL WAREHOUSE - AM" },
            new PayClassification { Id = 84, Name = "GENERAL WAREHOUSE - PM" },
            new PayClassification { Id = 85, Name = "DRIVER - TRAILER" },
            new PayClassification { Id = 86, Name = "DRIVER - STRAIGHT" },
            new PayClassification { Id = 87, Name = "MECHANIC" },
            new PayClassification { Id = 88, Name = "GARAGE - PM" },
            new PayClassification { Id = 89, Name = "FACILITY OPERATIONS" },
            new PayClassification { Id = 90, Name = "COMPUTER OPERATIONS" },
            new PayClassification { Id = 91, Name = "SIGN SHOP" },
            new PayClassification { Id = 92, Name = "INVENTORY" },
            new PayClassification { Id = 93, Name = "PROGRAMMING" },
            new PayClassification { Id = 94, Name = "HELP DESK" },
            new PayClassification { Id = 95, Name = "DEFUNCT" },
            new PayClassification { Id = 96, Name = "TECHNICAL SUPPORT" },
            new PayClassification { Id = 98, Name = "TRAINING" }
        );
    }
}
