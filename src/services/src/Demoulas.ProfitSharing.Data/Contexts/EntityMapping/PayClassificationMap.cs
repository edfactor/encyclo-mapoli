using Demoulas.ProfitSharing.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Demoulas.ProfitSharing.Data.Contexts.EntityMapping;

internal sealed class PayClassificationMap : IEntityTypeConfiguration<PayClassification>
{
    public void Configure(EntityTypeBuilder<PayClassification> builder)
    {
        builder.ToTable("PAY_CLASSIFICATION");
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
            new PayClassification { Id = PayClassification.Constants.ZeroOne, Name = "01" }, // There are 4 people in the obfuscation dataset with this JOBCLASS 
            new PayClassification { Id = PayClassification.Constants.Manager, Name = "MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantManager, Name = "ASSISTANT MANAGER" },
            new PayClassification { Id = PayClassification.Constants.SpiritsClerkPT, Name = "SPIRITS CLERK - PT" }, // 1 person in obfuscation dataset with this JOBCLASS
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
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenAsstMgr, Name = "MARKETS KITCHEN - ASST MGR" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenFT, Name = "MARKETS KITCHEN FT" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenPT, Name = "MARKETS KITCHEN PT" },
            new PayClassification { Id = PayClassification.Constants.KitchenManager, Name = "KITCHEN MANAGER" },
            new PayClassification { Id = PayClassification.Constants.NotUsed60, Name = "NOT USED" },
            new PayClassification { Id = PayClassification.Constants.PTBakeryMerchandiser, Name = "PT BAKERY MERCHANDISER" },
            new PayClassification { Id = PayClassification.Constants.FTCakeAndCreams, Name = "FT CAKE & CREAMS" },
            new PayClassification { Id = PayClassification.Constants.CakeAndCreamPT, Name = "CAKE & CREAM PT" },
            new PayClassification { Id = PayClassification.Constants.OverWorkerPT, Name = "OVER WORKER PT" },
            new PayClassification { Id = PayClassification.Constants.BenchWorkerPT, Name = "BENCH WORKER PT" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprRecAM, Name = "FORK LIFT OPR (REC) AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprRecPM, Name = "FORK LIFT OPR (REC) PM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprShipAM, Name = "FORK LIFT OPR (SHIP) AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprShipPM, Name = "FORK LIFT OPR (SHIP) PM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprMiscAM, Name = "FORK LIFT OPR (MISC.) AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprMiscPM, Name = "FORK LIFT OPR (MISC.) PM" },
            new PayClassification { Id = PayClassification.Constants.LoaderAM, Name = "LOADER - AM" },
            new PayClassification { Id = PayClassification.Constants.LoaderPM, Name = "LOADER - PM" },
            new PayClassification { Id = PayClassification.Constants.WhseMaintenanceAM, Name = "WHSE MAINTENANCE - AM" },
            new PayClassification { Id = PayClassification.Constants.WhseMaintenancePM, Name = "WHSE MAINTENANCE - PM" },
            new PayClassification { Id = PayClassification.Constants.SelectorPartTimeAM, Name = "SELECTOR PART TIME - AM" },
            new PayClassification { Id = PayClassification.Constants.SelectorPartTimePM, Name = "SELECTOR PART TIME - PM" },
            new PayClassification { Id = PayClassification.Constants.SelectorFullTimeAM, Name = "SELECTOR FULL TIME - AM" },
            new PayClassification { Id = PayClassification.Constants.TempFullTime, Name = "TEMP FULLTIME" },
            new PayClassification { Id = PayClassification.Constants.SelectorFullTimePM, Name = "SELECTOR FULL TIME - PM" },
            new PayClassification { Id = PayClassification.Constants.Inspector, Name = "INSPECTOR" },
            new PayClassification { Id = PayClassification.Constants.GeneralWarehouseAM, Name = "GENERAL WAREHOUSE - AM" },
            new PayClassification { Id = PayClassification.Constants.GeneralWarehousePM, Name = "GENERAL WAREHOUSE - PM" },
            new PayClassification { Id = PayClassification.Constants.DriverTrailer, Name = "DRIVER - TRAILER" },
            new PayClassification { Id = PayClassification.Constants.DriverStraight, Name = "DRIVER - STRAIGHT" },
            new PayClassification { Id = PayClassification.Constants.Mechanic, Name = "MECHANIC" },
            new PayClassification { Id = PayClassification.Constants.GaragePM, Name = "GARAGE - PM" },
            new PayClassification { Id = PayClassification.Constants.FacilityOperations, Name = "FACILITY OPERATIONS" },
            new PayClassification { Id = PayClassification.Constants.ComputerOperations, Name = "COMPUTER OPERATIONS" },
            new PayClassification { Id = PayClassification.Constants.SignShop, Name = "SIGN SHOP" },
            new PayClassification { Id = PayClassification.Constants.Inventory, Name = "INVENTORY" },
            new PayClassification { Id = PayClassification.Constants.Programming, Name = "PROGRAMMING" },
            new PayClassification { Id = PayClassification.Constants.HelpDesk, Name = "HELP DESK" },
            new PayClassification { Id = PayClassification.Constants.Defunct, Name = "DEFUNCT" },
            new PayClassification { Id = PayClassification.Constants.TechnicalSupport, Name = "TECHNICAL SUPPORT" },
            new PayClassification { Id = PayClassification.Constants.Training, Name = "TRAINING" }
        );
    }
}
