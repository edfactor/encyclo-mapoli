using Demoulas.ProfitSharing.Common.Interfaces;

namespace Demoulas.ProfitSharing.Data.Entities;

/// <summary>
/// https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31887785/DEMOGRAPHICS
/// </summary>

public sealed class PayClassification : ILookupTable<string>
{
    public static class Constants
    {
        public const string ZeroOne = "0"; // legacy numeric code '0'
        public const string Manager = "1";
        public const string AssistantManager = "2";
        public const string FrontEndManager = "10";
        public const string AssistantHeadCashier = "11";
        public const string CashiersAm = "13";
        public const string CashiersPm = "14";
        public const string Cashiers1415 = "15";
        public const string SackersAm = "16";
        public const string SackersPm = "17";
        public const string Sackers1415 = "18";
        public const string Available = "19"; // previously StoreMaintenance/Available
        public const string AssistantManager2 = "2"; // alias retained if needed
        public const string OfficeManager = "20";
        public const string AsstOfficeManager = "21";
        public const string CourtesyBoothFt = "22";
        public const string CourtesyBoothPt = "23";
        public const string PosFullTime = "24";
        public const string ClerkFullTimeAp = "25";
        public const string ClerksFullTimeAr = "26";
        public const string ClerksFullTimeGroc = "27";
        public const string ClerksFullTimePerishables = "28";
        public const string ClerksFullTimeWarehouse = "29";
        public const string Merchandiser = "30";
        public const string GroceryManager = "31";
        public const string EndsPartTime = "32";
        public const string FirstMeatCutter = "33";
        public const string FtBakerBench = "35"; // 35 - FT BAKER/BENCH
        public const string MarketsKitchenPt1617 = "36";
        public const string CafePartTime = "37";
        public const string Receiver = "38";
        public const string LossPrevention = "39";
        public const string SpiritsManager = "4";
        public const string MeatCutters = "40";
        public const string ApprMeatCutters = "41";
        public const string MeatCutterPartTime = "42";
        public const string PartTimeSubshop = "44";
        public const string AsstSubShopManager = "45";
        public const string ServiceCaseFullTime = "46";
        public const string WrappersFullTime = "47";
        public const string WrappersPartTimeAm = "48";
        public const string WrappersPartTimePm = "49";
        public const string AsstSpiritsManager = "5";
        public const string SpiritsClerkFt = "6";
        public const string SpiritsClerkPt = "7";
        public const string HeadClerk = "50";
        public const string SubShopManager = "51";
        public const string ClerksFullTimeAm = "52";
        public const string ClerksPartTimeAm = "53";
        public const string ClerksPartTimePm = "54";
        public const string PosPartTime = "55";
        public const string MarketsKitchenAsstMgr = "56";
        public const string MarketsKitchenFt = "57";
        public const string MarketsKitchenPt = "58";
        public const string KitchenManager = "59";
        public const string SpiritsClerkPtAlt = "59"; // placeholder if needed
        public const string SpiritsClerkFtAlt = "57"; // placeholder if needed
        public const string FtCake = "62";
        public const string PtCake = "63";
        public const string OvenWorkerPt = "64";
        public const string BenchWorkerPt = "65";
        public const string ForkLiftOprRecAm = "66";
        public const string ForkLiftOprRecPm = "67";
        public const string ForkLiftOprShipAm = "68";
        public const string ForkLiftOprShipPm = "69";
        public const string ForkLiftOprMiscAm = "70";
        public const string ForkLiftOprMiscPm = "71";
        public const string LoaderAm = "72";
        public const string LoaderPm = "73";
        public const string GeneralWarehouseFtAm = "74";
        public const string GeneralWarehousePtAm = "75";
        public const string SelectorPartTimeAm = "77";
        public const string SelectorPartTimePm = "78";
        public const string SelectorFullTimeAm = "79";
        public const string SelectorFullTimePm = "81";
        public const string Inspector = "82";
        public const string GeneralWarehouseFtPm = "83";
        public const string GeneralWarehousePtPm = "84";
        public const string DriverTrailer = "85";
        public const string DriverExcel = "86";
        public const string Mechanic = "87";
        public const string FacilityOperations = "89";
        public const string ComputerOperations = "90";
        public const string SignShop = "91";
        public const string Inventory = "92";
        public const string Programming = "93";
        public const string HelpDesk = "94";
        public const string TechnicalSupport = "96";
        public const string ExecutiveOffice = "97";
        public const string Training = "98";
        public const string AdManager = "AD1";
        public const string AdReceptionist = "AD2";
        public const string DrBartender = "DR1";
        public const string DrBusser = "DR2";
        public const string DrHostess = "DR3";
        public const string DrManager = "DR4";
        public const string DrServer = "DR5";
        public const string DrServer2 = "DR6";
        public const string FmMaintenanceAttendant = "FM1";
        public const string FmMaintenanceAttendant2 = "FM2";
        public const string FmManagerFacilityMaintenance = "FM3";
        public const string FmMaintAttend = "FM4";
        public const string FmManager = "FM5";
        public const string FtBartender = "FT1";
        public const string FtManager = "FT2";
        public const string FtServer = "FT3";
        public const string GmGolfCartMaint = "GM1";
        public const string GmGolfCartMaint2 = "GM2";
        public const string GmGroundsMaintenance = "GM3";
        public const string GmGroundsMaintenance2 = "GM4";
        public const string GmManager = "GM5";
        public const string GmMechanic = "GM6";
        public const string GrBusser = "GR1";
        public const string GrManager = "GR2";
        public const string GrServer = "GR3";
        public const string GrSnackShack = "GR4";
        public const string GrPoolSideGrilleRoom = "GR5";
        public const string KtManager = "KT1";
        public const string KtChef = "KT2";
        public const string KtChefKitchen = "KT3";
        public const string KtDishwasher = "KT4";
        public const string KtDishwasherKitchen = "KT5";
        public const string KtPrepChef = "KT6";
        public const string LgManager = "LG1";
        public const string LgLifeguard = "LG2";
        public const string PsProShopServices = "PS1";
        public const string PsProShopServices2 = "PS2";
        public const string PsManager = "PS3";
        public const string PsOutsideServices = "PS4";
        public const string PsOutsideServices2 = "PS5";
        public const string PsStarter = "PS6";
        public const string PsStarter2 = "PS7";
        public const string TnManager = "TN1";
        public const string TnTennisServices = "TN2";
    }

    public required string Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<Demographic>? Employees { get; set; }
}
