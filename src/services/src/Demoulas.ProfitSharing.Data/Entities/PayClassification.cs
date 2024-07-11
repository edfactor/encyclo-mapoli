using Demoulas.ProfitSharing.Data.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31887785/DEMOGRAPHICS
/// </summary>

public sealed class PayClassification : ILookupTable<byte>
{
    public static class Constants
    {
        public const byte Manager = 1;
        public const byte AssistantManager = 2;
        public const byte FrontEndManager = 10;
        public const byte AssistantHeadCashier = 11;
        public const byte CashiersAM = 13;
        public const byte CashiersPM = 14;
        public const byte Cashiers14_15 = 15;
        public const byte SackersAM = 16;
        public const byte SackersPM = 17;
        public const byte Sackers14_15 = 18;
        public const byte StoreMaintenance = 19;
        public const byte OfficeManager = 20;
        public const byte CourtesyBoothAM = 22;
        public const byte CourtesyBoothPM = 23;
        public const byte POSFullTime = 24;
        public const byte ClerkFullTimeAP = 25;
        public const byte ClerksFullTimeAR = 26;
        public const byte ClerksFullTimeGroc = 27;
        public const byte ClerksFullTimePerishables = 28;
        public const byte ClerksFullTimeWarehouse = 29;
        public const byte Merchandiser = 30;
        public const byte GroceryManager = 31;
        public const byte EndsPartTime = 32;
        public const byte FirstMeatCutter = 33;
        public const byte NotUsed35 = 35;
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
        public const byte WrappersPartTimeAM = 48;
        public const byte WrappersPartTimePM = 49;
        public const byte HeadClerk = 50;
        public const byte SubShopManager = 51;
        public const byte ClerksFullTimeAM = 52;
        public const byte ClerksPartTimeAM = 53;
        public const byte ClerksPartTimePM = 54;
        public const byte POSPartTime = 55;
        public const byte MarketsKitchenAsstMgr = 56;
        public const byte MarketsKitchenFT = 57;
        public const byte MarketsKitchenPT = 58;
        public const byte KitchenManager = 59;
        public const byte NotUsed60 = 60;
        public const byte PTBakeryMerchandiser = 61;
        public const byte FTCakeAndCreams = 62;
        public const byte CakeAndCreamPT = 63;
        public const byte OverWorkerPT = 64;
        public const byte BenchWorkerPT = 65;
        public const byte ForkLiftOprRecAM = 66;
        public const byte ForkLiftOprRecPM = 67;
        public const byte ForkLiftOprShipAM = 68;
        public const byte ForkLiftOprShipPM = 69;
        public const byte ForkLiftOprMiscAM = 70;
        public const byte ForkLiftOprMiscPM = 71;
        public const byte LoaderAM = 72;
        public const byte LoaderPM = 73;
        public const byte WhseMaintenanceAM = 74;
        public const byte WhseMaintenancePM = 75;
        public const byte SelectorPartTimeAM = 77;
        public const byte SelectorPartTimePM = 78;
        public const byte SelectorFullTimeAM = 79;
        public const byte TempFullTime = 80;
        public const byte SelectorFullTimePM = 81;
        public const byte Inspector = 82;
        public const byte GeneralWarehouseAM = 83;
        public const byte GeneralWarehousePM = 84;
        public const byte DriverTrailer = 85;
        public const byte DriverStraight = 86;
        public const byte Mechanic = 87;
        public const byte GaragePM = 88;
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
