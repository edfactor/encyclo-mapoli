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
            new PayClassification { Id = PayClassification.Constants.ZeroOne, Name = "01" },
            new PayClassification { Id = PayClassification.Constants.Manager, Name = "MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantManager, Name = "ASSISTANT MANAGER" },
            new PayClassification { Id = PayClassification.Constants.SpiritsManager, Name = "SPIRITS MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AsstSpiritsManager, Name = "ASST SPIRITS MANAGER" },
            new PayClassification { Id = PayClassification.Constants.SpiritsClerkFt, Name = "SPIRITS CLERK - FT" },
            new PayClassification { Id = PayClassification.Constants.SpiritsClerkPt, Name = "SPIRITS CLERK - PT" },
            new PayClassification { Id = PayClassification.Constants.FrontEndManager, Name = "FRONT END MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AssistantHeadCashier, Name = "ASST HEAD CASHIER" },
            new PayClassification { Id = PayClassification.Constants.CashiersAm, Name = "CASHIERS - AM" },
            new PayClassification { Id = PayClassification.Constants.CashiersPm, Name = "CASHIERS - PM" },
            new PayClassification { Id = PayClassification.Constants.Cashiers1415, Name = "CASHIER 14-15" },
            new PayClassification { Id = PayClassification.Constants.SackersAm, Name = "SACKERS - AM" },
            new PayClassification { Id = PayClassification.Constants.SackersPm, Name = "SACKERS - PM" },
            new PayClassification { Id = PayClassification.Constants.Sackers1415, Name = "SACKERS 14-15" },
            new PayClassification { Id = PayClassification.Constants.Available, Name = "AVAILABLE" },
            new PayClassification { Id = PayClassification.Constants.OfficeManager, Name = "OFFICE MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AsstOfficeManager, Name = "ASST OFFICE MANAGER" },
            new PayClassification { Id = PayClassification.Constants.CourtesyBoothFt, Name = "COURTESY BOOTH - FT" },
            new PayClassification { Id = PayClassification.Constants.CourtesyBoothPt, Name = "COURTESY BOOTH - PT" },
            new PayClassification { Id = PayClassification.Constants.PosFullTime, Name = "POS - FULL TIME" },
            new PayClassification { Id = PayClassification.Constants.ClerkFullTimeAp, Name = "CLERK - FULL TIME AP" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeAr, Name = "CLERKS - FULL TIME AR" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeGroc, Name = "CLERKS - FULL TIME GROC" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimePerishables, Name = "CLERKS - FULL TIME PERSH" },
            new PayClassification { Id = PayClassification.Constants.ClerksFullTimeWarehouse, Name = "CLERKS F-T WAREHOUSE" },
            new PayClassification { Id = PayClassification.Constants.Merchandiser, Name = "MERCHANDISER" },
            new PayClassification { Id = PayClassification.Constants.GroceryManager, Name = "GROCERY MANAGER" },
            new PayClassification { Id = PayClassification.Constants.EndsPartTime, Name = "ENDS - PART TIME" },
            new PayClassification { Id = PayClassification.Constants.FirstMeatCutter, Name = "FIRST MEAT CUTTER" },
            new PayClassification { Id = PayClassification.Constants.FtBakerBench, Name = "FT BAKER/BENCH" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenPt1617, Name = "MARKETS KITCHEN PT 16-17" },
            new PayClassification { Id = PayClassification.Constants.CafePartTime, Name = "CAFE PART TIME" },
            new PayClassification { Id = PayClassification.Constants.Receiver, Name = "RECEIVER" },
            new PayClassification { Id = PayClassification.Constants.LossPrevention, Name = "LOSS PREVENTION" },
            new PayClassification { Id = PayClassification.Constants.MeatCutters, Name = "MEAT CUTTERS" },
            new PayClassification { Id = PayClassification.Constants.ApprMeatCutters, Name = "APPR MEAT CUTTERS" },
            new PayClassification { Id = PayClassification.Constants.MeatCutterPartTime, Name = "MEAT CUTTER (PART TIME)" },
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
            new PayClassification { Id = PayClassification.Constants.ClerksPartTimePm, Name = "CLERK - PART TIME PM" },
            new PayClassification { Id = PayClassification.Constants.PosPartTime, Name = "POS - PART TIME" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenAsstMgr, Name = "MARKETS KITCHEN-ASST MGR" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenFt, Name = "MARKETS KITCHEN FT" },
            new PayClassification { Id = PayClassification.Constants.MarketsKitchenPt, Name = "MARKETS KITCHEN PT" },
            new PayClassification { Id = PayClassification.Constants.KitchenManager, Name = "KITCHEN MANAGER" },
            new PayClassification { Id = PayClassification.Constants.FtCake, Name = "FT CAKE" },
            new PayClassification { Id = PayClassification.Constants.PtCake, Name = "PT CAKE" },
            new PayClassification { Id = PayClassification.Constants.OvenWorkerPt, Name = "OVEN WORKER PT" },
            new PayClassification { Id = PayClassification.Constants.BenchWorkerPt, Name = "BENCH WORKER PT" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprRecAm, Name = "FORK LIFT OPR (REC)- AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprRecPm, Name = "FORK LIFT OPR (REC)- PM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprShipAm, Name = "FORK LIFT OPR (SHIP)- AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprShipPm, Name = "FORK LIFT OPR (SHIP)- PM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprMiscAm, Name = "FORK LIFT OPR (MISC)- AM" },
            new PayClassification { Id = PayClassification.Constants.ForkLiftOprMiscPm, Name = "FORK LIFT OPR (MISC)- PM" },
            new PayClassification { Id = PayClassification.Constants.LoaderAm, Name = "LOADER - AM" },
            new PayClassification { Id = PayClassification.Constants.LoaderPm, Name = "LOADER - PM" },
            new PayClassification { Id = PayClassification.Constants.GeneralWarehouseFtAm, Name = "GENERAL WAREHOUSE - FT - AM" },
            new PayClassification { Id = PayClassification.Constants.GeneralWarehousePtAm, Name = "GENERAL WAREHOUSE - PT - AM" },
            new PayClassification { Id = PayClassification.Constants.SelectorPartTimeAm, Name = "SELECTOR (PART-TIME) AM" },
            new PayClassification { Id = PayClassification.Constants.SelectorPartTimePm, Name = "SELECTOR (PART-TIME) PM" },
            new PayClassification { Id = PayClassification.Constants.SelectorFullTimeAm, Name = "SELECTOR FULL TIME-AM" },
            new PayClassification { Id = PayClassification.Constants.SelectorFullTimePm, Name = "SELECTOR (FULL-TIME) PM" },
            new PayClassification { Id = PayClassification.Constants.Inspector, Name = "INSPECTOR" },
            new PayClassification { Id = PayClassification.Constants.GeneralWarehouseFtPm, Name = "GENERAL WAREHOUSE - FT - PM" },
            new PayClassification { Id = PayClassification.Constants.GeneralWarehousePtPm, Name = "GENERAL WAREHOUSE - PT - PM" },
            new PayClassification { Id = PayClassification.Constants.DriverTrailer, Name = "DRIVER (TRAILER)" },
            new PayClassification { Id = PayClassification.Constants.DriverExcel, Name = "EXCEL" },
            new PayClassification { Id = PayClassification.Constants.Mechanic, Name = "MECHANIC" },
            new PayClassification { Id = PayClassification.Constants.FacilityOperations, Name = "FACILITY OPERATIONS" },
            new PayClassification { Id = PayClassification.Constants.ComputerOperations, Name = "COMPUTER OPERATIONS" },
            new PayClassification { Id = PayClassification.Constants.SignShop, Name = "SIGN SHOP" },
            new PayClassification { Id = PayClassification.Constants.Inventory, Name = "INVENTORY" },
            new PayClassification { Id = PayClassification.Constants.Programming, Name = "PROGRAMMING" },
            new PayClassification { Id = PayClassification.Constants.HelpDesk, Name = "HELP DESK" },
            new PayClassification { Id = PayClassification.Constants.TechnicalSupport, Name = "TECHNICAL SUPPORT" },
            new PayClassification { Id = PayClassification.Constants.ExecutiveOffice, Name = "EXECUTIVE OFFICE" },
            new PayClassification { Id = PayClassification.Constants.Training, Name = "TRAINING" },
            new PayClassification { Id = PayClassification.Constants.IndianRidge, Name = "Indian Ridge" },
            // Alphanumeric additions
            new PayClassification { Id = PayClassification.Constants.AdManager, Name = "AD1-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.AdReceptionist, Name = "AD2-RECEPTIONIST" },
            new PayClassification { Id = PayClassification.Constants.DrBartender, Name = "DR1-BARTENDER" },
            new PayClassification { Id = PayClassification.Constants.DrBusser, Name = "DR2-BUSSER" },
            new PayClassification { Id = PayClassification.Constants.DrHostess, Name = "DR3-HOSTESS" },
            new PayClassification { Id = PayClassification.Constants.DrManager, Name = "DR4-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.DrServer, Name = "DR5-SERVER" },
            new PayClassification { Id = PayClassification.Constants.DrServer2, Name = "DR6-SERVER" },
            new PayClassification { Id = PayClassification.Constants.FmMaintenanceAttendant, Name = "FM1-MAINTENANCE ATTENDANT" },
            new PayClassification { Id = PayClassification.Constants.FmMaintenanceAttendant2, Name = "FM2-MAINTENANCE ATTENDANT" },
            new PayClassification { Id = PayClassification.Constants.FmManagerFacilityMaintenance, Name = "FM3-MANAGER-FACILITY MAINTENANCE" },
            new PayClassification { Id = PayClassification.Constants.FmMaintAttend, Name = "FM4-MAINT ATTEND" },
            new PayClassification { Id = PayClassification.Constants.FmManager, Name = "FM5-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.FtBartender, Name = "FT1-BARTENDER" },
            new PayClassification { Id = PayClassification.Constants.FtManager, Name = "FT2-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.FtServer, Name = "FT3-SERVER" },
            new PayClassification { Id = PayClassification.Constants.GmGolfCartMaint, Name = "GM1-GOLF CART MAINT" },
            new PayClassification { Id = PayClassification.Constants.GmGolfCartMaint2, Name = "GM2-GOLF CART MAINT" },
            new PayClassification { Id = PayClassification.Constants.GmGroundsMaintenance, Name = "GM3-GROUNDS MAINTENANCE" },
            new PayClassification { Id = PayClassification.Constants.GmGroundsMaintenance2, Name = "GM4-GROUNDS MAINTENANCE" },
            new PayClassification { Id = PayClassification.Constants.GmManager, Name = "GM5-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.GmMechanic, Name = "GM6-MECHANIC" },
            new PayClassification { Id = PayClassification.Constants.GrBusser, Name = "GR1-BUSSER" },
            new PayClassification { Id = PayClassification.Constants.GrManager, Name = "GR2-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.GrServer, Name = "GR3-SERVER" },
            new PayClassification { Id = PayClassification.Constants.GrSnackShack, Name = "GR4-SNACK SHACK" },
            new PayClassification { Id = PayClassification.Constants.GrPoolSideGrilleRoom, Name = "GR5-POOLSIDE-GRILLE ROOM" },
            new PayClassification { Id = PayClassification.Constants.KtManager, Name = "KT1-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.KtChef, Name = "KT2-CHEF" },
            new PayClassification { Id = PayClassification.Constants.KtChefKitchen, Name = "KT3-CHEF-KITCHEN" },
            new PayClassification { Id = PayClassification.Constants.KtDishwasher, Name = "KT4-DISHWASHER" },
            new PayClassification { Id = PayClassification.Constants.KtDishwasherKitchen, Name = "KT5-DISHWASHER-KITCHEN" },
            new PayClassification { Id = PayClassification.Constants.KtPrepChef, Name = "KT6-PREP CHEF" },
            new PayClassification { Id = PayClassification.Constants.LgManager, Name = "LG1-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.LgLifeguard, Name = "LG2-LIFEGUARD" },
            new PayClassification { Id = PayClassification.Constants.PsProShopServices, Name = "PS1-PRO SHOP SERVICES" },
            new PayClassification { Id = PayClassification.Constants.PsProShopServices2, Name = "PS2-PRO SHOP SERVICES" },
            new PayClassification { Id = PayClassification.Constants.PsManager, Name = "PS3-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.PsOutsideServices, Name = "PS4-OUTSIDE SERVICES" },
            new PayClassification { Id = PayClassification.Constants.PsOutsideServices2, Name = "PS5-OUTSIDE SERVICES" },
            new PayClassification { Id = PayClassification.Constants.PsStarter, Name = "PS6-STARTER" },
            new PayClassification { Id = PayClassification.Constants.PsStarter2, Name = "PS7-STARTER" },
            new PayClassification { Id = PayClassification.Constants.TnManager, Name = "TN1-MANAGER" },
            new PayClassification { Id = PayClassification.Constants.TnTennisServices, Name = "TN2-TENNIS SERVICES" }
        );
    }
}
