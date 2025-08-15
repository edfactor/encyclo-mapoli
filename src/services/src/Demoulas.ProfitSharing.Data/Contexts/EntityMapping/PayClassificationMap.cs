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
            new PayClassification
            {
                Id = PayClassification.Constants.ZeroOne, Name = "01"
            }, // There are 4 people in the obfuscation dataset with this JOBCLASS 
            new PayClassification { Id = PayClassification.Constants.Manager, Name = "MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantManager, Name = "ASSISTANT MANAGER" },
            new PayClassification { Id = PayClassification.Constants.SpiritsManager, Name = "SPIRITS MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AsstSpiritsManager, Name = "ASST SPIRITS MANAGER" },
            new PayClassification { Id = PayClassification.Constants.SpiritsClerkFt, Name = "SPIRITS CLERK - FT" },
            new PayClassification { Id = PayClassification.Constants.SpiritsClerkPt, Name = "SPIRITS CLERK - PT" },
            new PayClassification { Id = PayClassification.Constants.FrontEndManager, Name = "FRONT END MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantHeadCashier, Name = "ASSISTANT HEAD CASHIER" },
            new PayClassification { Id = PayClassification.Constants.CashiersAm, Name = "CASHIERS - AM" },
            new PayClassification { Id = PayClassification.Constants.CashiersPm, Name = "CASHIERS - PM" },
            new PayClassification { Id = PayClassification.Constants.Cashiers1415, Name = "CASHIERS 14-15" },
            new PayClassification { Id = PayClassification.Constants.SackersAm, Name = "SACKERS - AM" },
            new PayClassification { Id = PayClassification.Constants.SackersPm, Name = "SACKERS - PM" },
            new PayClassification { Id = PayClassification.Constants.Sackers1415, Name = "SACKERS 14-15" },
            new PayClassification { Id = PayClassification.Constants.StoreMaintenance, Name = "STORE MAINTENANCE" },
            new PayClassification { Id = PayClassification.Constants.OfficeManager, Name = "OFFICE MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AsstOfficeManager, Name = "ASST OFFICE MANAGER" },
            new PayClassification { Id = PayClassification.Constants.CourtesyBoothAm, Name = "COURTESY BOOTH - AM" },
            new PayClassification { Id = PayClassification.Constants.CourtesyBoothPm, Name = "COURTESY BOOTH - PM" },
            new PayClassification { Id = PayClassification.Constants.PosFullTime, Name = "POS - FULL TIME" },
            new PayClassification { Id = PayClassification.Constants.ClerkFullTimeAp, Name = "CLERK -FULL TIME AP" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeAr, Name = "CLERKS - FULL TIME AR" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeGroc, Name = "CLERKS - FULL TIME GROC" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimePerishables, Name = "CLERKS - FULL TIME PERISHABLES" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeWarehouse, Name = "CLERKS - FULL TIME WAREHOUSE" },
            new PayClassification { Id = PayClassification.Constants.Merchandiser, Name = "MERCHANDISER" },
            new PayClassification { Id = PayClassification.Constants.GroceryManager, Name = "GROCERY MANAGER" },
            new PayClassification { Id = PayClassification.Constants.EndsPartTime, Name = "ENDS - PART TIME" },
            new PayClassification { Id = PayClassification.Constants.FirstMeatCutter, Name = "FIRST MEAT CUTTER" },
            new PayClassification { Id = PayClassification.Constants.NotUsed35, Name = "NOT USED" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenPartTime, Name = "MARKETS KITCHEN PT 16-17" },
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
            new PayClassification { Id = PayClassification.Constants.WrappersPartTimeAm, Name = "WRAPPERS - PART TIME AM" },
            new PayClassification { Id = PayClassification.Constants.WrappersPartTimePm, Name = "WRAPPERS - PART TIME PM" },
            new PayClassification { Id = PayClassification.Constants.HeadClerk, Name = "HEAD CLERK" },
            new PayClassification { Id = PayClassification.Constants.SubShopManager, Name = "SUB SHOP MANAGER" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeAm, Name = "CLERKS - FULL TIME AM" },
            new PayClassification { Id = PayClassification.Constants.ClerksPartTimeAm, Name = "CLERKS - PART TIME AM" },
            new PayClassification { Id = PayClassification.Constants.ClerksPartTimePm, Name = "CLERKS - PART TIME PM" },
            new PayClassification { Id = PayClassification.Constants.PosPartTime, Name = "POS - PART TIME" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenAsstMgr, Name = "MARKETS KITCHEN - ASST MGR" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenFt, Name = "MARKETS KITCHEN FT" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenPt, Name = "MARKETS KITCHEN PT" },
            new PayClassification { Id = PayClassification.Constants.KitchenManager, Name = "KITCHEN MANAGER" },
            new PayClassification { Id = PayClassification.Constants.NotUsed60, Name = "NOT USED" },
            new PayClassification { Id = PayClassification.Constants.PtBakeryMerchandiser, Name = "PT BAKERY MERCHANDISER" },
            new PayClassification { Id = PayClassification.Constants.FtCakeAndCreams, Name = "FT CAKE & CREAMS" },
            new PayClassification { Id = PayClassification.Constants.CakeAndCreamPt, Name = "CAKE & CREAM PT" },
            new PayClassification { Id = PayClassification.Constants.OverWorkerPt, Name = "OVER WORKER PT" },
            new PayClassification { Id = PayClassification.Constants.BenchWorkerPt, Name = "BENCH WORKER PT" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprRecAm, Name = "FORK LIFT OPR (REC) AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprRecPm, Name = "FORK LIFT OPR (REC) PM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprShipAm, Name = "FORK LIFT OPR (SHIP) AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprShipPm, Name = "FORK LIFT OPR (SHIP) PM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprMiscAm, Name = "FORK LIFT OPR (MISC.) AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprMiscPm, Name = "FORK LIFT OPR (MISC.) PM" },
            new PayClassification { Id = PayClassification.Constants.LoaderAm, Name = "LOADER - AM" },
            new PayClassification { Id = PayClassification.Constants.LoaderPm, Name = "LOADER - PM" },
            new PayClassification { Id = PayClassification.Constants.WhseMaintenanceAm, Name = "WHSE MAINTENANCE - AM" },
            new PayClassification { Id = PayClassification.Constants.WhseMaintenancePm, Name = "WHSE MAINTENANCE - PM" },
            new PayClassification { Id = PayClassification.Constants.SelectorPartTimeAm, Name = "SELECTOR PART TIME - AM" },
            new PayClassification { Id = PayClassification.Constants.SelectorPartTimePm, Name = "SELECTOR PART TIME - PM" },
            new PayClassification { Id = PayClassification.Constants.SelectorFullTimeAm, Name = "SELECTOR FULL TIME - AM" },
            new PayClassification { Id = PayClassification.Constants.TempFullTime, Name = "TEMP FULLTIME" },
            new PayClassification { Id = PayClassification.Constants.SelectorFullTimePm, Name = "SELECTOR FULL TIME - PM" },
            new PayClassification { Id = PayClassification.Constants.Inspector, Name = "INSPECTOR" },
            new PayClassification { Id = PayClassification.Constants.GeneralWarehouseAm, Name = "GENERAL WAREHOUSE - AM" },
            new PayClassification { Id = PayClassification.Constants.GeneralWarehousePm, Name = "GENERAL WAREHOUSE - PM" },
            new PayClassification { Id = PayClassification.Constants.DriverTrailer, Name = "DRIVER - TRAILER" },
            new PayClassification { Id = PayClassification.Constants.DriverStraight, Name = "DRIVER - STRAIGHT" },
            new PayClassification { Id = PayClassification.Constants.Mechanic, Name = "MECHANIC" },
            new PayClassification { Id = PayClassification.Constants.GaragePm, Name = "GARAGE - PM" },
            new PayClassification { Id = PayClassification.Constants.FacilityOperations, Name = "FACILITY OPERATIONS" },
            new PayClassification { Id = PayClassification.Constants.ComputerOperations, Name = "COMPUTER OPERATIONS" },
            new PayClassification { Id = PayClassification.Constants.SignShop, Name = "SIGN SHOP" },
            new PayClassification { Id = PayClassification.Constants.Inventory, Name = "INVENTORY" },
            new PayClassification { Id = PayClassification.Constants.Programming, Name = "PROGRAMMING" },
            new PayClassification { Id = PayClassification.Constants.HelpDesk, Name = "HELP DESK" },
            new PayClassification { Id = PayClassification.Constants.Defunct, Name = "DEFUNCT" },
            new PayClassification { Id = PayClassification.Constants.TechnicalSupport, Name = "TECHNICAL SUPPORT" },
            new PayClassification { Id = PayClassification.Constants.Training, Name = "TRAINING" },
            new PayClassification { Id = PayClassification.Constants.IndianRidge, Name = "Indian Ridge" }
        );
    }
}
