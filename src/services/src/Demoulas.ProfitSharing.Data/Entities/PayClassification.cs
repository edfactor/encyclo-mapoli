using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31887785/DEMOGRAPHICS
/// </summary>

public sealed class PayClassification : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte ZeroOne = 0; // at least 4 people in prod have this Pay Classification/JobClass (see PROFITSHARE)
        public const byte Manager = 1;
        public const byte AssistantManager = 2;
        public const byte SpiritsClerkPt = 7;  //  at least 1 person in prod has this Pay Classification/JobClass (see PROFITSHARE)
        public const byte FrontEndManager = 10;
        public const byte AssistantHeadCashier = 11;
        public const byte CashiersAm = 13;
        public const byte CashiersPm = 14;
        public const byte Cashiers1415 = 15;
        public const byte SackersAm = 16;
        public const byte SackersPm = 17;
        public const byte Sackers1415 = 18;
        public const byte StoreMaintenance = 19;
        public const byte OfficeManager = 20;
        public const byte CourtesyBoothAm = 22;
        public const byte CourtesyBoothPm = 23;
        public const byte PosFullTime = 24;
        public const byte ClerkFullTimeAp = 25;
        public const byte ClerksFullTimeAr = 26;
        public const byte ClerksFullTimeGroc = 27;
        public const byte ClerksFullTimePerishables = 28;
        public const byte ClerksFullTimeWarehouse = 29;
        public const byte Merchandiser = 30;
        public const byte GroceryManager = 31;
        public const byte EndsPartTime = 32;
        public const byte FirstMeatCutter = 33;
        public const byte NotUsed35 = 35;
        public const byte MarketsKitchenPartTime = 36;
        public const byte CafePartTime = 37;
        public const byte Receiver = 38;
        public const byte NotUsed39 = 39;
        public const byte MeatCutters = 40;
        public const byte ApprMeatCutters = 41;
        public const byte MeatCutterPartTime = 42;
        public const byte TraineeMeatCutter = 43;
        public const byte PartTimeSubshop = 44;
        public const byte AsstSubShopManager = 45;
        public const byte ServiceCaseFullTime = 46;
        public const byte WrappersFullTime = 47;
        public const byte WrappersPartTimeAm = 48;
        public const byte WrappersPartTimePm = 49;
        public const byte HeadClerk = 50;
        public const byte SubShopManager = 51;
        public const byte ClerksFullTimeAm = 52;
        public const byte ClerksPartTimeAm = 53;
        public const byte ClerksPartTimePm = 54;
        public const byte PosPartTime = 55;
        public const byte MarketsKitchenAsstMgr = 56;
        public const byte MarketsKitchenFt = 57;
        public const byte MarketsKitchenPt = 58;
        public const byte KitchenManager = 59;
        public const byte NotUsed60 = 60;
        public const byte PtBakeryMerchandiser = 61;
        public const byte FtCakeAndCreams = 62;
        public const byte CakeAndCreamPt = 63;
        public const byte OverWorkerPt = 64;
        public const byte BenchWorkerPt = 65;
        public const byte ForkLiftOprRecAm = 66;
        public const byte ForkLiftOprRecPm = 67;
        public const byte ForkLiftOprShipAm = 68;
        public const byte ForkLiftOprShipPm = 69;
        public const byte ForkLiftOprMiscAm = 70;
        public const byte ForkLiftOprMiscPm = 71;
        public const byte LoaderAm = 72;
        public const byte LoaderPm = 73;
        public const byte WhseMaintenanceAm = 74;
        public const byte WhseMaintenancePm = 75;
        public const byte SelectorPartTimeAm = 77;
        public const byte SelectorPartTimePm = 78;
        public const byte SelectorFullTimeAm = 79;
        public const byte TempFullTime = 80;
        public const byte SelectorFullTimePm = 81;
        public const byte Inspector = 82;
        public const byte GeneralWarehouseAm = 83;
        public const byte GeneralWarehousePm = 84;
        public const byte DriverTrailer = 85;
        public const byte DriverStraight = 86;
        public const byte Mechanic = 87;
        public const byte GaragePm = 88;
        public const byte FacilityOperations = 89;
        public const byte ComputerOperations = 90;
        public const byte SignShop = 91;
        public const byte Inventory = 92;
        public const byte Programming = 93;
        public const byte HelpDesk = 94;
        public const byte Defunct = 95;
        public const byte TechnicalSupport = 96;
        public const byte Training = 98;
    }

    public byte Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<Demographic>? Employees { get; set; }
}
